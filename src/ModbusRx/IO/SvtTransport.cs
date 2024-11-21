// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using ModbusRx.Message;
using ModbusRx.Unme.Common;

namespace ModbusRx.IO;

/// <summary>
/// Modbus transport.
/// Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern.
/// </summary>
public abstract class SvtTransport : IDisposable
{
    private readonly object _syncLock = new();
    private int _waitToRetryMilliseconds = Modbus.DefaultWaitToRetryMilliseconds;
    private IStreamResource? _streamResource;

    /// <summary>
    /// Initializes a new instance of the <see cref="SvtTransport"/> class.
    ///     This constructor is called by the NullTransport.
    /// </summary>
    internal SvtTransport()
    {
    }

    internal SvtTransport(IStreamResource streamResource)
    {
        Debug.Assert(streamResource is not null, "Argument streamResource cannot be null.");

        _streamResource = streamResource!;
    }

    /// <summary>
    ///     Gets or sets number of times to retry sending message after encountering a failure such as an IOException,
    ///     TimeoutException, or a corrupt message.
    /// </summary>
    public int Retries { get; set; } = Modbus.DefaultRetries;

    /// <summary>
    /// Gets or sets if non-zero, this will cause a second reply to be read if the first is behind the sequence number of the
    /// request by less than this number.  For example, set this to 3, and if when sending request 5, response 3 is
    /// read, we will attempt to re-read responses.
    /// </summary>
    public uint RetryOnOldResponseThreshold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether if set, Slave Busy exception causes retry count to be used.  If false, Slave Busy will cause infinite retries.
    /// </summary>
    public bool SlaveBusyUsesRetryCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of milliseconds the tranport will wait before retrying a message after receiving
    ///     an ACKNOWLEGE or SLAVE DEVICE BUSY slave exception response.
    /// </summary>
    public int WaitToRetryMilliseconds
    {
        get => _waitToRetryMilliseconds;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException(Resources.WaitRetryGreaterThanZero);
            }

            _waitToRetryMilliseconds = value;
        }
    }

    /// <summary>
    ///     Gets or sets the number of milliseconds before a timeout occurs when a read operation does not finish.
    /// </summary>
    public int ReadTimeout
    {
        get => StreamResource.ReadTimeout;
        set => StreamResource.ReadTimeout = value;
    }

    /// <summary>
    ///     Gets or sets the number of milliseconds before a timeout occurs when a write operation does not finish.
    /// </summary>
    public int WriteTimeout
    {
        get => StreamResource.WriteTimeout;
        set => StreamResource.WriteTimeout = value;
    }

    /// <summary>
    ///     Gets the stream resource.
    /// </summary>
    internal IStreamResource StreamResource => _streamResource!;

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    internal virtual T UnicastMessage<T>(ISvtMessage message)
        where T : ISvtMessage, new()
    {
        ISvtMessage response = null!;
        var attempt = 1;
        var success = false;

        do
        {
            try
            {
                lock (_syncLock)
                {
                    Write(message);

                    bool readAgain;
                    do
                    {
                        readAgain = false;
                        response = ReadResponse<T>().Result;
                        var exceptionResponse = response as SvtSlaveExceptionResponse;

                        if (exceptionResponse is not null)
                        {
                            // if SlaveExceptionCode == ACKNOWLEDGE we retry reading the response without resubmitting request
                            readAgain = exceptionResponse.SlaveExceptionCode == Modbus.Acknowledge;

                            if (readAgain)
                            {
                                Debug.WriteLine($"Received ACKNOWLEDGE slave exception response, waiting {_waitToRetryMilliseconds} milliseconds and retrying to read response.");
                                Sleep(WaitToRetryMilliseconds);
                            }
                            else
                            {
                                throw new SvtSlaveException(exceptionResponse);
                            }
                        }
                        else if (ShouldRetryResponse(message, response))
                        {
                            readAgain = true;
                        }
                    }
                    while (readAgain);
                }

                ValidateResponse(message, response);
                success = true;
            }
            catch (SlaveException se)
            {
                if (se.SlaveExceptionCode != Modbus.SlaveDeviceBusy)
                {
                    throw;
                }

                if (SlaveBusyUsesRetryCount && attempt++ > Retries)
                {
                    throw;
                }

                Debug.WriteLine($"Received SLAVE_DEVICE_BUSY exception response, waiting {_waitToRetryMilliseconds} milliseconds and resubmitting request.");
                Sleep(WaitToRetryMilliseconds);
            }
            catch (Exception e)
            {
                if (e is FormatException or
                    NotImplementedException or
                    TimeoutException or
                    IOException)
                {
                    Debug.WriteLine($"{e.GetType().Name}, {Retries - attempt + 1} retries remaining - {e}");

                    if (attempt++ > Retries)
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }
        while (!success);

        return (T)response;
    }

    internal virtual async Task<ISvtMessage> CreateResponse<T>(Task<byte[]> frame)
        where T : ISvtMessage, new()
    {
        var lframe = await frame;
        var functionCode = (ushort)((lframe[7] << 8) | lframe[8]);

        // check for slave exception response else create message from frame
        if (functionCode > Svt.ExceptionOffset)
        {
            return SvtMessageFactory.CreateSvtMessage<SvtSlaveExceptionResponse>(lframe);
        }

        return SvtMessageFactory.CreateSvtMessage<T>(lframe);
    }

    internal void ValidateResponse(ISvtMessage request, ISvtMessage response)
    {
        // always check the function code and slave address, regardless of transport protocol
        if (request.FunctionCode != response.FunctionCode)
        {
            var msg = $"Received response with unexpected Function Code. Expected {request.FunctionCode}, received {response.FunctionCode}.";
            throw new IOException(msg);
        }

        if (request.SlaveAddress != response.SlaveAddress)
        {
            var msg = $"Response slave address does not match request. Expected {response.SlaveAddress}, received {request.SlaveAddress}.";
            throw new IOException(msg);
        }

        // message specific validation
        var req = request as ISvtRequest;

        if (req is not null)
        {
            req.ValidateResponse(response);
        }

        OnValidateResponse(request, response);
    }

    /// <summary>
    ///     Check whether we need to attempt to read another response before processing it (e.g. response was from previous request).
    /// </summary>
    internal bool ShouldRetryResponse(ISvtMessage request, ISvtMessage response)
    {
        // These checks are enforced in ValidateRequest, we don't want to retry for these
        if (request.FunctionCode != response.FunctionCode)
        {
            return false;
        }

        if (request.SlaveAddress != response.SlaveAddress)
        {
            return false;
        }

        return OnShouldRetryResponse(request, response);
    }

    /// <summary>
    ///     Provide hook to check whether receiving a response should be retried.
    /// </summary>
    internal virtual bool OnShouldRetryResponse(ISvtMessage request, ISvtMessage response) => false;

    /// <summary>
    ///     Provide hook to do transport level message validation.
    /// </summary>
    internal abstract void OnValidateResponse(ISvtMessage request, ISvtMessage response);

    internal abstract Task<byte[]> ReadRequest();

    internal abstract Task<ISvtMessage> ReadResponse<T>()
        where T : ISvtMessage, new();

    internal abstract byte[] BuildMessageFrame(ISvtMessage message);

    internal abstract void Write(ISvtMessage message);

    /// <summary>
    ///     Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    ///     unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposableUtility.Dispose(ref _streamResource);
        }
    }

    private static void Sleep(int millisecondsTimeout) =>
        Task.Delay(millisecondsTimeout).Wait();
}

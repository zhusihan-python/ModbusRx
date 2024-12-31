// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using CP.IO.Ports;
using ModbusRx.IO;
using ModbusRx.Message;

namespace ModbusRx.Device;

/// <summary>
///     Svt serial slave device.
/// </summary>
public sealed class SvtSerialSlave : SvtSlave
{
    private SvtSerialSlave(byte unitId, SvtTransport transport)
    : base(unitId, transport)
    {
    }

    private SvtSerialTransport? SerialTransport
    {
        get
        {
            if (Transport is not SvtSerialTransport transport)
            {
                throw new ObjectDisposedException("SerialTransport");
            }

            return transport;
        }
    }

    /// <summary>
    /// Svt ASCII slave factory method.
    /// </summary>
    /// <param name="unitId">The unit identifier.</param>
    /// <param name="serialPort">The serial port.</param>
    /// <returns>A SvtSerialSlave.</returns>
    /// <exception cref="System.ArgumentNullException">serialPort.</exception>
    public static SvtSerialSlave CreateAscii(byte unitId, SerialPortRx serialPort)
    {
        if (serialPort == null)
        {
            throw new ArgumentNullException(nameof(serialPort));
        }

        return CreateAscii(unitId, new SvtSerialPortAdapter(serialPort));
    }

    /// <summary>
    /// Svt ASCII slave factory method.
    /// </summary>
    /// <param name="unitId">The unit identifier.</param>
    /// <param name="streamResource">The stream resource.</param>
    /// <returns>A SvtSerialSlave.</returns>
    /// <exception cref="System.ArgumentNullException">streamResource.</exception>
    public static SvtSerialSlave CreateAscii(byte unitId, IStreamResource streamResource)
    {
        if (streamResource == null)
        {
            throw new ArgumentNullException(nameof(streamResource));
        }

        return new SvtSerialSlave(unitId, new SvtAsciiTransport(streamResource));
    }

    /// <summary>
    /// Start slave listening for requests.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task ListenAsync()
    {
        while (true)
        {
            try
            {
                try
                {
                    // TODO: remove delay once async will be implemented in transport level
                    await Task.Delay(20).ConfigureAwait(false);

                    // read request and build message
                    var frame = await SerialTransport?.ReadRequest()!;
                    var request = SvtMessageFactory.CreateSvtRequest(frame!);

                    if (SerialTransport!.CheckFrame && !SerialTransport.ChecksumsMatch(request, frame!))
                    {
                        var msg = $"Checksums failed to match {string.Join(", ", request.MessageFrame)} != {string.Join(", ", frame!)}.";
                        Debug.WriteLine(msg);
                        throw new IOException(msg);
                    }

                    // only service requests addressed to this particular slave
                    if (request.SlaveAddress != UnitId)
                    {
                        Debug.WriteLine($"NModbus Slave {UnitId} ignoring request intended for NModbus Slave {request.SlaveAddress}");
                        continue;
                    }

                    // perform action
                    var response = ApplyRequest(request);

                    // write response
                    SerialTransport.Write(response);
                }
                catch (IOException ioe)
                {
                    Debug.WriteLine($"IO Exception encountered while listening for requests - {ioe.Message}");
                    SerialTransport?.DiscardInBuffer();
                }
                catch (TimeoutException te)
                {
                    Debug.WriteLine($"Timeout Exception encountered while listening for requests - {te.Message}");
                    SerialTransport?.DiscardInBuffer();
                }

                // TODO better exception handling here, missing FormatException, NotImplemented...
            }
            catch (InvalidOperationException)
            {
                // when the underlying transport is disposed
                break;
            }
        }
    }
}

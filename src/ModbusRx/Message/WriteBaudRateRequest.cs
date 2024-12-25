// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ModbusRx.Data;
using ModbusRx.Unme.Common;

namespace ModbusRx.Message;

/// <summary>
///     Write Multiple Coils request.
/// </summary>
public class WriteBaudRateRequest : AbstractSvtMessageWithData<SvtDataCollection>, ISvtRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WriteBaudRateRequest"/> class.
    ///     Write Multiple Coils request.
    /// </summary>
    public WriteBaudRateRequest()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WriteBaudRateRequest" /> class.
    /// Write Baud Rate request.
    /// </summary>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="masterAddress">The master address.</param>
    /// <param name="slaveAddress">The slave address.</param>
    /// <param name="byteCount">The data byte count.</param>
    /// <param name="data">The data.</param>
    public WriteBaudRateRequest(ushort transactionId, byte masterAddress, byte slaveAddress, ushort byteCount, SvtDataCollection data)
        : base(transactionId, slaveAddress, Svt.BaudRate, Svt.Write)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        else if (data.Count > Svt.MaximumDiscreteRequestResponseSize)
        {
            var msg = $"Maximum amount of data {Svt.MaximumDiscreteRequestResponseSize} baud rate.";
            throw new ArgumentOutOfRangeException("Data", msg);
        }

        MasterAddress = masterAddress;
        SlaveAddress = slaveAddress;
        ByteCount = byteCount;
        Data = data;
    }

    /// <summary>
    /// Gets or sets the byte count.
    /// </summary>
    /// <value>
    /// The byte count.
    /// </value>
    public new ushort ByteCount
    {
        get => MessageImpl.ByteCount;
        set => MessageImpl.ByteCount = value;
    }

    /// <inheritdoc/>
    public override int MinimumFrameSize => 7;

    /// <inheritdoc/>
    public override string ToString() =>
        $"Write {Data.Count} data at address {SlaveAddress}.";

    /// <inheritdoc/>
    public void ValidateResponse(ISvtMessage response)
    {
        var typedResponse = (WriteBaudRateResponse)response;

        if (SlaveAddress != typedResponse?.MasterAddress)
        {
            var msg = $"Unexpected start address in response. Expected {SlaveAddress}, received {typedResponse?.MasterAddress}.";
            throw new IOException(msg);
        }
    }

    /// <inheritdoc/>
    protected override void InitializeUnique(byte[] frame)
    {
        if (frame?.Length < MinimumFrameSize + frame![6])
        {
            throw new FormatException("Message frame does not contain enough bytes.");
        }

        ByteCount = (ushort)((frame![10] << 8) | frame[11]);
        Data = new SvtDataCollection(frame!.Slice(11, ByteCount).ToArray());
    }
}

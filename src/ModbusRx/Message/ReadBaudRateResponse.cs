// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ModbusRx.Data;
using ModbusRx.Unme.Common;

namespace ModbusRx.Message;

/// <summary>
/// ReadBaudRateResponse.
/// </summary>
/// <seealso cref="ModbusRx.Message.ISvtMessage" />
public class ReadBaudRateResponse : AbstractSvtMessageWithData<SvtDataCollection>, ISvtMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadBaudRateResponse"/> class.
    /// </summary>
    public ReadBaudRateResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadBaudRateResponse"/> class.
    /// </summary>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="functionCode">The function code.</param>
    /// <param name="extendCode">The function extend code.</param>
    /// <param name="masterAddress">The byte count.</param>
    /// <param name="slaveAddress">The slave address.</param>
    /// <param name="byteCount">The data byte count.</param>
    /// <param name="data">The data.</param>
    public ReadBaudRateResponse(ushort transactionId, ushort functionCode, byte extendCode, byte masterAddress, byte slaveAddress, ushort byteCount, SvtDataCollection data)
        : base(transactionId, slaveAddress, functionCode, extendCode)
    {
        ByteCount = byteCount;
        Data = data;
        MasterAddress = masterAddress;
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
    public override int MinimumFrameSize => 3;

    /// <inheritdoc/>
    public override string ToString() =>
       $"Read {Data.Count} inputs - {Data}.";

    /// <inheritdoc/>
    protected override void InitializeUnique(byte[] frame)
    {
        if (frame?.Length < TransactionLength - 6)
        {
            throw new FormatException("Message frame data segment does not contain enough bytes.");
        }

        ByteCount = (ushort)((frame![10] << 8) | frame[11]);
        Data = new SvtDataCollection(frame!.Slice(11, ByteCount).ToArray());
    }
}

// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ModbusRx.Data;
using ModbusRx.Unme.Common;

namespace ModbusRx.Message;

/// <summary>
///     Write Svt Register response.
/// </summary>
public class WriteSvtRegisterResponse : AbstractSvtMessageWithData<SvtDataCollection>, ISvtMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WriteSvtRegisterResponse"/> class.
    /// </summary>
    public WriteSvtRegisterResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WriteSvtRegisterResponse"/> class.
    /// </summary>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="functionCode">The function code.</param>
    /// <param name="extendCode">The function extend code.</param>
    /// <param name="masterAddress">The master address.</param>
    /// <param name="slaveAddress">The start address.</param>
    /// <param name="byteCount">The data byte count.</param>
    /// <param name="data">The data.</param>
    public WriteSvtRegisterResponse(ushort transactionId, byte masterAddress, byte slaveAddress, ushort functionCode, byte extendCode, ushort byteCount, SvtDataCollection data)
        : base(transactionId, slaveAddress, functionCode, extendCode)
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

        FunctionCode = functionCode;
        ExtendedCode = extendCode;
        ByteCount = byteCount;
        Data = data;
        MasterAddress = masterAddress;
    }

    /// <summary>
    /// Gets the minimum size of the frame.
    /// </summary>
    /// <value>
    /// The minimum size of the frame.
    /// </value>
    public override int MinimumFrameSize => 6;

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString() =>
        $"Function 0x{FunctionCode:X4} Write {ByteCount} data at address {SlaveAddress}.";

    /// <summary>
    /// Initializes the unique.
    /// </summary>
    /// <param name="frame">The frame.</param>
    protected override void InitializeUnique(byte[] frame)
    {
        if (frame == null)
        {
            throw new ArgumentNullException(nameof(frame));
        }

        ByteCount = (ushort)((frame![10] << 8) | frame[11]);
        Data = new SvtDataCollection(frame!.Slice(11, ByteCount).ToArray());
    }
}

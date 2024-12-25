// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using ModbusRx.Data;
using ModbusRx.Unme.Common;

namespace ModbusRx.Message;

/// <summary>
///     Write Multiple Coils request.
/// </summary>
public class WriteBaudRateResponse : AbstractSvtMessageWithData<SvtDataCollection>, ISvtMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WriteBaudRateResponse"/> class.
    /// </summary>
    public WriteBaudRateResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WriteBaudRateResponse"/> class.
    /// </summary>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="extendCode">The function extend code.</param>
    /// <param name="masterAddress">The master address.</param>
    /// <param name="slaveAddress">The start address.</param>
    /// <param name="byteCount">The data byte count.</param>
    /// <param name="data">The data.</param>
    public WriteBaudRateResponse(ushort transactionId, byte masterAddress, byte slaveAddress, byte extendCode, ushort byteCount, SvtDataCollection data)
        : base(transactionId, slaveAddress, Svt.BaudRate, extendCode)
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
    public override string ToString()
    {
        Debug.Assert(Data is not null, "Argument Data cannot be null.");
        Debug.Assert(Data?.Count == 1, "Data should have a count of 1.");

        var msg = $"Write baud rate {Data} at address {SlaveAddress}.";
        return msg;
    }

    /// <summary>
    /// Validate the specified response against the current request.
    /// </summary>
    /// <param name="response">The Svt Message.</param>
    public void ValidateResponse(ISvtMessage response)
    {
        var typedResponse = (WriteBaudRateResponse)response;

        if (SlaveAddress != typedResponse?.MasterAddress)
        {
            var msg = $"Unexpected start address in response. Expected {SlaveAddress}, received {typedResponse?.MasterAddress}.";
            throw new IOException(msg);
        }
    }

    /// <summary>
    /// Initializes the unique.
    /// </summary>
    /// <param name="frame">The frame.</param>
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

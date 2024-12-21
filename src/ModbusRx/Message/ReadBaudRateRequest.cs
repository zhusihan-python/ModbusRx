// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ModbusRx.Message;

/// <summary>
/// ReadCoilsInputsRequest.
/// </summary>
/// <seealso cref="ModbusRx.Message.AbstractSvtMessage" />
/// <seealso cref="ModbusRx.Message.ISvtRequest" />
public class ReadBaudRateRequest : AbstractSvtMessage, ISvtRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadBaudRateRequest"/> class.
    /// </summary>
    public ReadBaudRateRequest()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadBaudRateRequest"/> class.
    /// </summary>
    /// <param name="masterAddress">The master address.</param>
    /// <param name="slaveAddress">The slave address.</param>
    /// <param name="transactionId">The transaction id.</param>
    public ReadBaudRateRequest(ushort transactionId, byte masterAddress, byte slaveAddress)
        : base(transactionId, slaveAddress, Svt.BaudRate, Svt.Read)
    {
        MasterAddress = masterAddress;
        TransactionId = transactionId;
    }

    /// <inheritdoc/>
    public override int MinimumFrameSize => 6;

    /// <inheritdoc/>
    public override string ToString() =>
        $"Read 0x{FunctionCode:X} starting at address {MasterAddress}.";

    /// <inheritdoc/>
    public void ValidateResponse(ISvtMessage response)
    {
        var typedResponse = (ReadBaudRateResponse)response;

        // best effort validation - the same response for a request for 1 vs 6 coils (same byte count) will pass validation.
        var expectedByteCount = typedResponse?.ProtocolDataUnit.Length;

        if (expectedByteCount != typedResponse?.ByteCount)
        {
            var msg = $"Unexpected byte count. Expected {expectedByteCount}, received {typedResponse?.ByteCount}.";
            throw new IOException(msg);
        }
    }

    /// <inheritdoc/>
    protected override void InitializeUnique(byte[] frame)
    {
        // StartAddress = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
    }
}

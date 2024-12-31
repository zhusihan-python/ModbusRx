// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ModbusRx.Message;

/// <summary>
/// ReadCoilsInputsRequest.
/// </summary>
/// <seealso cref="ModbusRx.Message.AbstractSvtMessage" />
/// <seealso cref="ModbusRx.Message.ISvtRequest" />
public class ReadSvtRegisterRequest : AbstractSvtMessage, ISvtRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadSvtRegisterRequest"/> class.
    /// </summary>
    public ReadSvtRegisterRequest()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadSvtRegisterRequest"/> class.
    /// </summary>
    /// <param name="functionCode">The function code.</param>
    /// <param name="extendCode">The extend code.</param>
    /// <param name="masterAddress">The master address.</param>
    /// <param name="slaveAddress">The slave address.</param>
    /// <param name="transactionId">The transaction id.</param>
    public ReadSvtRegisterRequest(ushort transactionId, ushort functionCode, byte extendCode, byte masterAddress, byte slaveAddress)
        : base(transactionId, slaveAddress, functionCode, extendCode)
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
        var typedResponse = (ReadDeviceIdResponse)response;

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

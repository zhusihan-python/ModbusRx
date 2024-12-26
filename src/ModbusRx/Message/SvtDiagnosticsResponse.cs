// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ModbusRx.Data;
using ModbusRx.Unme.Common;

namespace ModbusRx.Message;

internal class SvtDiagnosticsResponse : AbstractSvtMessageWithData<SvtDataCollection>, ISvtMessage
{
    public SvtDiagnosticsResponse()
    {
    }

    public SvtDiagnosticsResponse(ushort transactionId, ushort functionCode, byte extendCode, byte masterAddress, byte slaveAddress, ushort byteCount, SvtDataCollection data)
        : base(transactionId, slaveAddress, functionCode, extendCode)
    {
        ByteCount = byteCount;
        Data = data;
        MasterAddress = masterAddress;
    }

    public override int MinimumFrameSize => 6;

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

    public override string ToString()
    {
        return $"Svt Diagnostics message return query data - {Data}.";
    }

    protected override void InitializeUnique(byte[] frame)
    {
        ByteCount = (ushort)((frame![10] << 8) | frame[11]);
        Data = new SvtDataCollection(frame!.Slice(11, ByteCount).ToArray());
    }
}

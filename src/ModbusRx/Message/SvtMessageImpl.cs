// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Transactions;
using ModbusRx.Data;

namespace ModbusRx.Message;

/// <summary>
///     Class holding all implementation shared between two or more message types.
///     Interfaces expose subsets of type specific implementations.
/// </summary>
internal class SvtMessageImpl
{
    public SvtMessageImpl()
    {
    }

    public SvtMessageImpl(byte slaveAddress, ushort functionCode, byte extendedCode)
    {
        SlaveAddress = slaveAddress;
        FunctionCode = functionCode;
        ExtendedCode = extendedCode;
    }

    public ushort TransactionId { get; set; }

    public ushort TransactionLength { get; set; }

    public byte AddressLength { get; set; }

    public ushort AddressGroup { get; set; }

    public ushort FunctionCode { get; set; }

    public byte ExtendedCode { get; set; }

    public ushort ByteCount { get; set; }

    public byte? ExceptionCode { get; set; }

    public ushort? NumberOfPoints { get; set; }

    public byte SlaveAddress { get; set; }

    public byte MasterAddress { get; set; }

    public ushort? StartAddress { get; set; }

    public ushort? SubFunctionCode { get; set; }

    public IDataCollection? Data { get; set; }

    public byte[] MessageFrame
    {
        get
        {
            var pdu = ProtocolDataUnit;
            var frame = new MemoryStream(7 + pdu.Length);
            var totalLength = 2 + 7 + pdu.Length + 4;

            frame.WriteByte((byte)(TransactionId >> 8));
            frame.WriteByte((byte)(TransactionId & 0xFF));
            frame.WriteByte((byte)(totalLength >> 8));
            frame.WriteByte((byte)(totalLength & 0xFF));
            frame.WriteByte(0x02);
            frame.WriteByte(MasterAddress);
            frame.WriteByte(SlaveAddress);
            frame.Write(pdu, 0, pdu.Length);

            return frame.ToArray();
        }
    }

    public byte[] ProtocolDataUnit
    {
        get
        {
            var pdu = new List<byte> { };
            pdu.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes(FunctionCode).Reverse() : BitConverter.GetBytes(FunctionCode));
            pdu.Add(ExtendedCode);
            pdu.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes(ByteCount).Reverse() : BitConverter.GetBytes(ByteCount));

            if (Data is not null)
            {
                pdu.AddRange(Data.NetworkBytes);
            }

            return pdu.ToArray();
        }
    }

    public void Initialize(byte[] frame)
    {
        if (frame == null)
        {
            throw new ArgumentNullException(nameof(frame));
        }

        if (frame.Length < Modbus.MinimumFrameSize)
        {
            var msg = $"Message frame must contain at least {Modbus.MinimumFrameSize} bytes of data.";
            throw new FormatException(msg);
        }

        TransactionId = (ushort)((frame[0] << 8) | frame[1]);
        SlaveAddress = frame[5];
        MasterAddress = frame[6];
        FunctionCode = (ushort)((frame[7] << 8) | frame[8]);
        ExtendedCode = frame[9];
    }
}

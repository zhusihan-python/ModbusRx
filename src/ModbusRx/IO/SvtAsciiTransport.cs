// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Text;
using ModbusRx.Message;
using ModbusRx.Utility;

namespace ModbusRx.IO;

/// <summary>
///     Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern.
/// </summary>
internal class SvtAsciiTransport : SvtSerialTransport
{
    internal SvtAsciiTransport(IStreamResource streamResource)
        : base(streamResource) => Debug.Assert(streamResource is not null, "Argument streamResource cannot be null.");

    internal override byte[] BuildMessageFrame(ISvtMessage message)
    {
        var msgFrame = message.MessageFrame;

        var msgFrameAscii = ModbusUtility.GetAsciiBytes(msgFrame);
        var crcAscii = ModbusUtility.GetAsciiBytes(ModbusUtility.CalculateCrc(msgFrame));

        var frame = new MemoryStream(2 + msgFrameAscii.Length + crcAscii.Length + 2);
        frame.WriteByte((byte)Svt.FrameHead1);
        frame.WriteByte((byte)Svt.FrameHead2);
        frame.Write(msgFrameAscii, 0, msgFrameAscii.Length);
        frame.Write(crcAscii, 0, crcAscii.Length);
        frame.WriteByte((byte)Svt.FrameTail1);
        frame.WriteByte((byte)Svt.FrameTail2);

        return frame.ToArray();
    }

    internal override bool ChecksumsMatch(ISvtMessage message, byte[] messageFrame) =>
        new byte[] { messageFrame[^2], messageFrame[^1] }.SequenceEqual(ModbusUtility.CalculateCrc(message.MessageFrame));

    internal override Task<byte[]> ReadRequest() =>
        ReadRequestResponse();

    internal override Task<ISvtMessage> ReadResponse<T>() =>
        CreateResponse<T>(ReadRequestResponse());

    internal async Task<byte[]> ReadRequestResponse()
    {
        // read message frame, removing frame start ':'
        var frameHex = (await StreamResourceUtility.ReadLineAsync(StreamResource))[1..];

        // convert hex to bytes
        var frame = ModbusUtility.HexToBytes(frameHex);
        Debug.WriteLine($"RX: {string.Join(", ", frame)}");

        if (frame.Length < 3)
        {
            throw new IOException("Premature end of stream, message truncated.");
        }

        return frame;
    }
}

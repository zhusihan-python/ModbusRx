// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using ModbusRx.Message;

namespace ModbusRx.IO;

/// <summary>
///     Transport for Serial protocols.
///     Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern.
/// </summary>
public abstract class SvtSerialTransport : SvtTransport
{
    internal SvtSerialTransport(IStreamResource streamResource)
        : base(streamResource) => Debug.Assert(streamResource is not null, "Argument streamResource cannot be null.");

    /// <summary>
    ///     Gets or sets a value indicating whether LRC/CRC frame checking is performed on messages.
    /// </summary>
    public bool CheckFrame { get; set; } = true;

    internal void DiscardInBuffer() =>
        StreamResource.DiscardInBuffer();

    internal override void Write(ISvtMessage message)
    {
        DiscardInBuffer();

        var frame = BuildMessageFrame(message);
        Debug.WriteLine($"TX: {string.Join(", ", frame)}");
        StreamResource.Write(frame, 0, frame.Length);
    }

    internal override async Task<ISvtMessage> CreateResponse<T>(Task<byte[]> frame)
    {
        var response = await base.CreateResponse<T>(frame);

        // compare checksum
        if (CheckFrame && !ChecksumsMatch(response, await frame))
        {
            var msg = $"Checksums failed to match {string.Join(", ", response.MessageFrame)} != {string.Join(", ", frame)}";
            Debug.WriteLine(msg);
            throw new IOException(msg);
        }

        return response;
    }

    internal abstract bool ChecksumsMatch(ISvtMessage message, byte[] messageFrame);

    internal override void OnValidateResponse(ISvtMessage request, ISvtMessage response)
    {
        // no-op
    }
}

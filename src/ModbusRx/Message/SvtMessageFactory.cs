// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ModbusRx.Message;

/// <summary>
///     Svt message factory.
/// </summary>
public static class SvtMessageFactory
{
    /// <summary>
    ///     Minimum request frame length.
    /// </summary>
    private const int MinRequestFrameLength = 3;

    /// <summary>
    ///     Create a Svt message.
    /// </summary>
    /// <typeparam name="T">Svt message type.</typeparam>
    /// <param name="frame">Bytes of Svt frame.</param>
    /// <returns>New Svt message based on type and frame bytes.</returns>
    public static T CreateSvtMessage<T>(byte[] frame)
        where T : ISvtMessage, new()
    {
        ISvtMessage message = new T();
        message.Initialize(frame);

        return (T)message;
    }

    /// <summary>
    ///     Create a Svt request.
    /// </summary>
    /// <param name="frame">Bytes of Svt frame.</param>
    /// <returns>Svt request.</returns>
    public static ISvtMessage CreateSvtRequest(byte[] frame)
    {
        if (frame?.Length < MinRequestFrameLength)
        {
            throw new FormatException($"Argument 'frame' must have a length of at least {MinRequestFrameLength} bytes.");
        }

        var functionCode = (ushort)((frame![7] << 8) | frame![8]);
        var extendCode = frame![9];
        return (functionCode, extendCode) switch
        {
            (Svt.DeviceId, Svt.Read) => CreateSvtMessage<ReadDeviceIdRequest>(frame),
            (Svt.BaudRate, Svt.Read) => CreateSvtMessage<ReadBaudRateRequest>(frame),
            (Svt.BaudRate, Svt.Write) => CreateSvtMessage<WriteBaudRateRequest>(frame),
            _ => throw new ArgumentException($"Unsupported function code {functionCode}", nameof(frame)),
        };
    }
}

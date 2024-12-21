// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ModbusRx.Data;

/// <summary>
///     Svt message containing data.
/// </summary>
public interface ISvtDataCollection
{
    /// <summary>
    ///     Gets the network bytes.
    /// </summary>
    byte[] NetworkBytes { get; }

    /// <summary>
    ///     Gets the byte count.
    /// </summary>
    byte ByteCount { get; }
}

// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ModbusRx.Message;

namespace ModbusRx.Device;

/// <summary>
///     Svt Slave request event args containing information on the message.
/// </summary>
public class SvtSlaveRequestEventArgs : EventArgs
{
    internal SvtSlaveRequestEventArgs(ISvtMessage message) => Message = message;

    /// <summary>
    ///     Gets the message.
    /// </summary>
    public ISvtMessage Message { get; }
}

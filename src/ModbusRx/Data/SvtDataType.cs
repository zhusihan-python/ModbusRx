// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ModbusRx.Data;

/// <summary>
///     Types of data supported by the Svt protocol.
/// </summary>
public enum SvtDataType
{
    /// <summary>
    ///     Read/write register.
    /// </summary>
    HoldingRegister,

    /// <summary>
    ///     Readonly register.
    /// </summary>
    InputRegister,
}

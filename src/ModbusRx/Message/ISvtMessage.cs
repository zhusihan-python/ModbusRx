// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ModbusRx.Message;

/// <summary>
///     A message built by the master (client) that initiates a Modbus transaction.
/// </summary>
public interface ISvtMessage
{
    /// <summary>
    ///     Gets or sets a unique identifier assigned to a message when using the IP protocol.
    /// </summary>
    ushort TransactionId { get; set; }

    /// <summary>
    ///     Gets or sets the frame length, including header and tail (2 bytes).
    /// </summary>
    ushort TransactionLength { get; set; }

    /// <summary>
    ///     Gets or sets the length of the address length (2 byte).
    /// </summary>
    byte AddressLength { get; set; }

    /// <summary>
    ///     Gets or sets address of the slave (server).
    /// </summary>
    byte SlaveAddress { get; set; }

    /// <summary>
    ///     Gets or sets address of the master (server).
    /// </summary>
    byte MasterAddress { get; set; }

    /// <summary>
    ///     Gets or sets the command code (2 bytes).
    /// </summary>
    ushort FunctionCode { get; set; }

    /// <summary>
    ///     Gets or sets the extended command code (1 byte).
    /// </summary>
    byte ExtendedCode { get; set; }

    /// <summary>
    ///     Gets or sets the data length (2 bytes).
    /// </summary>
    ushort ByteCount { get; set; }

    /// <summary>
    ///     Gets composition of the function code and message data.
    /// </summary>
    byte[] ProtocolDataUnit { get; }

    /// <summary>
    ///     Gets composition of the entire ModbusSvt message frame.
    /// </summary>
    byte[] MessageFrame { get; }

    /// <summary>
    ///     Initializes a ModbusSvt message from the specified message frame.
    /// </summary>
    /// <param name="frame">Bytes of ModbusSvt frame.</param>
    void Initialize(byte[] frame);
}

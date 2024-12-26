// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using ModbusRx.IO;

namespace ModbusRx.Device;

/// <summary>
///     Svt master device.
/// </summary>
public interface ISvtMaster : ICancelable
{
    /// <summary>
    ///     Gets transport used by this master.
    /// </summary>
    SvtTransport? Transport { get; }

    /// <summary>
    ///    Asynchronously reads device id.
    /// </summary>
    /// <param name="transactionId">Transaction id of data.</param>
    /// <param name="masterAddress">Address of source.</param>
    /// <param name="slaveAddress">Address of destination.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    Task<byte[]> ReadDeviceIdAsync(ushort transactionId, byte masterAddress, byte slaveAddress);

    /// <summary>
    ///    Asynchronously reads baud rate.
    /// </summary>
    /// <param name="transactionId">Transaction id of data.</param>
    /// <param name="masterAddress">Address of source.</param>
    /// <param name="slaveAddress">Address of destination.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    Task<byte[]> ReadBaudRateAsync(ushort transactionId, byte masterAddress, byte slaveAddress);

    /// <summary>
    ///    Asynchronously reads contiguous block of holding registers.
    /// </summary>
    /// <param name="transactionId">Address of device to read values from.</param>
    /// <param name="masterAddress">Address to begin reading.</param>
    /// <param name="slaveAddress">Number of holding registers to read.</param>
    /// <param name="byteCount">Size of data.</param>
    /// <param name="data">Content of data.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    Task WriteBaudRateAsync(ushort transactionId, byte masterAddress, byte slaveAddress, ushort byteCount, byte[] data);
}

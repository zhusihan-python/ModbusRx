// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ModbusRx.Data;
using ModbusRx.IO;
using ModbusRx.Message;

namespace ModbusRx.Device;

/// <summary>
///     Svt master device.
/// </summary>
public abstract class SvtMaster : SvtDevice, ISvtMaster
{
    internal SvtMaster(SvtTransport transport)
    : base(transport)
    {
    }

    /// <summary>
    ///    Asynchronously reads device id.
    /// </summary>
    /// <param name="transactionId">Transaction id of data.</param>
    /// <param name="masterAddress">Address of source.</param>
    /// <param name="slaveAddress">Address of destination.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    public Task<byte[]> ReadDeviceIdAsync(ushort transactionId, byte masterAddress, byte slaveAddress)
    {
        var request = new ReadDeviceIdRequest(
            transactionId,
            Svt.DeviceId,
            Svt.Read,
            masterAddress,
            slaveAddress);

        return PerformReadRegistersAsync(request);
    }

    /// <summary>
    ///    Asynchronously reads contiguous block of holding registers.
    /// </summary>
    /// <param name="transactionId">Transaction id of data.</param>
    /// <param name="masterAddress">Address of source.</param>
    /// <param name="slaveAddress">Address of destination.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    public Task<byte[]> ReadBaudRateAsync(ushort transactionId, byte masterAddress, byte slaveAddress)
    {
        var request = new ReadBaudRateRequest(
            transactionId,
            masterAddress,
            slaveAddress);

        return PerformReadRegistersAsync(request);
    }

    /// <summary>
    ///    Asynchronously writes a block of data.
    /// </summary>
    /// <param name="transactionId">Transaction id of data.</param>
    /// <param name="masterAddress">Address of source.</param>
    /// <param name="slaveAddress">Address of destination.</param>
    /// <param name="byteCount">Size of data.</param>
    /// <param name="data">Content of data.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public Task WriteBaudRateAsync(ushort transactionId, byte masterAddress, byte slaveAddress, ushort byteCount, byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "Data can not be null.");
        }

        var request = new WriteBaudRateRequest(
            transactionId,
            masterAddress,
            slaveAddress,
            (ushort)data.Length,
            new SvtDataCollection(data));

        return PerformWriteRequestAsync<WriteBaudRateResponse>(request);
    }

    private byte[] PerformReadRegisters(ReadDeviceIdRequest request)
    {
        var response = Transport?.UnicastMessage<ReadDeviceIdResponse>(request);
        return response!.Data.ToArray();
    }

#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
    private Task<byte[]> PerformReadRegistersAsync(ReadDeviceIdRequest request) =>
        Task.Factory.StartNew(() => PerformReadRegisters(request));

    private byte[] PerformReadRegisters(ReadBaudRateRequest request)
    {
        var response = Transport?.UnicastMessage<ReadBaudRateResponse>(request);

        return response!.Data.ToArray();
    }

    private Task<byte[]> PerformReadRegistersAsync(ReadBaudRateRequest request) =>
    Task.Factory.StartNew(() => PerformReadRegisters(request));

    private Task PerformWriteRequestAsync<T>(ISvtMessage request)
        where T : ISvtMessage, new() =>
        Task.Factory.StartNew(() => Transport?.UnicastMessage<T>(request));
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
}

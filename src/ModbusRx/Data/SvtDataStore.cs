// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using ModbusRx.Unme.Common;

namespace ModbusRx.Data;

/// <summary>
///     Object simulation of device memory map.
///     The underlying collections are thread safe when using the SvtMaster API to read/write values.
///     You can use the SyncRoot property to synchronize direct access to the SvtDataStore collections.
/// </summary>
public class SvtDataStore
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SvtDataStore" /> class.
    /// </summary>
    public SvtDataStore()
    {
        HoldingRegisters = new() { SvtDataType = SvtDataType.HoldingRegister };
        InputRegisters = new() { SvtDataType = SvtDataType.InputRegister };
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SvtDataStore"/> class.
    /// </summary>
    /// <param name="holdingRegisters">List of holding register values.</param>
    /// <param name="inputRegisters">List of input register values.</param>
    internal SvtDataStore(
        IList<byte> holdingRegisters,
        IList<byte> inputRegisters)
    {
        HoldingRegisters = new(holdingRegisters) { SvtDataType = SvtDataType.HoldingRegister };
        InputRegisters = new(inputRegisters) { SvtDataType = SvtDataType.InputRegister };
    }

    /// <summary>
    ///     Occurs when the DataStore is written to via a Svt command.
    /// </summary>
    public event EventHandler<SvtDataStoreEventArgs>? DataStoreWrittenTo;

    /// <summary>
    ///     Occurs when the DataStore is read from via a Svt command.
    /// </summary>
    public event EventHandler<SvtDataStoreEventArgs>? DataStoreReadFrom;

    /// <summary>
    ///     Gets the holding registers.
    /// </summary>
    public NewModbusDataCollection<byte> HoldingRegisters { get; }

    /// <summary>
    ///     Gets the input registers.
    /// </summary>
    public NewModbusDataCollection<byte> InputRegisters { get; }

    /// <summary>
    ///     Gets an object that can be used to synchronize direct access to the DataStore collections.
    /// </summary>
    public object SyncRoot { get; } = new();

    /// <summary>
    ///     Retrieves subset of data from collection.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    /// <typeparam name="TU">The type of elements in the collection.</typeparam>
    internal static T ReadData<T, TU>(
        SvtDataStore dataStore,
        NewModbusDataCollection<TU> dataSource,
        ushort startAddress,
        ushort count,
        object syncRoot)
        where T : Collection<TU>, new()
        where TU : struct
    {
        SvtDataStoreEventArgs dataStoreEventArgs;
        var startIndex = startAddress + 1;

        if (startIndex < 0 || dataSource.Count < startIndex + count)
        {
            throw new InvalidModbusRequestException(Svt.IllegalDataAddress);
        }

        TU[] dataToRetrieve;
        lock (syncRoot)
        {
            dataToRetrieve = dataSource.Slice(startIndex, count).ToArray();
        }

        var result = new T();
        for (var i = 0; i < count; i++)
        {
            result.Add(dataToRetrieve[i]);
        }

        dataStoreEventArgs = SvtDataStoreEventArgs.CreateDataStoreEventArgs(startAddress, dataSource.SvtDataType, result);
        dataStore.DataStoreReadFrom?.Invoke(dataStore, dataStoreEventArgs);
        return result;
    }

    /// <summary>
    ///     Write data to data store.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    internal static void WriteData<TData>(
        SvtDataStore dataStore,
        IEnumerable<TData> items,
        NewModbusDataCollection<TData> destination,
        ushort startAddress,
        object syncRoot)
        where TData : struct
    {
        SvtDataStoreEventArgs dataStoreEventArgs;
        var startIndex = startAddress + 1;

        if (startIndex < 0 || destination.Count < startIndex + items.Count())
        {
            throw new InvalidModbusRequestException(Svt.IllegalDataAddress);
        }

        lock (syncRoot)
        {
            Update(items, destination, startIndex);
        }

        dataStoreEventArgs = SvtDataStoreEventArgs.CreateDataStoreEventArgs(
            startAddress,
            destination.SvtDataType,
            items);

        dataStore.DataStoreWrittenTo?.Invoke(dataStore, dataStoreEventArgs);
    }

    /// <summary>
    ///     Updates subset of values in a collection.
    /// </summary>
    internal static void Update<T>(IEnumerable<T> items, IList<T> destination, int startIndex)
    {
        if (startIndex < 0 || destination.Count < startIndex + items.Count())
        {
            throw new InvalidModbusRequestException(Svt.IllegalDataAddress);
        }

        var index = startIndex;

        foreach (var item in items)
        {
            destination[index] = item;
            ++index;
        }
    }
}

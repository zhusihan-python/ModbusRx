// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ModbusRx.Data;

/// <summary>
///     Data story factory.
/// </summary>
public static class SvtDataStoreFactory
{
    /// <summary>
    /// Factory method for default data store - register values set to 0 and discrete values set to false.
    /// </summary>
    /// <returns>A SvtDataStore.</returns>
    public static SvtDataStore CreateDefaultDataStore() =>
        CreateDefaultDataStore(ushort.MaxValue, ushort.MaxValue);

    /// <summary>
    ///     Factory method for default data store - register values set to 0 and discrete values set to false.
    /// </summary>
    /// <param name="holdingRegistersCount">Number of holding registers.</param>
    /// <param name="inputRegistersCount">Number of input registers.</param>
    /// <returns>New instance of Data store with defined inputs/outputs.</returns>
    public static SvtDataStore CreateDefaultDataStore(ushort holdingRegistersCount, ushort inputRegistersCount)
    {
        var holdingRegs = new byte[holdingRegistersCount];
        var inputRegs = new byte[inputRegistersCount];

        return new SvtDataStore(holdingRegs, inputRegs);
    }

    /// <summary>
    ///     Factory method for test data store.
    /// </summary>
    internal static SvtDataStore CreateTestDataStore()
    {
        var dataStore = new SvtDataStore();

        for (var i = 1; i < 3000; i++)
        {
            var value = i % 2 > 0;
            dataStore.HoldingRegisters.Add((byte)i);
            dataStore.InputRegisters.Add((byte)(i * 10));
        }

        return dataStore;
    }
}

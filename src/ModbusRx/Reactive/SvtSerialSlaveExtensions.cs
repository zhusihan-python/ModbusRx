// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using ModbusRx.Data;
using ModbusRx.Device;
using ModbusRx.Message;

namespace ModbusRx.Reactive
{
    /// <summary>
    /// SvtSerialSlaveExtensions.
    /// </summary>
    public static class SvtSerialSlaveExtensions
    {
        /// <summary>
        /// Writes the Svt Registers.
        /// </summary>
        /// <param name="slave">The slave.</param>
        /// <param name="masterAddress">The master address.</param>
        /// <param name="slaveAddress">The slave address.</param>
        /// <param name="functionCode">The function code.</param>
        /// <param name="extendCode">The extend code.</param>
        /// <param name="valuesToWrite">The values to write.</param>
        /// <returns>
        /// Observable SvtSerialSlave.
        /// </returns>
        public static IObservable<SvtSerialSlave> WriteSvtRegisters(this IObservable<SvtSerialSlave> slave, byte masterAddress, byte slaveAddress, ushort functionCode, byte extendCode, IObservable<byte[]> valuesToWrite)
        {
            slave.CombineLatest(
            valuesToWrite, (slave, data) => (slave, data))
                .Subscribe(source => SvtSlave.WriteRegister(
                    new WriteSvtRegisterResponse(1, masterAddress, slaveAddress, functionCode, extendCode, (ushort)source.data.Length, new SvtDataCollection(source.data)), source.slave.DataStore, source.slave.DataStore.InputRegisters));
            return slave;
        }
    }
}

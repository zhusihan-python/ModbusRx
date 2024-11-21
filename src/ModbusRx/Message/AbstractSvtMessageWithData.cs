// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ModbusRx.Data;

namespace ModbusRx.Message;

/// <summary>
/// AbstractSvtMessageWithData.
/// </summary>
/// <typeparam name="TData">The type of the data.</typeparam>
/// <seealso cref="ModbusRx.Message.AbstractSvtMessage" />
public abstract class AbstractSvtMessageWithData<TData> : AbstractSvtMessage
    where TData : IDataCollection
{
    internal AbstractSvtMessageWithData()
    {
    }

    internal AbstractSvtMessageWithData(byte slaveAddress, byte functionCode, byte extendedCode)
        : base(slaveAddress, functionCode, extendedCode)
    {
    }

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    /// <value>
    /// The data.
    /// </value>
    public TData Data
    {
        get => (TData)MessageImpl.Data!;
        set => MessageImpl.Data = value;
    }
}

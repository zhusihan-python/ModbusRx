// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using ModbusRx.Utility;

namespace ModbusRx.Data;

/// <summary>
///     Event args for read write actions performed on the SvtDataStore.
/// </summary>
public sealed class SvtDataStoreEventArgs : EventArgs
{
    private SvtDataStoreEventArgs(ushort startAddress, SvtDataType svtDataType)
    {
        StartAddress = startAddress;
        SvtDataType = svtDataType;
    }

    /// <summary>
    ///     Gets type of Svt data (e.g. Holding register).
    /// </summary>
    public SvtDataType SvtDataType { get; }

    /// <summary>
    ///     Gets start address of data.
    /// </summary>
    public ushort StartAddress { get; }

    /// <summary>
    ///     Gets data that was read or written.
    /// </summary>
    public DiscriminatedUnion<ReadOnlyCollection<byte>, ReadOnlyCollection<ushort>>? Data { get; private set; }

    internal static SvtDataStoreEventArgs CreateDataStoreEventArgs<T>(ushort startAddress, SvtDataType svtDataType, IEnumerable<T> data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (typeof(T) == typeof(byte))
        {
            var a = new ReadOnlyCollection<byte>(data.Cast<byte>().ToArray());

            return new SvtDataStoreEventArgs(startAddress, svtDataType)
            {
                Data = DiscriminatedUnion<ReadOnlyCollection<byte>, ReadOnlyCollection<ushort>>.CreateA(a),
            };
        }
        else if (typeof(T) == typeof(ushort))
        {
            var b = new ReadOnlyCollection<ushort>(data.Cast<ushort>().ToArray());

            return new SvtDataStoreEventArgs(startAddress, svtDataType)
            {
                Data = DiscriminatedUnion<ReadOnlyCollection<byte>, ReadOnlyCollection<ushort>>.CreateB(b),
            };
        }
        else
        {
            throw new ArgumentException("Generic type T should be of type byte or ushort");
        }
    }
}

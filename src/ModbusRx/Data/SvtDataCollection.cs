// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ModbusRx.Data;

/// <summary>
///     Collection of data values.
/// </summary>
public class SvtDataCollection : Collection<byte>, IDataCollection
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SvtDataCollection" /> class.
    /// </summary>
    public SvtDataCollection()
        : this(new List<byte>())
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SvtDataCollection" /> class.
    /// </summary>
    /// <param name="bytes">Array for data collection.</param>
    public SvtDataCollection(params byte[] bytes)
        : this((IList<byte>)bytes)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SvtDataCollection" /> class.
    /// </summary>
    /// <param name="bytes">List for discrete collection.</param>
    public SvtDataCollection(IList<byte> bytes)
        : this(new List<byte>(bytes))
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SvtDataCollection" /> class.
    /// </summary>
    /// <param name="bytes">List for discrete collection.</param>
    internal SvtDataCollection(List<byte> bytes)
        : base(bytes)
    {
        Debug.Assert(bytes is not null, "Discrete bits is null.");
    }

    /// <summary>
    ///     Gets the network bytes.
    /// </summary>
    public byte[] NetworkBytes
    {
        get
        {
            return Items.ToArray();
        }
    }

    /// <summary>
    ///     Gets the byte count.
    /// </summary>
    public byte ByteCount => (byte)Count;

    /// <summary>
    ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </returns>
    public override string ToString() =>
        string.Concat("{", string.Join(", ", this.Select(b => b.ToString("X2")).ToArray()), "}");
}

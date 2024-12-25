// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using ModbusRx.Data;
using ModbusRx.Message;
using Xunit;

namespace ModbusRx.UnitTests.Message;

/// <summary>
/// WriteBaudRateRequestFixture.
/// </summary>
public class WriteBaudRateRequestFixture
{
    /// <summary>
    /// Creates the write baud rate request.
    /// </summary>
    [Fact]
    public void CreateWriteBaudRateRequest()
    {
        var col = new SvtDataCollection(new byte[] { 0x03 });
        var request = new WriteBaudRateRequest(0x0001, 0x12, 0x11, 0x01, col);
        Assert.Equal(Svt.BaudRate, request.FunctionCode);
        Assert.Equal(0x12, request.MasterAddress);
        Assert.Equal(0x11, request.SlaveAddress);
        Assert.Equal(0x01, request.ByteCount);
        Assert.Equal(col.NetworkBytes, request.Data.NetworkBytes);
    }

    /// <summary>
    /// Creates the write baud rate too much data.
    /// </summary>
    [Fact]
    public void CreateWriteBaudRateRequestTooMuchData() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new WriteBaudRateRequest(0x0001, 0x12, 0x11, 0x01, MessageUtility.CreateDefaultCollection<SvtDataCollection, byte>(0x00, Svt.MaximumDiscreteRequestResponseSize + 1)));

    /// <summary>
    /// Creates the maximum size of the write baud rate.
    /// </summary>
    [Fact]
    public void CreateWriteBaudRateRequestMaxSize()
    {
        var request = new WriteBaudRateRequest(0x0001, 0x12, 0x11, 0x01, MessageUtility.CreateDefaultCollection<SvtDataCollection, byte>(0x00, Svt.MaximumDiscreteRequestResponseSize));

        Assert.Equal(Svt.MaximumDiscreteRequestResponseSize, request.Data.Count);
    }

    /// <summary>
    /// Converts to string_writebaudraterequest.
    /// </summary>
    [Fact]
    public void ToString_WriteBaudRateRequest()
    {
        var col = new SvtDataCollection(new byte[] { 0x03 });
        var request = new WriteBaudRateRequest(0x0001, 0x12, 0x11, 0x01, col);

        Assert.Equal("Write 1 data at address 17.", request.ToString());
    }
}

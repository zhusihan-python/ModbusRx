// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using ModbusRx.Data;
using ModbusRx.Message;
using Xunit;

namespace ModbusRx.UnitTests.Message;

/// <summary>
/// ReadCoilsInputsResponseFixture.
/// </summary>
public class ReadDeviceIdResponseFixture
{
    /// <summary>
    /// Creates the read coils response.
    /// </summary>
    [Fact]
    public void CreateReadDeviceIdResponse()
    {
        var response = new ReadDeviceIdResponse(0x0001, Svt.ReadDeviceId, Svt.ReadSuccess, 0x12, 0x11, 5, new SvtDataCollection(new byte[] { 1, 2, 3, 4, 5 }));
        Assert.Equal(Svt.ReadDeviceId, response.FunctionCode);
        Assert.Equal(0x11, response.SlaveAddress);
        Assert.Equal(5, response.ByteCount);
        var col = new SvtDataCollection(new byte[] { 1, 2, 3, 4, 5 });
        Assert.Equal(col.NetworkBytes, response.Data.NetworkBytes);
    }

    /// <summary>
    /// Creates the read inputs response.
    /// </summary>
    [Fact]
    public void CreateReadBaudRateResponse()
    {
        var response = new ReadBaudRateResponse(0x0001, Svt.BaudRate, Svt.ReadSuccess, 0x12, 0x11, 6, new SvtDataCollection(new List<byte> { 1, 2, 3, 4, 5, 6 }));
        Assert.Equal(Svt.BaudRate, response.FunctionCode);
        Assert.Equal(0x12, response.MasterAddress);
        Assert.Equal(0x11, response.SlaveAddress);
        Assert.Equal(6, response.ByteCount);
        var col = new SvtDataCollection(new List<byte> { 1, 2, 3, 4, 5, 6 });
        Assert.Equal(col.NetworkBytes, response.Data.NetworkBytes);
    }

    /// <summary>
    /// Converts to string_device_id.
    /// </summary>
    [Fact]
    public void ToString_DeviceId()
    {
        var response = new ReadDeviceIdResponse(0x0001, Svt.ReadDeviceId, Svt.ReadSuccess, 0x12, 0x11, 5, new SvtDataCollection(new byte[] { 1, 2, 3, 4, 5 }));

        Assert.Equal("Read 5 inputs - {01, 02, 03, 04, 05}.", response.ToString());
    }

    /// <summary>
    /// Converts to string_baud_rate.
    /// </summary>
    [Fact]
    public void ToString_BaudRate()
    {
        var response = new ReadBaudRateResponse(0x0001, Svt.BaudRate, Svt.ReadSuccess, 0x12, 0x11, 6, new SvtDataCollection(new List<byte> { 1, 2, 3, 4, 5, 6 }));

        Assert.Equal("Read 6 inputs - {01, 02, 03, 04, 05, 06}.", response.ToString());
    }
}

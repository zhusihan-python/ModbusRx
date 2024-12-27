// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ModbusRx.Message;
using Xunit;

namespace ModbusRx.UnitTests.Message;

/// <summary>
/// ReadDeviceIdRequestFixture.
/// </summary>
public class ReadDeviceIdRequestFixture
{
    /// <summary>
    /// Creates the read device id request.
    /// </summary>
    [Fact]
    public void CreateReadDeviceIdRequest()
    {
        var request = new ReadDeviceIdRequest(0x0001, Svt.DeviceId, Svt.Read, 0x12, 0x11);
        Assert.Equal(Svt.DeviceId, request.FunctionCode);
        Assert.Equal(0x01, request.TransactionId);
        Assert.Equal(0x12, request.MasterAddress);
        Assert.Equal(0x11, request.SlaveAddress);
    }

    /// <summary>
    /// Creates the read baudrate request.
    /// </summary>
    [Fact]
    public void CreateReadBaudRateRequest()
    {
        var request = new ReadBaudRateRequest(0x0001, 0x12, 0x11);
        Assert.Equal(Svt.BaudRate, request.FunctionCode);
        Assert.Equal(0x01, request.TransactionId);
        Assert.Equal(0x12, request.MasterAddress);
        Assert.Equal(0x11, request.SlaveAddress);
    }

    /// <summary>
    /// Converts to string_readdeviceidrequest.
    /// </summary>
    [Fact]
    public void ToString_ReadDeviceIdRequest()
    {
        var request = new ReadDeviceIdRequest(0x0001, Svt.DeviceId, Svt.Read, 0x12, 0x11);

        Assert.Equal("Read 0x20 starting at address 18.", request.ToString());
    }

    /// <summary>
    /// Converts to string_readbaudraterequest.
    /// </summary>
    [Fact]
    public void ToString_ReadBaudRateRequest()
    {
        var request = new ReadBaudRateRequest(0x0001, 0x12, 0x11);

        Assert.Equal("Read 0x22 starting at address 18.", request.ToString());
    }
}

// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using ModbusRx.Data;
using ModbusRx.Message;
using Xunit;

namespace ModbusRx.UnitTests.Message;

/// <summary>
/// WriteBaudRateResponseFixture.
/// </summary>
public class WriteBaudRateResponseFixture
{
    /// <summary>
    /// Creates the write baud rate response.
    /// </summary>
    [Fact]
    public void CreateWriteBaudRateResponse()
    {
        var col = new SvtDataCollection(new byte[] { 0x03 });
        var response = new WriteBaudRateResponse(0x0001, 0x12, 0x11, 0x88, 0x01, col);
        Assert.Equal(Svt.BaudRate, response.FunctionCode);
        Assert.Equal(0x12, response.MasterAddress);
        Assert.Equal(0x11, response.SlaveAddress);
        Assert.Equal(0x88, response.ExtendedCode);
        byte[] expectedResult = { 0x03 };
        Assert.Equal(expectedResult, response.Data.NetworkBytes);
    }

    /// <summary>
    /// Creates the write baud rate response too much data.
    /// </summary>
    [Fact]
    public void CreateWriteBaudRateResponseTooMuchData() => Assert.Throws<ArgumentOutOfRangeException>(
        () => new WriteBaudRateResponse(0x0001, 0x12, 0x11, 0x88, 0x01, MessageUtility.CreateDefaultCollection<SvtDataCollection, byte>(0x00, Svt.MaximumDiscreteRequestResponseSize + 1)));

    /// <summary>
    /// Creates the maximum size of the write baud rate response.
    /// </summary>
    [Fact]
    public void CreateWriteBaudRateResponseMaxSize()
    {
        var response = new WriteBaudRateResponse(0x0001, 0x12, 0x11, 0x88, 0x01, MessageUtility.CreateDefaultCollection<SvtDataCollection, byte>(0x00, Svt.MaximumDiscreteRequestResponseSize));
        Assert.Equal(Svt.MaximumDiscreteRequestResponseSize, response.Data.Count);
    }

    /// <summary>
    /// Converts to string_test.
    /// </summary>
    [Fact]
    public void ToString_Test()
    {
        var col = new SvtDataCollection(new byte[] { 0x03 });
        var response = new WriteBaudRateResponse(0x0001, 0x12, 0x11, 0x88, 0x01, col);

        Assert.Equal("Write baud rate {03} at address 17.", response.ToString());
    }
}

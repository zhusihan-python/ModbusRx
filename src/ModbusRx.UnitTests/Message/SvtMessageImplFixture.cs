// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using ModbusRx.Message;
using Xunit;

namespace ModbusRx.UnitTests.Message;

/// <summary>
/// SvtMessageImplFixture.
/// </summary>
public class SvtMessageImplFixture
{
    /// <summary>
    /// Svt the message ctor initializes properties.
    /// </summary>
    [Fact]
    public void SvtMessageCtorInitializesProperties()
    {
        var messageImpl = new SvtMessageImpl(0x0001, 0x11, Svt.BaudRate, Svt.Read);
        Assert.Equal(0x11, messageImpl.SlaveAddress);
        Assert.Equal(Svt.BaudRate, messageImpl.FunctionCode);
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    [Fact]
    public void Initialize()
    {
        var messageImpl = new SvtMessageImpl();
        messageImpl.Initialize(new byte[] { 0x00, 0x01, 0x00, 0x14, 0x02, 0x12, 0x11, 0x00, 0x20, 0xaa, 0x00, 0x01, 0x01 });
        Assert.Equal(0x12, messageImpl.SlaveAddress);
        Assert.Equal(0x11, messageImpl.MasterAddress);
        Assert.Equal(0x0020, messageImpl.FunctionCode);
    }

    /// <summary>
    /// Checcks the initialize frame null.
    /// </summary>
    [Fact]
    public void ChecckInitializeFrameNull()
    {
        var messageImpl = new SvtMessageImpl();
        Assert.Throws<ArgumentNullException>(() => messageImpl.Initialize(null!));
    }

    /// <summary>
    /// Initializes the invalid frame.
    /// </summary>
    [Fact]
    public void InitializeInvalidFrame()
    {
        var messageImpl = new SvtMessageImpl();
        Assert.Throws<FormatException>(() => messageImpl.Initialize(new byte[] { 1 }));
    }

    /// <summary>
    /// Protocols the data unit.
    /// </summary>
    [Fact]
    public void ProtocolDataUnit()
    {
        var messageImpl = new SvtMessageImpl(0x0001, 0x11, Svt.BaudRate, Svt.Read);
        var reverseBytes = BitConverter.GetBytes(Svt.BaudRate).Reverse().ToArray();
        var expectedResult = reverseBytes.Concat(new byte[] { Svt.Read, 0x00, 0x00 }).ToArray();
        Assert.Equal(expectedResult, messageImpl.ProtocolDataUnit);
    }

    /// <summary>
    /// Messages the frame.
    /// </summary>
    [Fact]
    public void MessageFrame()
    {
        var messageImpl = new SvtMessageImpl(0x0001, 0x11, Svt.BaudRate, Svt.Read);
        byte[] expectedMessageFrame = { 0x00, 0x01, 0x00, 0x12, 0x02, 0x00, 0x11, 0x00, 0x22, Svt.Read, 0x00, 0x00 };
        Assert.Equal(expectedMessageFrame, messageImpl.MessageFrame);
    }
}

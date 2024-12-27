// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using ModbusRx.Message;
using Xunit;

namespace ModbusRx.UnitTests.Message;

/// <summary>
/// SvtMessageFixture.
/// </summary>
public class SvtMessageFixture
{
    /// <summary>
    /// Protocols the data unit read device id request.
    /// </summary>
    [Fact]
    public void ProtocolDataUnitReadDeviceIdRequest()
    {
        AbstractSvtMessage message = new ReadDeviceIdRequest(0x0001, Svt.ReadDeviceId, Svt.Read, 0x12, 0x11);
        var reverseBytes = BitConverter.GetBytes(Svt.ReadDeviceId).Reverse().ToArray();
        var expectedResult = reverseBytes.Concat(new byte[] { Svt.Read, 0x00, 0x00 }).ToArray();
        Assert.Equal(expectedResult, message.ProtocolDataUnit);
    }

    /// <summary>
    /// Messages the frame read device id request.
    /// </summary>
    [Fact]
    public void MessageFrameReadDeviceIdRequest()
    {
        AbstractSvtMessage message = new ReadDeviceIdRequest(0x0001, Svt.ReadDeviceId, Svt.Read, 0x12, 0x11);
        byte[] expectedMessageFrame = { 0x00, 0x01, 0x00, 0x12, 0x02, 0x12, 0x11, 0x00, 0x20, Svt.Read, 0x00, 0x00 };
        Assert.Equal(expectedMessageFrame, message.MessageFrame);
    }

    /// <summary>
    /// Svt the message to string overriden.
    /// </summary>
    [Fact]
    public void SvtMessageToStringOverriden()
    {
        var messageTypes = from message in typeof(AbstractSvtMessage).GetTypeInfo().Assembly.GetTypes()
                           let typeInfo = message.GetTypeInfo()
                           where !typeInfo.IsAbstract && typeInfo.IsSubclassOf(typeof(AbstractSvtMessage))
                           select message;

        foreach (var messageType in messageTypes)
        {
            Assert.NotNull(
                messageType.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        }
    }

    /// <summary>
    /// Asserts the svt message properties are equal.
    /// </summary>
    /// <param name="obj1">The obj1.</param>
    /// <param name="obj2">The obj2.</param>
    internal static void AssertSvtMessagePropertiesAreEqual(ISvtMessage obj1, ISvtMessage obj2)
    {
        Assert.Equal(obj1.FunctionCode, obj2.FunctionCode);
        Assert.Equal(obj1.SlaveAddress, obj2.SlaveAddress);
        Assert.Equal(obj1.MessageFrame, obj2.MessageFrame);
        Assert.Equal(obj1.ProtocolDataUnit, obj2.ProtocolDataUnit);
    }
}

// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusRx.IO;
using ModbusRx.Message;
using Moq;
using Xunit;

namespace ModbusRx.UnitTests.IO;

/// <summary>
/// ModbusAsciiTransportFixture.
/// </summary>
public class SvtAsciiTransportFixture
{
    /// <summary>
    /// Gets the stream resource.
    /// </summary>
    /// <value>
    /// The stream resource.
    /// </value>
    private static IStreamResource StreamResource => new Mock<IStreamResource>(MockBehavior.Strict).Object;

    /// <summary>
    /// Builds the message frame.
    /// </summary>
    [Fact]
    public void BuildMessageFrame()
    {
        byte[] expected = { 0x3c, 0x28, 0x00, 0x01, 0x00, 0x12, 0x02, 0x12, 0x11, 0x00, 0x20, 0x55, 0x00, 0x00, 0x1F, 0x17, 0x29, 0x3e };
        var request = new ReadDeviceIdRequest(0x0001, Svt.DeviceId, Svt.Read, 0x12, 0x11);
        var actual = new SvtAsciiTransport(StreamResource)
            .BuildMessageFrame(request);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Reads the request response.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReadRequestResponseAsync()
    {
        var mock = new Mock<IStreamResource>(MockBehavior.Strict);
        var stream = mock.Object;
        var transport = new SvtAsciiTransport(stream);
        var calls = 0;
        var input = "<(\x00\x01\x00\x14\x03\x14\x12\x11\x00 \x99\x00\x01\x88\xc8\x06)>";
        var bytes = input.Select(c => Convert.ToByte(c)).ToArray();

        mock.Setup(s => s.ReadAsync(It.Is<byte[]>(x => x.Length == 1), 0, 1).Result)
            .Returns((byte[] buffer, int offset, int count) =>
            {
                buffer[offset] = bytes[calls++];
                return 1;
            });

        byte[] expected = { 0, 1, 0, 20, 3, 20, 18, 17, 0, 32, 153, 0, 1, 136, 200, 6 };
        Assert.Equal(expected, await transport.ReadRequestResponse());
        mock.VerifyAll();
    }

    /// <summary>
    /// Reads the request response not enough bytes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReadRequestResponseNotEnoughBytesAsync()
    {
        var mock = new Mock<IStreamResource>(MockBehavior.Strict);
        var stream = mock.Object;
        var transport = new SvtAsciiTransport(stream);
        var calls = 0;
        var bytes = Encoding.ASCII.GetBytes("<(\x00)>");

        mock.Setup(s => s.ReadAsync(It.Is<byte[]>(x => x.Length == 1), 0, 1).Result)
            .Returns((byte[] buffer, int offset, int count) =>
            {
                buffer[offset] = bytes[calls++];
                return 1;
            });

        await Assert.ThrowsAsync<IOException>(() => transport.ReadRequestResponse());
        mock.VerifyAll();
    }

    /// <summary>
    /// Checksumses the match succeed.
    /// </summary>
    [Fact]
    public void ChecksumsMatchSucceed()
    {
        byte[] expected = { 0x3c, 0x28, 0x00, 0x01, 0x00, 0x12, 0x02, 0x12, 0x11, 0x00, 0x20, 0x55, 0x00, 0x00, 0x1F, 0x17, 0x29, 0x3e };
        var transport = new SvtAsciiTransport(StreamResource);
        var message = new ReadDeviceIdRequest(0x0001, Svt.DeviceId, Svt.Read, 0x12, 0x11);
        byte[] frame = { 0x00, 0x01, 0x00, 0x12, 0x02, 0x12, 0x11, 0x00, 0x20, 0x55, 0x00, 0x00, 0x1F, 0x17 };

        Assert.True(transport.ChecksumsMatch(message, frame));
    }

    /// <summary>
    /// Checksumses the match fail.
    /// </summary>
    [Fact]
    public void ChecksumsMatchFail()
    {
        var transport = new SvtAsciiTransport(StreamResource);
        var message = new ReadDeviceIdRequest(0x0001, Svt.DeviceId, Svt.Read, 0x12, 0x11);
        byte[] frame = { 0x00, 0x01, 0x00, 0x12, 0x02, 0x12, 0x11, 0x00, 0x20, 0x55, 0x00, 0x00, 0x1F, 0x11 };

        Assert.False(transport.ChecksumsMatch(message, frame));
    }
}

// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CP.IO.Ports;
using ModbusRx.Data;
using ModbusRx.IO;
using ModbusRx.Message;

namespace ModbusRx.Device;

/// <summary>
///     Svt serial master device.
/// </summary>
public sealed class SvtSerialMaster : SvtMaster, ISvtSerialMaster
{
    private SvtSerialMaster(SvtTransport transport)
        : base(transport)
    {
    }

    /// <summary>
    ///     Gets the Svt Transport.
    /// </summary>
    SvtSerialTransport? ISvtSerialMaster.Transport =>
        (SvtSerialTransport?)Transport;

    /// <summary>
    /// Svt ASCII master factory method.
    /// </summary>
    /// <param name="serialPort">The serial port.</param>
    /// <returns>A SvtSerialMaster.</returns>
    /// <exception cref="System.ArgumentNullException">serialPort.</exception>
    public static SvtSerialMaster CreateAscii(SerialPortRx serialPort)
    {
        if (serialPort == null)
        {
            throw new ArgumentNullException(nameof(serialPort));
        }

        return CreateAscii(new SerialPortAdapter(serialPort));
    }

    /// <summary>
    /// Svt ASCII master factory method.
    /// </summary>
    /// <param name="tcpClient">The TCP client.</param>
    /// <returns>A SvtSerialMaster.</returns>
    /// <exception cref="System.ArgumentNullException">tcpClient.</exception>
    public static SvtSerialMaster CreateAscii(TcpClientRx tcpClient)
    {
        if (tcpClient == null)
        {
            throw new ArgumentNullException(nameof(tcpClient));
        }

        return CreateAscii(new TcpClientAdapter(tcpClient));
    }

    /// <summary>
    /// Svt ASCII master factory method.
    /// </summary>
    /// <param name="udpClient">The UDP client.</param>
    /// <returns>A SvtSerialMaster.</returns>
    /// <exception cref="System.ArgumentNullException">udpClient.</exception>
    public static SvtSerialMaster CreateAscii(UdpClientRx udpClient)
    {
        if (udpClient == null)
        {
            throw new ArgumentNullException(nameof(udpClient));
        }

        if (!udpClient.Client.Connected)
        {
            throw new InvalidOperationException(Resources.UdpClientNotConnected);
        }

        return CreateAscii(new UdpClientAdapter(udpClient));
    }

    /// <summary>
    /// Svt ASCII master factory method.
    /// </summary>
    /// <param name="streamResource">The stream resource.</param>
    /// <returns>A SvtSerialMaster.</returns>
    /// <exception cref="System.ArgumentNullException">streamResource.</exception>
    public static SvtSerialMaster CreateAscii(IStreamResource streamResource)
    {
        if (streamResource == null)
        {
            throw new ArgumentNullException(nameof(streamResource));
        }

        return new SvtSerialMaster(new SvtAsciiTransport(streamResource));
    }

    /// <summary>
    ///     Serial Line only.
    ///     Diagnostic function which loops back the original data.
    ///     NModbus only supports looping back one ushort value, this is a limitation of the "Best Effort" implementation of
    ///     the RTU protocol.
    /// </summary>
    /// <param name="slaveAddress">Address of device to test.</param>
    /// <param name="data">Data to return.</param>
    /// <returns>Return true if slave device echoed data.</returns>
    public bool ReturnQueryData(byte slaveAddress, ushort data)
    {
        var request = new SvtDiagnosticsResponse(
            0x0001, Svt.Diagnostics, Svt.Read, 0x12, slaveAddress, 0, new SvtDataCollection(new byte[] { 0, }));

        var response = Transport?.UnicastMessage<SvtDiagnosticsResponse>(request);

        return response!.Data.Count == 6;
    }
}

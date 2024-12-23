// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ModbusRx;

/// <summary>
///     Defines constants related to the Modbus protocol.
/// </summary>
internal static class Svt
{
    // frame head 0x3C 0x28 frame tail 0x29 0x3E
    public const byte FrameHead1 = 0x3C;  // "<"
    public const byte FrameHead2 = 0x28;  // "("
    public const byte FrameTail1 = 0x29;  // ")"
    public const byte FrameTail2 = 0x3E;  // ">"

    // supported function codes
    public const ushort DiagnosticsReturnQueryData = 0;
    public const byte ReadDeviceId = 0x0020;
    public const byte BaudRate = 0x0022;

    public const int MaximumDiscreteRequestResponseSize = 65535;
    public const int MaximumRegisterRequestResponseSize = 4095;

    // supported extend codes
    public const byte Read = 0x55;
    public const byte Write = 0x66;
    public const byte ReadSuccess = 0xAA;
    public const byte WriteScueess = 0x88;
    public const byte WriteFailed = 0x99;

    // modbus slave exception offset that is added to the function code, to flag an exception
    public const byte ExceptionOffset = 128;

    // modbus slave exception codes
    public const byte IllegalFunction = 1;
    public const byte IllegalDataAddress = 2;
    public const byte Acknowledge = 5;
    public const byte SlaveDeviceBusy = 6;

    // default setting for number of retries for IO operations
    public const int DefaultRetries = 3;

    // default number of milliseconds to wait after encountering an ACKNOWLEGE or SLAVE DEVIC BUSY slave exception response.
    public const int DefaultWaitToRetryMilliseconds = 250;

    // default setting for IO timeouts in milliseconds
    public const int DefaultTimeout = 1000;

    // smallest supported message frame size (sans checksum)
    public const int MinimumFrameSize = 2;

    public const ushort CoilOn = 0xFF00;
    public const ushort CoilOff = 0x0000;

    // IP slaves should be addressed by IP
    public const byte DefaultIpSlaveUnitId = 0;

    // An existing connection was forcibly closed by the remote host
    public const int ConnectionResetByPeer = 10054;

    // Existing socket connection is being closed
    public const int WSACancelBlockingCall = 10004;

    // used by the ASCII tranport to indicate end of message
    // public const string NewLine = ")>";
    public const string NewLine = "293E";
}

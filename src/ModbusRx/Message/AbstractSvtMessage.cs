// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ModbusRx.Message;

/// <summary>
///     Abstract Svt message.
/// </summary>
public abstract class AbstractSvtMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractSvtMessage"/> class.
    ///     Abstract Svt message.
    /// </summary>
    internal AbstractSvtMessage() => MessageImpl = new SvtMessageImpl();

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractSvtMessage"/> class.
    ///     Abstract Svt message.
    /// </summary>
    internal AbstractSvtMessage(ushort transactionId, byte slaveAddress, ushort functionCode, byte extendedCode)
        => MessageImpl = new SvtMessageImpl(transactionId, slaveAddress, functionCode, extendedCode);

    /// <summary>
    /// Gets or sets the transaction identifier.
    /// </summary>
    /// <value>
    /// The transaction identifier.
    /// </value>
    public ushort TransactionId
    {
        get => MessageImpl.TransactionId;
        set => MessageImpl.TransactionId = value;
    }

    /// <summary>
    /// Gets or sets the transaction identifier.
    /// </summary>
    /// <value>
    /// The transaction length.
    /// </value>
    public ushort TransactionLength
    {
        get => MessageImpl.TransactionLength;
        set => MessageImpl.TransactionLength = value;
    }

    /// <summary>
    /// Gets or sets the function code.
    /// </summary>
    /// <value>
    /// The function code.
    /// </value>
    public ushort FunctionCode
    {
        get => MessageImpl.FunctionCode;
        set => MessageImpl.FunctionCode = value;
    }

    /// <summary>
    /// Gets or sets the extend code.
    /// </summary>
    /// <value>
    /// The extend code.
    /// </value>
    public byte ExtendedCode
    {
        get => MessageImpl.ExtendedCode;
        set => MessageImpl.ExtendedCode = value;
    }

    /// <summary>
    /// Gets or sets the address length.
    /// </summary>
    /// <value>
    /// The address length.
    /// </value>
    public byte AddressLength
    {
        get => MessageImpl.AddressLength;
        set => MessageImpl.AddressLength = value;
    }

    /// <summary>
    /// Gets or sets the slave address.
    /// </summary>
    /// <value>
    /// The slave address.
    /// </value>
    public byte SlaveAddress
    {
        get => MessageImpl.SlaveAddress;
        set => MessageImpl.SlaveAddress = value;
    }

    /// <summary>
    /// Gets or sets the slave address.
    /// </summary>
    /// <value>
    /// The master address.
    /// </value>
    public byte MasterAddress
    {
        get => MessageImpl.MasterAddress;
        set => MessageImpl.MasterAddress = value;
    }

    /// <summary>
    /// Gets or sets the data byte count.
    /// </summary>
    /// <value>
    /// The data byte count.
    /// </value>
    public ushort ByteCount
    {
        get => MessageImpl.ByteCount;
        set => MessageImpl.ByteCount = value;
    }

    /// <summary>
    /// Gets the message frame.
    /// </summary>
    /// <value>
    /// The message frame.
    /// </value>
    public byte[] MessageFrame =>
        MessageImpl.MessageFrame;

    /// <summary>
    /// Gets the protocol data unit.
    /// </summary>
    /// <value>
    /// The protocol data unit.
    /// </value>
    public virtual byte[] ProtocolDataUnit =>
        MessageImpl.ProtocolDataUnit;

    /// <summary>
    /// Gets the minimum size of the frame.
    /// </summary>
    /// <value>
    /// The minimum size of the frame.
    /// </value>
    public abstract int MinimumFrameSize { get; }

    internal SvtMessageImpl MessageImpl { get; }

    /// <summary>
    /// Initializes the specified frame.
    /// </summary>
    /// <param name="frame">The frame.</param>
    public void Initialize(byte[] frame)
    {
        if (frame?.Length < MinimumFrameSize)
        {
            var msg = $"Message frame must contain at least {MinimumFrameSize} bytes of data.";
            throw new FormatException(msg);
        }

        MessageImpl.Initialize(frame!);
        InitializeUnique(frame!);
    }

    /// <summary>
    /// Initializes the unique.
    /// </summary>
    /// <param name="frame">The frame.</param>
    protected abstract void InitializeUnique(byte[] frame);
}

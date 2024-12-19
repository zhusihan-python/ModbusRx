![License](https://img.shields.io/github/license/ChrisPulman/ModbusRx.svg) [![Build](https://github.com/ChrisPulman/ModbusRx/actions/workflows/BuildOnly.yml/badge.svg)](https://github.com/ChrisPulman/ModbusRx/actions/workflows/BuildOnly.yml) ![Nuget](https://img.shields.io/nuget/dt/ModbusRx?color=pink&style=plastic) [![NuGet](https://img.shields.io/nuget/v/ModbusRx.svg?style=plastic)](https://www.nuget.org/packages/ModbusRx)

![Alt](https://repobeats.axiom.co/api/embed/003b94efc502d37d72af8cc355f4cf4ef95f317a.svg "Repobeats analytics image")

<p align="left">
  <a href="https://github.com/ChrisPulman/ModbusRx">
    <img alt="ModbusRx" src="https://github.com/ChrisPulman/ModbusRx/blob/main/Images/ModbusRx.png" width="200"/>
  </a>
</p>

# ModbusRx
A Reactive Modbus implementation based on NModbus4


## Using ModbusRx
### What is ModbusRx? 

ModbusRx has been developed using the NModbus4 library as a base
ModbusRx is a reactive implementation of the Modbus protocol. 
It is designed to be used with the Reactive Extensions library (Rx.NET) and provides a simple and easy to use API for reading and writing Modbus data.
a. Modbus/RTU Master/Slave 
b. Modbus/ASCII Master/Slave 
c. Modbus/TCP Master/Slave 
d. Modbus/UDP Master/Slave 


### NModbus Master API

Create Master, Set Retries, Set ReadTimeout, Master.Read, Update to UI, Master.Dispose, CreateRtu, CreateAscii, CreateIp(TcpClient), CreateIp(UdpClient), Retries [Property], ReadTimeout[Property], Master.ReadCoils, Master.ReadInputs, Master.ReadHoldingRegisters, Master.ReadInputRegisters, WriteSingleCoil, WriteSingleRegisterSet Retries 

 
### CreateRtu

RTU master create connection to serial port. 


### CreateAscii

Ascii master create connection to serial port.  


### CreateIp(TcpClient) 
IP master create connection to TCP. 


### CreateIp(UdpClient) 
IP master create connection to UDP. 


### ReadTimeout[Property] 
Gets or sets the number of milliseconds before a timeout occurs when a read 
operation does not finish. 


### ReadCoils 
Read coils status. 


### ReadInputs 
Read input status. 


### ReadHoldingRegisters 
Read holding registers value. 


### ReadInputRegisters 
Read input registers value. 


### WriteSingleCoil 
Write a coil value. 


### WriteSingleRegister 
Write a holding register value. 


### NModbus Slave API 
Create Slave 

### CreateRtu 
Create a RTU slave connection. 


### CreateAscii 
Create an Ascii slave connection. 


### CreateTcp 
Create a TCP slave connection. Max value of TCP slave from master is 50. 


### CreateUdp 
Create a UDP slave connection. 


### CreateDefaultDataStore 
Create memory space in Datastore. AI and AO抯 Datastore set to 0. DI and DO抯 Datastore set 
to false. For each memory default size is 65535 and range is 1 to 65535. 


### ModbusSlaveRequestReceived[event] 
Occurs when a modbus slave receives a request. You can disassemble request packet and set 
particular action here. 


### DataStoreWrittenTo[event] 
Occurs when a slave receive write AO or DO command to Datastore from master via a 
Modbus command. Address starts from 1 to 65535. 
 

Remarks 
搒lave?is defined by ModbusSlave and create slave connection. For example: To create TCP 
slave connection, syntax is = Create.TcpIpSlave("127.0.0.1"). 


### Listen 
Slave starts listening for requests.  


### CoilDiscretes[DO data array] 
Data array of DO values. Address starts from 1 to 65535.  


### InputDiscretes [DI data array] 
Data array of DI values. You can store DI values in the array. Address starts from 1 to 65535. 


### HoldingRegisters [AO data array] 
Data array of AO values. Address starts from 1 to 65535. 


### InputRegisters [AI data array] 
Data array of AI values. You can store AI values in the array. Address starts from 1 to 65535. 


## Common API 
### Dispose 
Performs application-defined tasks associated with freeing, releasing, or resetting 
unmanaged resources. 


SVT Protocol

帧头固定2位，内容固定为0x3c 0x28，即b"<("，
帧序号固定2位，序号为当前发送协议帧的序号，由主设备指定。从设备在回复命令时，帧序号需与接收到的命令中的保持一致，
帧长度固定2位，帧长度为包括帧头帧尾在内的整个数据帧长度，
地址长度固定1位，地址组长度。
地址组长度x位，不固定，长度为地址组长度的值x，
命令码固定2位，内容为操作设备的命令，
命令扩展码固定1位，内容有四个取值，0x55:读，0xAA:读回应，0x66：写，0x99: 写回应，
数据长度固定2位，为数据段的长度，
数据段y位，不固定，长度为数据长度的值y，
CRC校验固定2位，检验内容为帧序号至数据段所有内容，不包括帧头和帧尾，
帧尾固定2位，内容固定为0x29 0x3e，即b")>"

ProtocolDataUnit = FunctionCode + ExtendedCode + ByteCount + Data  
                   命令码 + 命令扩展码 + 数据长度 + 数据段
MessageFrame = TransactionId + FrameLength + AddressLength + SlaveAddress + MasterAddress + ProtocolDataUnit
				帧序号 + 帧长度 + 地址组长度 + 源地址 + 目标地址 + 命令码 + 命令扩展码 + 数据长度 + 数据段

帧头	帧序号	帧长度	地址长度  地址组  命令码	扩展码	数据长度	数据内容	CRC校验	  帧尾
3C 28	00 01	00 13	  02	   11 12  00 20	     66	     00 01	     01	         BB 0C	  29 3E

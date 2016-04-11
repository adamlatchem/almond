#region License
// Copyright 2016 Adam Latchem
// 
//    Licensed under the Apache License, Version 2.0 (the "License"); 
//    you may not use this file except in compliance with the License. 
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0 
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//    See the License for the specific language governing permissions and 
//    limitations under the License. 
//
#endregion
using Almond.LineDriver;
using Almond.ProtocolDriver.Packets;
using Almond.SQLDriver;
using System;
using System.Text;

namespace Almond.ProtocolDriver
{
    /// <summary>
    /// Defines packets and state machine for the MySQL Client/Server protocol.
    /// </summary>
    internal class ProtocolDriver : IDisposable
    {
        #region Constants
        /// <summary>
        /// The length of the packet header - this is not truly constant as if compression is
        /// enabled the size of the header increases.
        /// </summary>
        public static int PACKET_HEADER_LENGTH = 4;
        #endregion

        #region Members
        /// <summary>
        /// The line driver used to communicate with the server.
        /// </summary>
        private TCPLineDriver _lineDriver;

        /// <summary>
        /// The expected sequence Id of the next packet from the server.
        /// </summary>
        private byte _sequenceNumber;

        /// <summary>
        /// Client Capabilities used when constructing/sending packets
        /// </summary>
        public Capability ClientCapability
        {
            get; set;
        }

        /// <summary>
        /// The client encoding
        /// </summary>
        public Encoding ClientEncoding
        {
            get; private set;
        }
        #endregion

        /// <summary>
        /// Use to:
        /// . check for an error packet and throw an exception if there is
        /// . check type of packet is expected type T
        /// . return packet cast to type T
        /// </summary>
        /// <typeparam name="T">The packet type we expect to have</typeparam>
        /// <param name="packet"></param>
        /// <returns>Packet cast to type T</returns>
        private T Expect<T>(IServerPacket packet) where T : IServerPacket
        {
            if (packet is ERR)
                throw new ProtocolException(packet.ToString());
            if (!(packet is T))
                throw new ProtocolException(
                    String.Format(
                        "Unexpected {0} packet, should have been {1}",
                        typeof(T).Name,
                        packet.GetType().Name)
                    );
            return (T)packet;
        }

        /// <summary>
        /// Constructor - connects to the given connection string using Hostname and Port,
        /// initiates reading and returns synchronously.
        /// </summary>
        /// <param name="connectionStringBuilder"></param>
        public ProtocolDriver(ConnectionStringBuilder connectionStringBuilder)
        {
            _lineDriver = new TCPLineDriver(connectionStringBuilder);

            ClientCapability =
                Capability.CLIENT_PROTOCOL_41 |
                Capability.CLIENT_SECURE_CONNECTION |
                Capability.CLIENT_PLUGIN_AUTH;

            ClientEncoding = System.Text.Encoding.ASCII;

            IServerPacket packet = ReceivePacket();
            ServerHandshake serverHandshakePacket = Expect<ServerHandshake>(packet);
            Capability serverCapability = serverHandshakePacket.Capabilities;

            // Ensure the client capability is a subset of server capability
            ClientCapability &= serverCapability;

            HandshakeResponse handshakeResponsePacket = new HandshakeResponse();
            handshakeResponsePacket.MaxPacketSize = connectionStringBuilder.MaxPacketSize;
            handshakeResponsePacket.CharacterSet = 11; // ascii_general_ci
            handshakeResponsePacket.Username = connectionStringBuilder.Username;
            handshakeResponsePacket.MySQLNativePassword(connectionStringBuilder.Password, serverHandshakePacket, ClientEncoding);
            if (!String.IsNullOrEmpty(connectionStringBuilder.Database))
            {
                handshakeResponsePacket.Database = connectionStringBuilder.Database;
                ClientCapability |= Capability.CLIENT_CONNECT_WITH_DB;
            }
            SendPacket(handshakeResponsePacket);

            packet = ReceivePacket();
            OK okPacket = Expect<OK>(packet);
        }

        /// <summary>
        /// Blocks until the next packet is completly read from the socket.
        /// </summary>
        /// <returns></returns>
        public IServerPacket ReceivePacket()
        {
            ChunkReader chunkReader = _lineDriver.ChunkReader;
            IServerPacket result = CreatePacket(chunkReader);
            return result;
        }

        /// <summary>
        /// Sends client packet as a chunk to the line driver.
        /// </summary>
        /// <returns></returns>
        public void SendPacket(IClientPacket packet)
        {
            ChunkWriter chunkWriter = _lineDriver.ChunkWriter;
            chunkWriter.NewChunk();

            // populate empty length field - it is written before sending.
            chunkWriter.WriteMyInt3(0);
            chunkWriter.WriteMyInt1(_sequenceNumber);
            packet.ToWriter(chunkWriter, ClientCapability, ClientEncoding);
            int payloadLength = chunkWriter.WrittenSoFar() - PACKET_HEADER_LENGTH;
            chunkWriter.WriteMyInt3((UInt32)payloadLength, 0);

            _lineDriver.SendChunk();
            _sequenceNumber++;
        }

        /// <summary>
        /// Packet factory method
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IServerPacket CreatePacket(ChunkReader reader)
        {
            reader.StartNewPacket();
            UInt32 payloadLength = reader.ReadMyInt3();
            byte sequenceNumber = reader.ReadMyInt1();
            if (sequenceNumber != _sequenceNumber)
                throw new ProtocolException(
                    String.Format(
                        "Expected sequence number {0} but got {1}",
                        _sequenceNumber,
                        sequenceNumber));
            _sequenceNumber++;

            IServerPacket result = null;
            switch (reader.PeekByte())
            {
                case 9:
                case 10:
                    result = new ServerHandshake();
                    break;
                case 0:
                case 0xFE:
                    result = new OK();
                    break;
                case 0xFF:
                    result = new ERR();
                    break;
                default:
                    result = new Generic();
                    break;
            }

            result.FromReader(reader, payloadLength, ClientCapability, ClientEncoding);
            return result;
        }

        #region IDisposable
        public void Dispose()
        {
            if (_lineDriver != null)
            {
                _lineDriver.Dispose();
                _lineDriver = null;
            }
        }
        #endregion
    }
}

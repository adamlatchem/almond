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
using System.Data;
using System.Text;

namespace Almond.ProtocolDriver
{
    /// <summary>
    /// Defines packets and state machine for the MySQL Client/Server protocol.
    /// </summary>
    public class ProtocolDriver : IDisposable, IServerPacket
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
        public T Expect<T>(IServerPacket packet) where T : IServerPacket
        {
            if (packet is ERR)
                throw new ProtocolException((ERR)packet);
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
                Capability.CLIENT_DEPRECATE_EOF |
                Capability.CLIENT_PLUGIN_AUTH;

            byte characterSet = 11; // ascii_general_ci
            ClientEncoding = Mapping.CharSetToEncoding(characterSet);

            IServerPacket packet = ReceivePacket(this);
            ServerHandshake serverHandshakePacket = Expect<ServerHandshake>(packet);
            Capability serverCapability = serverHandshakePacket.Capabilities;

            // Ensure the client capability is a subset of server capability
            ClientCapability &= serverCapability;

            HandshakeResponse handshakeResponsePacket = new HandshakeResponse();
            handshakeResponsePacket.MaxPacketSize = connectionStringBuilder.MaxPacketSize;
            handshakeResponsePacket.CharacterSet = characterSet;
            handshakeResponsePacket.Username = connectionStringBuilder.Username;
            handshakeResponsePacket.MySQLNativePassword(connectionStringBuilder.Password, serverHandshakePacket, ClientEncoding);
            if (!String.IsNullOrEmpty(connectionStringBuilder.Database))
            {
                handshakeResponsePacket.Database = connectionStringBuilder.Database;
                ClientCapability |= Capability.CLIENT_CONNECT_WITH_DB;
            }
            SendPacket(handshakeResponsePacket);

            packet = ReceivePacket(this);
            OK okPacket = Expect<OK>(packet);
        }

        /// <summary>
        /// Parse packet header and check sequence number. Return the payload length.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>Payload length from packet</returns>
        private UInt32 StripHeader(ChunkReader reader)
        {
            UInt32 payloadLength = reader.ReadMyInt3();
            byte sequenceNumber = reader.ReadMyInt1();
            if (sequenceNumber != _sequenceNumber)
                throw new ProtocolException(
                    String.Format(
                        "Expected sequence number {0} but got {1}",
                        _sequenceNumber,
                        sequenceNumber));
            _sequenceNumber++;

            return payloadLength;
        }

        /// <summary>
        /// Blocks until the next packet is completely read from the socket.
        /// </summary>
        /// <returns></returns>
        public IServerPacket ReceivePacket(IServerPacket factoryPacket)
        {
            ChunkReader chunkReader = _lineDriver.ChunkReader;
            chunkReader.StartNewPacket();
            UInt32 payloadLength = StripHeader(chunkReader);
            if (payloadLength >= 0xffffff)
                throw new NotImplementedException("Unable to read large packets yet");

            IServerPacket result = factoryPacket.FromWireFormat(chunkReader, payloadLength, this);
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
            packet.ToWireFormat(chunkWriter, this);
            int payloadLength = chunkWriter.WrittenSoFar() - PACKET_HEADER_LENGTH;
            if (payloadLength >= 0xffffff)
                throw new NotImplementedException("Not able to send large packets yet");
            chunkWriter.WriteMyInt3((UInt32)payloadLength, 0);

            _lineDriver.SendChunk();
            _sequenceNumber++;
        }

        #region IServerPacket
        /// <summary>
        /// Packet factory method - header has already been read by the chunk reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>The packet that was read</returns>
        public IServerPacket FromWireFormat(ChunkReader reader, UInt32 payloadLength, ProtocolDriver driver)
        {
            IServerPacket result = null;
            byte possiblePacketType = reader.PeekByte();
            switch (possiblePacketType)
            {
                case 9:
                case 10:
                    result = new ServerHandshake();
                    break;
                case 0:
                    result = new OK();
                    break;
                case 0xFE:
                    result = new EOF();
                    break;
                case 0xFF:
                    result = new ERR();
                    break;
                default:
                    result = new Generic();
                    break;
            }

            result.FromWireFormat(reader, payloadLength, this);
            return result;
        }
        #endregion

        /// <summary>
        /// Factory to handle the reply for COM_QUERY packet.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="payloadLength"></param>
        /// <returns></returns>
        public IServerPacket CreateResultSet(ChunkReader reader, UInt32 payloadLength)
        {
            IServerPacket result = null;
            byte possiblePacketType = reader.PeekByte();
            switch (possiblePacketType)
            {
                case 0:
                    result = new OK();
                    break;
                case 0xFF:
                    result = new ERR();
                    break;
                default:
                    result = new ResultSet();
                    break;
            }

            result.FromWireFormat(reader, payloadLength, this);
            return result;
        }

        /// <summary>
        /// Execute the given query text on the server and return the result.
        /// 
        /// Will throw if an error occurs or an unexpected packet is sent.
        /// Will return null if an OK response is sent.
        /// </summary>
        /// <param name="queryText">The query to execute</param>
        /// <param name="behavior">The behavior required of the query</param>
        /// <returns></returns>
        public ResultSet ExecuteQuery(string queryText, CommandBehavior behavior)
        {
            if (behavior != CommandBehavior.Default)
                throw new NotImplementedException("Unimplemented command behavior " + behavior);

            _sequenceNumber = 0;
            COM_QUERY packet = new COM_QUERY();
            packet.QueryText = queryText;
            SendPacket(packet);

            ResultSet result = new ResultSet();
            IServerPacket response = ReceivePacket(result);
            if (response is OK)
                return null;
            else if (response is ERR)
                throw new ProtocolException((ERR)response);
            return (ResultSet)response;
        }

        #region IDisposable
        public void Dispose()
        {
            if (_lineDriver != null)
            {
                SendPacket(new COM_QUIT());
                _lineDriver.Dispose();
                _lineDriver = null;
            }
        }
        #endregion
    }
}

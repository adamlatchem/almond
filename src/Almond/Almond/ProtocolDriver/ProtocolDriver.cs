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
using Almond.MySQLDriver;
using System;
using System.IO;

namespace Almond.ProtocolDriver
{
    /// <summary>
    /// Defines packets and state machine for the MySQL Client/Server protocol.
    /// </summary>
    internal class ProtocolDriver : IPacketFactory, IDisposable
    {
        #region Members
        /// <summary>
        /// The line driver used to communicate with the server.
        /// </summary>
        private TCPLineDriver _lineDriver;

        /// <summary>
        /// The expected sequence Id of the next packet.
        /// </summary>
        private byte _expectedSequenceNumber;
        #endregion

        /// <summary>
        /// Constructor - connects to the given connection string using Hostname and Port,
        /// initiates reading and returns synchronously.
        /// </summary>
        /// <param name="connectionStringBuilder"></param>
        public ProtocolDriver(ConnectionStringBuilder connectionStringBuilder)
        {
            _lineDriver = new TCPLineDriver(connectionStringBuilder);
            IPacket initialPacket = _lineDriver.ReadPacket(this);
            if (!(initialPacket is InitialHandshakePacket))
            {
                if (initialPacket is ERRPacket)
                    throw new ProtocolErrorException(((ERRPacket)initialPacket).ErrorMessage);
                throw new ProtocolErrorException("Missing Initial Handshake packet");
            }

            throw new NotImplementedException();
        }

        #region IPacketFactory
        public IPacket CreatePacket(BinaryReader packetHeader)
        {
            Int32 payloadLength = packetHeader.ReadMyInt3();
            byte sequenceNumber = packetHeader.ReadMyInt1();
            if (sequenceNumber != _expectedSequenceNumber)
                throw new ProtocolErrorException(
                    String.Format(
                        "Expected sequence number {0} but got {1}",
                        _expectedSequenceNumber, 
                        sequenceNumber));
            _expectedSequenceNumber++;

            IPacket result = null;
            switch (packetHeader.PeekByte())
            {
                case 9:
                case 10:
                    result = new InitialHandshakePacket();
                    break;
                case 255:
                    result = new ERRPacket();
                    break;
                default:
                    result = new GenericPacket();
                    break;
            }

            result.SequenceNumber = sequenceNumber;
            result.Length = payloadLength;
            return result;
        }
        #endregion

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

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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Almond.ProtocolDriver.Packets
{
    public class HandshakeResponse : IClientPacket
    {
        #region Members
        public static SHA1 sha = new SHA1CryptoServiceProvider();

        public Capability Capability
        {
            get; set;
        }

        public UInt32 MaxPacketSize
        {
            get; set;
        }

        public byte CharacterSet
        {
            get; set;
        }

        public string Username
        {
            get; set;
        }

        public byte[] AuthResponse
        {
            get; private set;
        }

        public string Database
        {
            get; set;
        }

        public string AuthPluginName
        {
            get; private set;
        }
        #endregion

        #region IPacketHeader
        public UInt32 Length
        {
            get; private set;
        }

        public byte SequenceNumber
        {
            get; private set;
        }
        #endregion

        #region IClientPacket
        public void ToWireFormat(ChunkWriter writer, ProtocolDriver driver)
        {
            Capability = driver.ClientCapability;

            writer.WriteMyInt4((UInt32)Capability);
            writer.WriteMyInt4(MaxPacketSize);
            writer.WriteMyInt1(CharacterSet);
            writer.Fill(23); // Reserved
            writer.WriteTextNull(Username, driver.ClientEncoding);
            if (Capability.HasFlag(Capability.CLIENT_PLUGIN_AUTH_LENENC_CLIENT_DATA))
            {
                // lenenc - int     length of auth - response
                // string[n]        auth-response
                throw new NotImplementedException();
            }
            else if (Capability.HasFlag(Capability.CLIENT_SECURE_CONNECTION))
            {
                writer.WriteMyInt1((byte)AuthResponse.Length);
                writer.WriteMyStringFix(AuthResponse, (UInt32)AuthResponse.Length);
            }
            else
            {
                writer.WriteMyStringNull(AuthResponse, (UInt32)AuthResponse.Length);
            }
            if (Capability.HasFlag(Capability.CLIENT_CONNECT_WITH_DB))
            {
                writer.WriteTextNull(Database, driver.ClientEncoding);
            }
            if (Capability.HasFlag(Capability.CLIENT_PLUGIN_AUTH))
            {
                writer.WriteTextNull(AuthPluginName, driver.ClientEncoding);
            }
            if (Capability.HasFlag(Capability.CLIENT_CONNECT_ATTRS))
            {
                // lenenc - int     length of all key-values
                // lenenc - str     key
                // lenenc - str     value
                // if-more data in 'length of all key-values', more keys and value pairs
                throw new NotImplementedException();
            }
        }
        #endregion

        /// <summary>
        /// Compute correct authentication response using mysql_native_password method.
        /// </summary>
        /// <param name="password">The users password for authentication</param>
        /// <param name="serverHandshakePacket">Initial handshake from server</param>
        /// <param name="clientEncoding">Text encoder</param>
        public void MySQLNativePassword(string password, ServerHandshake serverHandshakePacket, Encoding clientEncoding)
        {
            if (!serverHandshakePacket.AuthPluginName.Equals("mysql_native_password"))
                throw new ProtocolException("Only mysql_native_password authentication is supported at this time");

            List<byte> data = serverHandshakePacket.AuthPluginData;

            byte[] shaPassword = sha.ComputeHash(ChunkWriter.StringToBytes(password, clientEncoding));

            byte[] shaSquaredPassword = sha.ComputeHash(shaPassword);

            data.RemoveAt(20);
            Debug.Assert(data.Count == 20);
            data.AddRange(shaSquaredPassword);
            AuthResponse = sha.ComputeHash(data.ToArray());

            Debug.Assert(AuthResponse.Length == 20);
            for (int i = 0; i < 20; i++)
                AuthResponse[i] ^= shaPassword[i];

            AuthPluginName = serverHandshakePacket.AuthPluginName;
        }
    }
}

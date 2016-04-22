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
using System.Text;

namespace Almond.ProtocolDriver.Packets
{
    /// <summary>
    /// Read packet sent by send_server_handshake_packet.
    /// </summary>
    public class ServerHandshake : IServerPacket
    {
        #region members
        public int ProtocolVersion
        {
            get; set;
        }

        public string ServerVersion
        {
            get; set;
        }

        public UInt32 ConnectionThreadId
        {
            get; set;
        }

        public List<byte> AuthPluginData
        {
            get; set;
        }

        public Capability Capabilities
        {
            get; set;
        }

        public byte CharacterSet
        {
            get; set;
        }

        public Status StatusFlags
        {
            get; set;
        }

        public byte LengthOfAuthPluginData
        {
            get; set;
        }

        public string AuthPluginName
        {
            get; set;
        }
        #endregion

        private void AddPluginData(ArraySegment<byte> data)
        {
            if (AuthPluginData == null)
                AuthPluginData = new List<byte>();
            for (int i = data.Offset; i < data.Offset + data.Count; i++)
                AuthPluginData.Add(data.Array[i]);
        }

        #region IServerPacket
        public IServerPacket FromWireFormat(ChunkReader chunkReader, UInt32 payloadLength, ProtocolDriver driver)
        {
            UInt32 headerLength = chunkReader.ReadSoFar();

            ProtocolVersion = chunkReader.ReadMyInt1();
            if (ProtocolVersion == 10)
            {
                ServerVersion = chunkReader.ReadTextNull(System.Text.Encoding.UTF8);
                ConnectionThreadId = chunkReader.ReadMyInt4();
                AddPluginData(chunkReader.ReadMyStringNull());
                Debug.Assert(AuthPluginData.Count == 8);
                Capabilities = (Capability)chunkReader.ReadMyInt2();
                if (17 + ServerVersion.Length < payloadLength)
                {
                    CharacterSet = chunkReader.ReadMyInt1();
                    StatusFlags = (Status)chunkReader.ReadMyInt2();
                    Capabilities = (Capability)((UInt32)Capabilities | (((UInt32)chunkReader.ReadMyInt2()) << 16));
                    if (Capabilities.HasFlag(Capability.CLIENT_PLUGIN_AUTH))
                        LengthOfAuthPluginData = chunkReader.ReadMyInt1();
                    else
                        chunkReader.Skip(1);
                    chunkReader.Skip(10);

                    if (Capabilities.HasFlag(Capability.CLIENT_SECURE_CONNECTION))
                    {
                        List<byte> pluginData = new List<byte>(AuthPluginData);
                        ArraySegment<byte> data = chunkReader.ReadMyStringFix((UInt32)Math.Max(13, LengthOfAuthPluginData - 8));
                        AddPluginData(data);
                    }
                    if (Capabilities.HasFlag(Capability.CLIENT_PLUGIN_AUTH))
                        AuthPluginName = chunkReader.ReadTextNull(System.Text.Encoding.UTF8);
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            Debug.Assert(chunkReader.ReadSoFar() == headerLength + payloadLength);
            return this;
        }
        #endregion
    }
}

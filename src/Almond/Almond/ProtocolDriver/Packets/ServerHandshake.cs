﻿#region License
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

        public byte[] AuthPluginData
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

        #region IServerPacket
        public IServerPacket FromWireFormat(ChunkReader reader, UInt32 payloadLength, ProtocolDriver driver)
        {
            ProtocolVersion = reader.ReadMyInt1();
            if (ProtocolVersion == 10)
            {
                ServerVersion = reader.ReadTextNull(System.Text.Encoding.UTF8);
                ConnectionThreadId = reader.ReadMyInt4();
                AuthPluginData = reader.ReadMyStringNull();
                Debug.Assert(AuthPluginData.Length == 8);
                Capabilities = (Capability)reader.ReadMyInt2();
                if (17 + ServerVersion.Length < payloadLength)
                {
                    CharacterSet = reader.ReadMyInt1();
                    StatusFlags = (Status)reader.ReadMyInt2();
                    Capabilities = (Capability)((UInt32)Capabilities | (((UInt32)reader.ReadMyInt2()) << 16));
                    if (Capabilities.HasFlag(Capability.CLIENT_PLUGIN_AUTH))
                        LengthOfAuthPluginData = reader.ReadMyInt1();
                    else
                        reader.Skip(1);
                    reader.Skip(10);

                    if (Capabilities.HasFlag(Capability.CLIENT_SECURE_CONNECTION))
                    {
                        List<byte> pluginData = new List<byte>(AuthPluginData);
                        pluginData.AddRange(reader.ReadMyStringFix((UInt32)Math.Max(13, LengthOfAuthPluginData - 8)));
                        AuthPluginData = pluginData.ToArray();
                    }
                    if (Capabilities.HasFlag(Capability.CLIENT_PLUGIN_AUTH))
                        AuthPluginName = reader.ReadTextNull(System.Text.Encoding.UTF8);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            return this;
        }
        #endregion
    }
}

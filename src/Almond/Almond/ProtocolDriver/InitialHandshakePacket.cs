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
using System.IO;

namespace Almond.ProtocolDriver
{
    public class InitialHandshakePacket : IServerPacket
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

        public UInt32 ConnectionId
        {
            get; set;
        }

        public string AuthPluginDataPart1
        {
            get; set;
        }

        public string AuthPluginDataPart2
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

        public UInt32 StatusFlags
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
        public UInt32 Length
        {
            get; set;
        }

        public byte SequenceNumber
        {
            get; set;
        }

        public void FromReader(ChunkReader reader, Capability capabilities)
        {
            ProtocolVersion = reader.ReadMyInt1();
            if (ProtocolVersion == 10)
            {
                ServerVersion = reader.ReadStringNull();
                ConnectionId = reader.ReadMyInt4();
                AuthPluginDataPart1 = reader.ReadStringFix(8);
                reader.Skip(1);
                Capabilities = (Capability)reader.ReadMyInt2();
                if (17 + ServerVersion.Length < Length)
                {
                    CharacterSet = reader.ReadMyInt1();
                    StatusFlags = reader.ReadMyInt2();
                    Capabilities = (Capability)((UInt32)Capabilities | (reader.ReadMyInt2() << 16));
                    if (Capabilities.HasFlag(Capability.CLIENT_PLUGIN_AUTH))
                        LengthOfAuthPluginData = reader.ReadMyInt1();
                    else
                        reader.Skip(1);
                    reader.Skip(10);

                    if (Capabilities.HasFlag(Capability.CLIENT_SECURE_CONNECTION))
                        AuthPluginDataPart2 = reader.ReadStringFix((byte)Math.Max(13, LengthOfAuthPluginData - 8));
                    if (Capabilities.HasFlag(Capability.CLIENT_PLUGIN_AUTH))
                        AuthPluginName = reader.ReadStringNull();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}

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
using System.Diagnostics;
using System.Text;

namespace Almond.ProtocolDriver.Packets
{
    public class EOF : IServerPacket
    {
        #region Members
        public UInt32 NumberOfWarnings
        {
            get; set;
        }

        public Status StatusFlags
        {
            get; set;
        }
        #endregion

        #region IServerPacket
        public void FromReader(ChunkReader reader, UInt32 payloadLength, Capability clientCapability, Encoding clientEncoding)
        {
            byte header = reader.ReadMyInt1();
            Debug.Assert(header == 0xFE);

            if (clientCapability.HasFlag(Capability.CLIENT_PROTOCOL_41))
            {
                NumberOfWarnings = reader.ReadMyInt2();
                StatusFlags = (Status)reader.ReadMyInt2();
            }
        }
        #endregion
    }
}

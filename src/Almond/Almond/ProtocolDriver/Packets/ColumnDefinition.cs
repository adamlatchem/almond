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
using System.Text;

namespace Almond.ProtocolDriver.Packets
{
    /// <summary>
    /// Column definition sent as part of a result set
    /// </summary>
    public class ColumnDefinition : IServerPacket, IClientPacket
    {
        #region Members
        public UInt32 PayloadLength
        {
            get; set;
        }

        public byte[] Payload
        {
            get; set;
        }

        #region Debug helpers
        public Encoding ClientEncoding
        {
            get; set;
        }

        public string PayloadAsString
        {
            get
            {
                return ChunkReader.BytesToString(Payload, ClientEncoding);
            }
        }
        #endregion
        #endregion

        #region IServerPacket
        public IServerPacket FromWireFormat(ChunkReader reader, UInt32 payloadLength, ProtocolDriver driver)
        {
            PayloadLength = payloadLength;
            ClientEncoding = driver.ClientEncoding;
            Payload = reader.ReadMyStringFix(payloadLength);
            return this;
        }
        #endregion

        #region IClientPacket
        public void ToWireFormat(ChunkWriter writer, ProtocolDriver driver)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

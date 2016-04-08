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
    public class ERRPacket : IServerPacket
    {
        #region Members
        public UInt32 ErrorCode
        {
            get; set;
        }

        public string ErrorMessage
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
            byte header = reader.ReadMyInt1();
            ErrorCode = reader.ReadMyInt2();
            UInt32 stringLength = Length - 3;
            if (capabilities.HasFlag(Capability.CLIENT_PROTOCOL_41))
            {
                throw new NotImplementedException();
            }
            ErrorMessage = reader.ReadStringFix(stringLength);
        }
        #endregion
    }
}

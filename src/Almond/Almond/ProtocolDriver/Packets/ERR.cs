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
    public class ERR : IServerPacket
    {
        #region Members
        public UInt32 ErrorCode
        {
            get; set;
        }

        public string SQLState
        {
            get; set;
        }

        public string ErrorMessage
        {
            get; set;
        }
        #endregion

        #region IServerPacket
        public void FromReader(ChunkReader reader, UInt32 payloadLength, Capability clientCapability, Encoding clientEncoding)
        {
            byte header = reader.ReadMyInt1();
            ErrorCode = reader.ReadMyInt2();
            UInt32 stringLength = payloadLength - 3;
            if (clientCapability.HasFlag(Capability.CLIENT_PROTOCOL_41) && reader.PeekByte() == '#')
            {
                byte hashMark = reader.ReadMyInt1();
                Debug.Assert(hashMark == '#');
                SQLState = reader.ReadTextFix(5, clientEncoding);
                stringLength -= 6;
            }
            ErrorMessage = reader.ReadTextFix(stringLength, clientEncoding);
        }
        #endregion

        public override string ToString()
        {
            StringBuilder result = new StringBuilder("Server Error ");
            result.Append(ErrorCode);
            if (!String.IsNullOrEmpty(ErrorMessage))
                result.Append(" : " + ErrorMessage);
            if (!String.IsNullOrEmpty(SQLState))
                result.Append(" # " + SQLState);
            return result.ToString();
        }
    }
}

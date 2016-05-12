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

namespace Almond.ProtocolDriver.Packets
{
    public class COM_STMT_EXECUTE : IClientPacket
    {
        #region Members
        public UInt32 StatementId
        {
            get; set;
        }

        public CursorFlags Flags
        {
            get; set;
        }

        public UInt32 IterationCount
        {
            get; set;
        }
        #endregion

        public void ToWireFormat(ChunkWriter writer, ProtocolDriver driver)
        {
            writer.WriteMyInt1(17);
            writer.WriteMyInt4(StatementId);
            writer.WriteMyInt1((byte)Flags);
            writer.WriteMyInt4(IterationCount);

            throw new NotImplementedException();
        }
    }
}

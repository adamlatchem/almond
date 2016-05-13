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
        public CursorFlags Flags
        {
            get; set;
        }

        public COM_STMT_PREPARE_OK PrepareOK
        {
            get; set;
        }
        #endregion

        public void ToWireFormat(ChunkWriter writer, ProtocolDriver driver)
        {
            if (PrepareOK == null)
                throw new ProtocolException("A COM_STMT_PREPARE_OK is required");

            writer.WriteMyInt1(17);
            writer.WriteMyInt4(PrepareOK.StatementId);
            writer.WriteMyInt1((byte)Flags);
            writer.WriteMyInt4(1 /* Iteration Count */);

            byte newParamsBoundFlag = 0;
            if (PrepareOK.NumberParameters > 0)
            {
                int nullBitmapLength = (newParamsBoundFlag + 7) / 8;
                // NULL BITMAP
                throw new NotImplementedException();
                writer.WriteMyInt1(newParamsBoundFlag);
            }
            if (newParamsBoundFlag == 1)
            {
                /*
                n  type of each parameter, length: num-params * 2
                n  value of each parameter
                */
                throw new NotImplementedException();
            }
        }
    }
}

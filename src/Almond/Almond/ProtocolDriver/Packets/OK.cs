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
    public class OK : IServerPacket
    {
        #region Members
        public byte Header
        {
            get; set;
        }

        public UInt64 AffectedRows
        {
            get; set;
        }

        public UInt64 LastInsertId
        {
            get; set;
        }

        public Status StatusFlags
        {
            get; set;
        }

        public UInt32 NumberOfWarnings
        {
            get; set;
        }

        public string Info
        {
            get; set;
        }

        public string SessionStateChanges
        {
            get; set;
        }
        #endregion

        #region IServerPacket
        public IServerPacket FromWireFormat(ChunkReader chunkReader, UInt32 payloadLength, ProtocolDriver driver)
        {
            UInt32 headerLength = chunkReader.ReadSoFar();

            Header = chunkReader.ReadMyInt1();
            Debug.Assert(Header == 0);
            AffectedRows = chunkReader.ReadMyIntLenEnc();
            LastInsertId = chunkReader.ReadMyIntLenEnc();

            if (driver.ClientCapability.HasFlag(Capability.CLIENT_PROTOCOL_41))
            {
                StatusFlags = (Status)chunkReader.ReadMyInt2();
                NumberOfWarnings = chunkReader.ReadMyInt2();
            }
            else if (driver.ClientCapability.HasFlag(Capability.CLIENT_TRANSACTIONS))
            {
                StatusFlags = (Status)chunkReader.ReadMyInt2();
            }

            if (driver.ClientCapability.HasFlag(Capability.CLIENT_SESSION_TRACK))
            {
                Info = chunkReader.ReadTextLenEnc(driver.ClientEncoding);
                if (StatusFlags.HasFlag(Status.SERVER_SESSION_STATE_CHANGED))
                {
                    SessionStateChanges = chunkReader.ReadTextLenEnc(driver.ClientEncoding);
                }
            }
            else {
                Info = chunkReader.ReadTextEOF(payloadLength + headerLength, driver.ClientEncoding);
            }

            Debug.Assert(chunkReader.ReadSoFar() ==  headerLength + payloadLength);
            return this;
        }
        #endregion
    }
}

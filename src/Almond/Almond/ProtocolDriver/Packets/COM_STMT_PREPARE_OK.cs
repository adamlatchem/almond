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
    public class COM_STMT_PREPARE_OK : IServerPacket
    {
        #region Members
        public byte Status
        {
            get; private set;
        }

        public UInt32 StatementId
        {
            get; private set;
        }

        public UInt16 NumberColumns
        {
            get; private set;
        }

        public UInt16 NumberParameters
        {
            get; private set;
        }

        public UInt32 WarningCount
        {
            get; private set;
        }

        public IList<ColumnDefinition> ParmeterDefinitions
        {
            get; private set;
        }

        public IList<ColumnDefinition> ColumnDefinitions
        {
            get; private set;
        }
        #endregion

        #region IServerPacket
        public IServerPacket FromWireFormat(ChunkReader chunkReader, UInt32 payloadLength, ProtocolDriver driver)
        {
            byte header = chunkReader.PeekByte();
            if (header == 0xFF)
                return (new ERR()).FromWireFormat(chunkReader, payloadLength, driver);

            UInt32 headerLength = chunkReader.ReadSoFar();

            Status = chunkReader.ReadMyInt1();
            Debug.Assert(Status == 0);
            StatementId = chunkReader.ReadMyInt4();
            NumberColumns = chunkReader.ReadMyInt2();
            NumberParameters = chunkReader.ReadMyInt2();
            byte reserverd = chunkReader.ReadByte();
            Debug.Assert(reserverd == 0);
            WarningCount = chunkReader.ReadMyInt2();

            if (NumberParameters > 0)
            {
                ParmeterDefinitions = new List<ColumnDefinition>();
                for (int i = 0; i < NumberParameters; i++)
                {
                    ColumnDefinition parameterDefinition = new ColumnDefinition();
                    IServerPacket parameter = driver.ReceivePacket(parameterDefinition);
                    driver.Expect<ColumnDefinition>(parameter);
                    Debug.Assert(parameterDefinition == parameter);
                    ParmeterDefinitions.Add(parameterDefinition);
                }
            }

            if (NumberColumns > 0)
            {
                ColumnDefinitions = new List<ColumnDefinition>();
                for (int i = 0; i < NumberColumns; i++)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    IServerPacket column = driver.ReceivePacket(columnDefinition);
                    driver.Expect<ColumnDefinition>(column);
                    Debug.Assert(columnDefinition == column);
                    ColumnDefinitions.Add(columnDefinition);
                }
            }

            return this;
        }
        #endregion
    }
}

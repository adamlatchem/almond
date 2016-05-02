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

namespace Almond.ProtocolDriver.Packets
{
    /// <summary>
    /// A meta packet to read results sets from the chunk stream.
    /// </summary>
    public class ResultSet : IServerPacket
    {
        #region Members
        /// <summary>
        /// The columns defining the result set
        /// </summary>
        public IList<ColumnDefinition> Columns
        {
            get; set;
        }

        /// <summary>
        /// The rows of the result set
        /// </summary>
        public IList<Row> Rows
        {
            get; set;
        }

        /// <summary>
        /// Return the tablename of the first column
        /// </summary>
        public string Tablename
        {
            get
            {
                if (Columns.Count < 1)
                    throw new ProtocolException("There are no columns in the resultset");
                return Columns[0].Definition.Table;
            }
        }

        /// <summary>
        /// Return the Schema of the first column
        /// </summary>
        public string Tablespace
        {
            get
            {
                if (Columns.Count < 1)
                    throw new ProtocolException("There are no columns in the resultset");
                return Columns[0].Definition.Schema;
            }
        }
        #endregion

        /// <summary>
        /// Deafult Ctor
        /// </summary>
        public ResultSet()
        {
            // NOP
        }

        /// <summary>
        /// Ctor to create an empty result set
        /// </summary>
        /// <param name="empty"></param>
        public ResultSet(bool empty)
        {
            Columns = new List<ColumnDefinition>();
            Rows = new List<Row>();
        }

        #region IServerPacket
        public IServerPacket FromWireFormat(ChunkReader chunkReader, UInt32 payloadLength, ProtocolDriver driver)
        {
            UInt32 headerLength = chunkReader.ReadSoFar();

            byte header = chunkReader.PeekByte();
            if (header == 0)
                return (new OK()).FromWireFormat(chunkReader, payloadLength, driver);
            else if (header == 0xFF)
                return (new ERR()).FromWireFormat(chunkReader, payloadLength, driver);
            else if (header == 0xFB)
                throw new NotImplementedException("INFILE Packet not implemented");

            UInt64 columns = chunkReader.ReadMyIntLenEnc();

            Debug.Assert(chunkReader.ReadSoFar() == headerLength + payloadLength);

            Columns = new List<ColumnDefinition>();
            for (UInt64 i = 0; i < columns; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                IServerPacket column = driver.ReceivePacket(columnDefinition);
                Debug.Assert(columnDefinition == column);
                Columns.Add(columnDefinition);
            }

            bool deprecateEOF = driver.ClientCapability.HasFlag(Capability.CLIENT_DEPRECATE_EOF);
            if (!deprecateEOF)
            {
                IServerPacket response = driver.ReceivePacket(driver);
                driver.Expect<EOF>(response);
            }

            Rows = new List<Row>();
            while (true)
            {
                Row rowPacket = new Row();
                IServerPacket row = driver.ReceivePacket(rowPacket);

                if (row is OK)
                {
                    OK ok = (OK)row;
                    if (ok.StatusFlags.HasFlag(Status.SERVER_MORE_RESULTS_EXISTS))
                        throw new NotImplementedException("Multi-resultset not implemented");
                    break;
                }
                else if (row is EOF)
                {
                    EOF eof = (EOF)row;
                    if (eof.StatusFlags.HasFlag(Status.SERVER_MORE_RESULTS_EXISTS))
                        throw new NotImplementedException("Multi-resultset not implemented");
                    break;
                }
                else if (row is ERR)
                {
                    ERR err = (ERR)row;
                    throw new ProtocolException(err);
                }

                Debug.Assert(row == rowPacket);
                Rows.Add(rowPacket);
            }

            return this;
        }
        #endregion
    }
}

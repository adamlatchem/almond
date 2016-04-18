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
using System.Text;

namespace Almond.ProtocolDriver.Packets
{
    /// <summary>
    /// Represents a row paket in the result set sent by the server.
    /// </summary>
    public class Row : IServerPacket
    {
        #region Members
        private Lazy<RowReader> _reader;
        private RowReader Reader
        {
            get { return _reader.Value; }
        }

        public IList<object> Values
        {
            get
            {
                return Reader.Values;
            }
        }
        #endregion

        #region IServerPacket
        public IServerPacket FromWireFormat(ChunkReader reader, UInt32 payloadLength, ProtocolDriver driver)
        {
            byte header = reader.PeekByte();
            if (header == 0)
                return (new OK()).FromWireFormat(reader, payloadLength, driver);
            else if (header == 0xFF)
                return (new ERR()).FromWireFormat(reader, payloadLength, driver);
            else if (header == 0xFE)
                return (new EOF()).FromWireFormat(reader, payloadLength, driver);

            byte[] payload = reader.ReadMyStringFix(payloadLength);

            _reader = new Lazy<RowReader>(() => new RowReader(payload, driver.ClientEncoding));
            return this;
        }
        #endregion

        /// <summary>
        /// Nested class to convert buffered data to CLR objects.
        /// </summary>
        public class RowReader
        {
            public bool IsNull
            {
                get
                {
                    return Values == null;
                }
            }

            public IList<object> Values
            {
                get; set;
            }

            public RowReader(byte[] rowData, Encoding encoding)
            {
                ChunkReader reader = new ChunkReader(rowData);

                // Null row
                if (reader.PeekByte() == 0xFB)
                    return;

                Values = new List<object>();
                while (reader.ReadSoFar() != rowData.Length)
                {
                    string data = reader.ReadTextLenEnc(encoding);
                    Values.Add(data);
                }
            }
        }
    }
}

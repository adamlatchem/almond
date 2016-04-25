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
            get
            {
                if (_reader != null)
                    return _reader.Value;
                throw new ProtocolException("No row data");
            }
        }

        public IList<ArraySegment<byte>> Values
        {
            get
            {
                return Reader.Values;
            }
        }

        private Encoding Encoding
        {
            get; set;
        }

        internal static readonly ArraySegment<byte> NULL = new ArraySegment<byte>(new byte[0], 0, 0);
        #endregion

        /// <summary>
        /// Decode given column to a string value or null
        /// </summary>
        /// <param name="i"></param>
        /// <param name="encodingOverride"></param>
        /// <returns></returns>
        public string StringValue(int i, Encoding encodingOverride)
        {
            ArraySegment<byte> value = Values[i];
            if (value == NULL)
                return null;
            return ChunkReader.BytesToString(value, encodingOverride ?? Encoding);
        }

        #region IServerPacket
        public IServerPacket FromWireFormat(ChunkReader reader, UInt32 payloadLength, ProtocolDriver driver)
        {
            Encoding = driver.ClientEncoding;
            byte header = reader.PeekByte();
            if (header == 0)
                return (new OK()).FromWireFormat(reader, payloadLength, driver);
            else if (header == 0xFF)
                return (new ERR()).FromWireFormat(reader, payloadLength, driver);
            else if (header == 0xFE &&
                (payloadLength == 7 ||
                (payloadLength == 5 && !driver.ClientCapability.HasFlag(Capability.CLIENT_PROTOCOL_41))))
                return (new EOF()).FromWireFormat(reader, payloadLength, driver);

            ArraySegment<byte> payload = reader.ReadMyStringFix(payloadLength);

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

            public Encoding Encoding
            {
                get; private set;
            }

            public IList<ArraySegment<byte>> Values
            {
                get; set;
            }

            public RowReader(ArraySegment<byte> rowData, Encoding encoding)
            {
                Encoding = encoding;
                ChunkReader reader = new ChunkReader(rowData);

                // Null row
                if (reader.PeekByte() == 0xFB)
                    return;

                Values = new List<ArraySegment<byte>>();
                ArraySegment<byte> data;
                while (reader.ReadSoFar() != rowData.Count)
                {
                    byte peek = reader.PeekByte();
                    if (peek == 0xFB)
                    {
                        data = NULL;
                        reader.ReadByte();
                    }
                    else
                    {
                        data = reader.ReadMyStringLenEnc();
                    }

                    Values.Add(data);
                }
            }
        }
    }
}

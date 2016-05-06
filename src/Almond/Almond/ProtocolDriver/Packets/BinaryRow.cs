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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Almond.ProtocolDriver.Packets
{
    /// <summary>
    /// Represents a binary row paket in the result set sent by the server.
    /// The binary format is used in server responses for prepared
    /// statements.
    /// </summary>
    public class BinaryRow : IServerPacket, IRow
    {
        #region Members
        private Lazy<BinaryRowReader> _reader;
        private BinaryRowReader Reader
        {
            get
            {
                if (_reader != null)
                    return _reader.Value;
                throw new ProtocolException("No row data");
            }
        }

        private Encoding Encoding
        {
            get; set;
        }

        private BitArray NullMap
        {
            get; set;
        }
        #endregion

        #region IRow
        public ArraySegment<byte> Value(int i, Encoding encodingOverride, IList<ColumnDefinition> Columns)
        {
            return Reader.Values[i];
        }

        public string StringValue(int i, Encoding encodingOverride, IList<ColumnDefinition> Columns)
        {
            ArraySegment<byte> value = Value(i, encodingOverride, Columns);
            if (value == LineDriver.ChunkReader.NULL)
                return null;
            return ChunkReader.BytesToString(value, encodingOverride ?? Encoding);
        }
        #endregion

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

            _reader = new Lazy<BinaryRowReader>(() => new BinaryRowReader(payload));
            return this;
        }
        #endregion

        /// <summary>
        /// Nested class to convert buffered data to CLR objects.
        /// </summary>
        public class BinaryRowReader
        {
            public IList<ArraySegment<byte>> Values
            {
                get; set;
            }

            public BinaryRowReader(ArraySegment<byte> rowData)
            {
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
                        data = LineDriver.ChunkReader.NULL;
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

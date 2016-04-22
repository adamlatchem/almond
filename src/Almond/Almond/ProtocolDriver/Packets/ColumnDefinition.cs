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
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Almond.ProtocolDriver.Packets
{
    /// <summary>
    /// Column definition sent as part of a result set
    /// </summary>
    public class ColumnDefinition : IServerPacket
    {
        #region Members
        private Lazy<ColumnDefinitionReader> _definition;
        public ColumnDefinitionReader Definition
        {
            get { return _definition.Value; }
        }
        #endregion

        #region IServerPacket
        public IServerPacket FromWireFormat(ChunkReader chunkReader, UInt32 payloadLength, ProtocolDriver driver)
        {
            UInt32 headerLength = chunkReader.ReadSoFar();

            ArraySegment<byte> payload = chunkReader.ReadMyStringFix(payloadLength);

            _definition = new Lazy<ColumnDefinitionReader>(
                () => new ColumnDefinitionReader(payload, driver.ClientEncoding, false));

            Debug.Assert(chunkReader.ReadSoFar() == headerLength + payloadLength);
            return this;
        }
        #endregion

        /// <summary>
        /// Nested class to extract dotNet objects from payload if required
        /// </summary>
        public class ColumnDefinitionReader
        {
            #region Members
            public string Catalog
            {
                get; set;
            }

            public string Schema
            {
                get; set;
            }

            public string Table
            {
                get; set;
            }

            public string OrgTable
            {
                get; set;
            }

            public string Name
            {
                get; set;
            }

            public string OrgName
            {
                get; set;
            }

            public UInt64 FieldDataLength
            {
                get; set;
            }

            public UInt32 CharacterSet
            {
                get; set;
            }

            public UInt64 ColumnLength
            {
                get; set;
            }

            public ColumnType Type
            {
                get; set;
            }

            public Flags Flags
            {
                get; set;
            }

            public byte Decimals
            {
                get; set;
            }

            public string Default
            {
                get; set;
            }
            #endregion Members

            #region Properties
            public int Precision
            {
                get
                {
                    // See chapter 11 of the MySQL reference manual
                    if (!Flags.HasFlag(Flags.NUM_FLAG))
                        return 255;

                    bool unsigned = Flags.HasFlag(Flags.UNSIGNED_FLAG);

                    switch (Type)
                    {
                        case ColumnType.MYSQL_TYPE_BIT:
                        case ColumnType.MYSQL_TYPE_FLOAT:
                        case ColumnType.MYSQL_TYPE_DOUBLE:
                        case ColumnType.MYSQL_TYPE_DECIMAL:
                        case ColumnType.MYSQL_TYPE_NEWDECIMAL:
                            return (int)ColumnLength;
                        case ColumnType.MYSQL_TYPE_TINY:
                            return 3;
                        case ColumnType.MYSQL_TYPE_SHORT:
                            return 5;
                        case ColumnType.MYSQL_TYPE_INT24:
                            return unsigned ? 8 : 7;
                        case ColumnType.MYSQL_TYPE_LONG:
                            return 10;
                        case ColumnType.MYSQL_TYPE_LONGLONG:
                            return unsigned ? 20 : 19;
                    }
                    throw new ProtocolException("Unkown precision for column " + Name ?? "<unknown>");
                }
            }

            public int Scale
            {
                get
                {
                    if (Decimals > 0)
                        return Decimals;
                    return 255;
                }
            }

            public Type CLRType
            {
                get
                {
                    bool unsigned = Flags.HasFlag(Flags.UNSIGNED_FLAG);

                    switch (Type)
                    {
                        case ColumnType.MYSQL_TYPE_TINY:
                            return unsigned ? typeof(byte) : typeof(sbyte);
                        case ColumnType.MYSQL_TYPE_SHORT:
                            return unsigned ? typeof(UInt16) : typeof(Int16);
                        case ColumnType.MYSQL_TYPE_INT24:
                        case ColumnType.MYSQL_TYPE_LONG:
                            return unsigned ? typeof(UInt32) : typeof(Int32);
                        case ColumnType.MYSQL_TYPE_FLOAT:
                            return typeof(float);
                        case ColumnType.MYSQL_TYPE_DOUBLE:
                            return typeof(double);
                        case ColumnType.MYSQL_TYPE_NULL:
                            return typeof(DBNull);
                        case ColumnType.MYSQL_TYPE_LONGLONG:
                            return unsigned ? typeof(ulong) : typeof(long);
                        case ColumnType.MYSQL_TYPE_YEAR:
                            return typeof(Int32);
                        case ColumnType.MYSQL_TYPE_BIT:
                            return typeof(byte);
                        case ColumnType.MYSQL_TYPE_TIMESTAMP:
                        case ColumnType.MYSQL_TYPE_DATE:
                        case ColumnType.MYSQL_TYPE_TIME:
                        case ColumnType.MYSQL_TYPE_DATETIME:
                        case ColumnType.MYSQL_TYPE_NEWDATE:
                        case ColumnType.MYSQL_TYPE_TIMESTAMP2:
                        case ColumnType.MYSQL_TYPE_DATETIME2:
                        case ColumnType.MYSQL_TYPE_TIME2:
                            return typeof(DateTime);
                        case ColumnType.MYSQL_TYPE_DECIMAL:
                        case ColumnType.MYSQL_TYPE_NEWDECIMAL:
                            return typeof(decimal);
                        case ColumnType.MYSQL_TYPE_ENUM:
                            return typeof(UInt64);
                        case ColumnType.MYSQL_TYPE_VARCHAR:
                        case ColumnType.MYSQL_TYPE_VAR_STRING:
                        case ColumnType.MYSQL_TYPE_STRING:
                            return typeof(string);
                        case ColumnType.MYSQL_TYPE_SET:
                        case ColumnType.MYSQL_TYPE_TINY_BLOB:
                        case ColumnType.MYSQL_TYPE_MEDIUM_BLOB:
                        case ColumnType.MYSQL_TYPE_LONG_BLOB:
                        case ColumnType.MYSQL_TYPE_BLOB:
                        case ColumnType.MYSQL_TYPE_GEOMETRY:
                            return typeof(object);
                    }
                    throw new ProtocolException("Unknown data type " + Type);
                }
            }
            #endregion

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="columnDefinitionPacketPayload"></param>
            /// <param name="encoding"></param>
            /// <param name="FieldList"></param>
            public ColumnDefinitionReader(ArraySegment<byte> columnDefinitionPacketPayload, Encoding encoding, bool FieldList)
            {
                ChunkReader reader = new ChunkReader(columnDefinitionPacketPayload);

                Catalog = reader.ReadTextLenEnc(encoding);
                Schema = reader.ReadTextLenEnc(encoding);
                Table = reader.ReadTextLenEnc(encoding);
                OrgTable = reader.ReadTextLenEnc(encoding);
                Name = reader.ReadTextLenEnc(encoding);
                OrgName = reader.ReadTextLenEnc(encoding);
                FieldDataLength = reader.ReadMyIntLenEnc();
                Debug.Assert(FieldDataLength == 12);
                CharacterSet = reader.ReadMyInt2();
                ColumnLength = reader.ReadMyInt4();
                Type = (ColumnType)reader.ReadMyInt1();
                Flags = (Flags)reader.ReadMyInt2();
                Decimals = reader.ReadMyInt1();
                UInt32 filler = reader.ReadMyInt2();

                if (FieldList)
                    Default = reader.ReadTextLenEnc(encoding);
            }
        }
    }
}

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
using Almond.ProtocolDriver;
using Almond.ProtocolDriver.Packets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Almond.SQLDriver
{
    /// <summary>
    /// Provides a dataread to extract dotNet objects from the underlying
    /// resultsset returned from the server. The underlying rows might be
    /// Row or BinaryRow objects.
    /// </summary>
    public class DataReader<RowT> : IDataReader where RowT : IServerPacket, IRow, new()
    {
        #region Members
        private CommandBehavior Behaviour
        {
            get; set;
        }

        private IList<ResultSet<RowT>> _data;
        private IList<ResultSet<RowT>> Data
        {
            get
            {
                if (_data == null)
                    throw new ObjectDisposedException("DataReader has been Close()'d or Dispose()'d");
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// The index of dataset in Data property we are on
        /// </summary>
        private int _set;
        private int Set
        {
            get
            {
                return _set;
            }
            set
            {
                _set = value;
                _cachedSchemaTable = null;
            }
        }

        /// <summary>
        /// Used to cache the schema table of the current set
        /// </summary>
        private DataTable _cachedSchemaTable;
        private DataTable CachedSchema
        {
            get
            {
                if (_cachedSchemaTable == null)
                    _cachedSchemaTable = GetSchemaTable();
                return _cachedSchemaTable;
            }
        }

        /// <summary>
        /// The row in the current set we are on
        /// </summary>
        private int Row
        {
            get; set;
        }

        /// <summary>
        /// The connection that produced this dataset.
        /// </summary>
        private Connection Connection
        {
            get; set;
        }

        /// <summary>
        /// Used to represent a NULL result from the database with no rows or data.
        /// </summary>
        private readonly ResultSet<RowT> EMPTY_RESULTSET = new ResultSet<RowT>(/*empty*/true);
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultsSetPacket"></param>
        /// <param name="connection"></param>
        /// <param name="behaviour"></param>
        internal DataReader(ResultSet<RowT> resultsSetPacket, Connection connection, CommandBehavior behaviour)
        {
            if (connection == null)
                throw new SQLDriverException("Connection must not be null");
            if (!(connection is Connection))
                throw new SQLDriverException("Connection must be an instance of Connection");
            if (behaviour != CommandBehavior.Default && behaviour != CommandBehavior.CloseConnection)
                throw new SQLDriverException("Unsupported behaviour " + behaviour);

            Behaviour = behaviour;
            Data = new List<ResultSet<RowT>>();
            if (resultsSetPacket != null)
                Data.Add(resultsSetPacket);
            else
                Data.Add(EMPTY_RESULTSET);
            Connection = connection;
            Set = 0;
            Row = -1;
        }

        #region IDataReader
        public object this[string name]
        {
            get
            {
                return this[GetOrdinal(name)];
            }
        }

        public object this[int i]
        {
            get
            {
                return GetValue(i);
            }
        }

        public int Depth
        {
            get
            {
                return 0;
            }
        }

        public int FieldCount
        {
            get
            {
                return Data[Set].Columns.Count;
            }
        }

        public bool IsClosed
        {
            get
            {
                return _data == null;
            }
        }

        public int RecordsAffected
        {
            get
            {
                return Data[Set].Rows.Count;
            }
        }

        public void Close()
        {
            if (Behaviour == CommandBehavior.CloseConnection)
                Connection.Close();
            Dispose();
        }

        private ArraySegment<byte> RawValue(int i)
        {
            RowT row = Data[Set].Rows[Row];
            return row.Values[i];
        }

        /// <summary>
        /// Return a string value for the column or null.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private string StringValue(int i)
        {
            UInt32 charSet = Data[Set].Columns[i].Definition.CharacterSet;
            Encoding encoding = Mapping.CharSetToEncoding(charSet);
            return Data[Set].Rows[Row].StringValue(i, encoding, Data[Set].Columns);
        }

        public bool GetBoolean(int i)
        {
            ArraySegment<byte> value = RawValue(i);
            if (value.Count == 1)
                return value.Array[value.Offset] != 0;
            throw new SQLDriverException("Unable to interpret value as Boolean");
        }

        public byte GetByte(int i)
        {
            string value = StringValue(i);
            return byte.Parse(value);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            ArraySegment<byte> value = RawValue(i);

            // This is a way to query the length
            if (buffer == null)
                return value.Count;
            length = Math.Min(length, value.Count);
            Array.Copy(value.Array, value.Offset, buffer, bufferoffset, length);
            return length;
        }

        public char GetChar(int i)
        {
            string asString = GetString(i);
            return asString[0];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            string asString = GetString(i);
            int len = Math.Min(length, asString.Length);
            for (int j = 0; j < len; ++j)
                buffer[bufferoffset + j] = asString[(int)fieldoffset + j];
            return len;
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            return DateTime.Parse(StringValue(i));
        }

        public decimal GetDecimal(int i)
        {
            return decimal.Parse(StringValue(i));
        }

        public double GetDouble(int i)
        {
            return double.Parse(StringValue(i));
        }

        public float GetFloat(int i)
        {
            return float.Parse(StringValue(i));
        }

        public Guid GetGuid(int i)
        {
            return Guid.Parse(GetString(i));
        }

        public short GetInt16(int i)
        {
            return short.Parse(StringValue(i));
        }

        public int GetInt32(int i)
        {
            return int.Parse(StringValue(i));
        }

        public long GetInt64(int i)
        {
            return long.Parse(StringValue(i));
        }

        public string GetString(int i)
        {
            return StringValue(i);
        }

        public object GetValue(int i)
        {
            ColumnType Type = Data[Set].Columns[i].Definition.Type;
            bool unsigned = Data[Set].Columns[i].Definition.Flags.HasFlag(Flags.UNSIGNED_FLAG);

            if (RawValue(i) == LineDriver.ChunkReader.NULL)
                return DBNull.Value;

            switch (Type)
            {
                case ColumnType.MYSQL_TYPE_TINY:
                    return unsigned ? (object)byte.Parse(GetString(i)) : sbyte.Parse(GetString(i));
                case ColumnType.MYSQL_TYPE_SHORT:
                    return unsigned ? (object)UInt16.Parse(GetString(i)) : Int16.Parse(GetString(i));
                case ColumnType.MYSQL_TYPE_INT24:
                case ColumnType.MYSQL_TYPE_LONG:
                    return unsigned ? (object)UInt32.Parse(GetString(i)) : Int32.Parse(GetString(i));
                case ColumnType.MYSQL_TYPE_FLOAT:
                    return float.Parse(GetString(i));
                case ColumnType.MYSQL_TYPE_DOUBLE:
                    return double.Parse(GetString(i));
                case ColumnType.MYSQL_TYPE_NULL:
                    return DBNull.Value;
                case ColumnType.MYSQL_TYPE_LONGLONG:
                    return unsigned ? (object)ulong.Parse(GetString(i)) : (object)long.Parse(GetString(i));
                case ColumnType.MYSQL_TYPE_YEAR:
                    return Int32.Parse(GetString(i));
                case ColumnType.MYSQL_TYPE_BIT:
                    return GetString(i)[0] != '\u0000';
                case ColumnType.MYSQL_TYPE_TIMESTAMP:
                case ColumnType.MYSQL_TYPE_DATE:
                case ColumnType.MYSQL_TYPE_TIME:
                case ColumnType.MYSQL_TYPE_DATETIME:
                case ColumnType.MYSQL_TYPE_NEWDATE:
                case ColumnType.MYSQL_TYPE_TIMESTAMP2:
                case ColumnType.MYSQL_TYPE_DATETIME2:
                case ColumnType.MYSQL_TYPE_TIME2:
                    return DateTime.Parse(GetString(i));
                case ColumnType.MYSQL_TYPE_DECIMAL:
                case ColumnType.MYSQL_TYPE_NEWDECIMAL:
                    return decimal.Parse(GetString(i));
                case ColumnType.MYSQL_TYPE_ENUM:
                    return UInt64.Parse(GetString(i));
                case ColumnType.MYSQL_TYPE_VARCHAR:
                case ColumnType.MYSQL_TYPE_VAR_STRING:
                case ColumnType.MYSQL_TYPE_STRING:
                    return GetString(i);
                case ColumnType.MYSQL_TYPE_SET:
                case ColumnType.MYSQL_TYPE_TINY_BLOB:
                case ColumnType.MYSQL_TYPE_MEDIUM_BLOB:
                case ColumnType.MYSQL_TYPE_LONG_BLOB:
                case ColumnType.MYSQL_TYPE_BLOB:
                case ColumnType.MYSQL_TYPE_GEOMETRY:
                    return RawValue(i);
            }
            throw new SQLDriverException("Unable to convert column data");
        }

        public int GetValues(object[] values)
        {
            int length = Math.Min(CachedSchema.Rows.Count, values.Length);
            for (int i = 0; i < length; ++i)
                values[i] = GetValue(i);
            return length;
        }

        public bool IsDBNull(int i)
        {
            return GetValue(i) == DBNull.Value;
        }

        public string GetDataTypeName(int i)
        {
            return Data[Set].Columns[i].Definition.Type.ToString();
        }

        public Type GetFieldType(int i)
        {
            return Data[Set].Columns[i].Definition.CLRType;
        }

        public string GetName(int i)
        {
            return Data[Set].Columns[i].Definition.Name;
        }

        public int GetOrdinal(string name)
        {
            DataTable schema = CachedSchema;
            DataRow[] rows = schema.Select(string.Format(@"[ColumnName] = '{0}'", name));
            if (rows.Length != 1)
                throw new IndexOutOfRangeException("No such column " + name);
            return (int)rows[0]["ColumnOrdinal"];
        }

        public DataTable GetSchemaTable()
        {
            DataTable result = new DataTable(Data[Set].Tablename, Data[Set].Tablespace);

            result.Columns.Add("AllowDBNull", typeof(bool));
            result.Columns.Add("BaseCatalogName", typeof(string));
            result.Columns.Add("BaseColumnName", typeof(string));
            result.Columns.Add("BaseSchemaName", typeof(string));
            result.Columns.Add("BaseServerName", typeof(string));
            result.Columns.Add("BaseTableName", typeof(string));
            result.Columns.Add("ColumnName", typeof(string));
            result.Columns.Add("ColumnOrdinal", typeof(int));
            result.Columns.Add("ColumnSize", typeof(int));
            result.Columns.Add("DataTypeName", typeof(string));
            result.Columns.Add("IsAliased", typeof(bool));
            result.Columns.Add("IsAutoIncrement", typeof(bool));
            result.Columns.Add("IsColumnSet", typeof(bool));
            result.Columns.Add("IsHidden", typeof(bool));
            result.Columns.Add("IsKey", typeof(bool));
            result.Columns.Add("IsLong", typeof(bool));
            result.Columns.Add("IsRowVersion", typeof(bool));
            result.Columns.Add("IsUnique", typeof(bool));
            result.Columns.Add("NumericPrecision", typeof(byte));
            result.Columns.Add("NumericScale", typeof(byte));
            result.Columns.Add("ProviderSpecificDataType", typeof(ColumnType));
            result.Columns.Add("ProviderType", typeof(Type));

            ConnectionStringBuilder csb = new ConnectionStringBuilder();
            csb.ConnectionString = Connection.ConnectionString;
            int ordinal = 0;

            foreach (ColumnDefinition column in Data[Set].Columns)
            {
                ColumnDefinition.ColumnDefinitionReader definition = column.Definition;
                DataRow row = result.NewRow();

                row["AllowDBNull"] = !definition.Flags.HasFlag(Flags.NOT_NULL_FLAG);
                row["BaseCatalogName"] = definition.Catalog;
                row["BaseColumnName"] = definition.OrgName;
                row["BaseSchemaName"] = definition.Schema;
                row["BaseServerName"] = csb.Hostname;
                row["BaseTableName"] = definition.OrgTable;
                row["ColumnName"] = definition.Name;
                row["ColumnOrdinal"] = ordinal;
                row["ColumnSize"] = definition.ColumnLength;
                row["DataTypeName"] = definition.Type.ToString();
                row["IsAliased"] = !definition.OrgName.Equals(definition.Name);
                row["IsAutoIncrement"] = definition.Flags.HasFlag(Flags.AUTO_INCREMENT_FLAG);
                row["IsColumnSet"] = definition.Flags.HasFlag(Flags.SET_FLAG);
                row["IsHidden"] = false;
                row["IsKey"] = definition.Flags.HasFlag(Flags.PRI_KEY_FLAG);
                row["IsLong"] = definition.Flags.HasFlag(Flags.BLOB_FLAG);
                row["IsRowVersion"] = false;
                row["IsUnique"] = definition.Flags.HasFlag(Flags.UNIQUE_KEY_FLAG);
                row["NumericPrecision"] = definition.Precision;
                row["NumericScale"] = definition.Scale;
                row["ProviderSpecificDataType"] = definition.Type;
                row["ProviderType"] = definition.CLRType;

                result.Rows.Add(row);
                ordinal++;
            }
            return result;
        }

        public bool NextResult()
        {
            if (Set < Data.Count)
            {
                Set++;
                return Set < Data.Count;
            }
            return false;
        }

        public bool Read()
        {
            if (Row < Data[Set].Rows.Count)
            {
                Row++;
                return Row < Data[Set].Rows.Count;
            }
            return false;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Data = null;
            _cachedSchemaTable = null;
            Connection = null;
        }
        #endregion
    }
}

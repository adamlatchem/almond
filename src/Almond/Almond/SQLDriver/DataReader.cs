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

namespace Almond.SQLDriver
{
    /// <summary>
    /// Provides a dataread to extract dotNet objects from the underlying
    /// resultsset returned from the server.
    /// </summary>
    public class DataReader : IDataReader
    {
        #region Members
        private IList<ResultSet> _data;
        private IList<ResultSet> Data
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
        #endregion

        internal DataReader(ResultSet resultsSetPacket, IDbConnection connection)
        {
            if (resultsSetPacket == null)
                throw new ProtocolException("ResultSet must not be null");
            if (connection == null)
                throw new ProtocolException("Connection must not be null");
            if (!(connection is Connection))
                throw new ProtocolException("Connection must be an instance of Connection");

            Data = new List<ResultSet>() { resultsSetPacket };
            Connection = (Connection)connection;
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
            Dispose();
        }

        public bool GetBoolean(int i)
        {
            return (bool)Data[Set].Rows[Row].Values[i];
        }

        public byte GetByte(int i)
        {
            return (byte)Data[Set].Rows[Row].Values[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            byte[] value = (byte[])Data[Set].Rows[Row].Values[i];
            if (buffer == null)
                return value.Length;
            length = Math.Min(length, value.Length);
            Array.Copy(value, 0, buffer, bufferoffset, length);
            return length;
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)Data[Set].Rows[Row].Values[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)Data[Set].Rows[Row].Values[i];
        }

        public double GetDouble(int i)
        {
            return (double)Data[Set].Rows[Row].Values[i];
        }

        public float GetFloat(int i)
        {
            return (float)Data[Set].Rows[Row].Values[i];
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            return (Int16)Data[Set].Rows[Row].Values[i];
        }

        public int GetInt32(int i)
        {
            return (Int32)Data[Set].Rows[Row].Values[i];
        }

        public long GetInt64(int i)
        {
            return (Int64)Data[Set].Rows[Row].Values[i];
        }

        public string GetString(int i)
        {
            return (string)Data[Set].Rows[Row].Values[i];
        }

        public object GetValue(int i)
        {
            return Data[Set].Rows[Row].Values[i];
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
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
            result.Columns.Add("IsExpression", typeof(bool));
            result.Columns.Add("IsHidden", typeof(bool));
            result.Columns.Add("IsIdentity", typeof(bool));
            result.Columns.Add("IsKey", typeof(bool));
            result.Columns.Add("IsLong", typeof(bool));
            result.Columns.Add("IsReadOnly", typeof(bool));
            result.Columns.Add("IsRowVersion", typeof(bool));
            result.Columns.Add("IsUnique", typeof(bool));
            result.Columns.Add("NonVersionedProviderType", typeof(SqlDbType));
            result.Columns.Add("NumericPrecision", typeof(byte));
            result.Columns.Add("NumericScale", typeof(byte));
            result.Columns.Add("ProviderSpecificDataType", typeof(ColumnType));
            result.Columns.Add("ProviderType", typeof(Type));
            result.Columns.Add("UdtAssemblyQualifiedName", typeof(string));
            result.Columns.Add("XmlSchemaCollectionDatabase", typeof(string));
            result.Columns.Add("XmlSchemaCollectionName", typeof(string));
            result.Columns.Add("XmlSchemaCollectionOwningSchema", typeof(string));

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
                row["IsExpression"] = DBNull.Value;
                row["IsHidden"] = false;
                row["IsIdentity"] = DBNull.Value;
                row["IsKey"] = definition.Flags.HasFlag(Flags.PRI_KEY_FLAG);
                row["IsLong"] = definition.Flags.HasFlag(Flags.BLOB_FLAG);
                row["IsReadOnly"] = DBNull.Value;
                row["IsRowVersion"] = false;
                row["IsUnique"] = definition.Flags.HasFlag(Flags.UNIQUE_KEY_FLAG);
                row["NonVersionedProviderType"] = definition.SqlDbType;
                row["NumericPrecision"] = definition.Precision;
                row["NumericScale"] = definition.Scale;
                row["ProviderSpecificDataType"] = definition.Type;
                row["ProviderType"] = definition.CLRType;
                row["UdtAssemblyQualifiedName"] = DBNull.Value;
                row["XmlSchemaCollectionDatabase"] = DBNull.Value;
                row["XmlSchemaCollectionName"] = DBNull.Value;
                row["XmlSchemaCollectionOwningSchema"] = DBNull.Value;

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
                return true;
            }
            return false;
        }

        public bool Read()
        {
            if (Row < Data[Set].Rows.Count)
            {
                Row++;
                return true;
            }
            return false;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Data = null;
            _cachedSchemaTable = null;
        }
        #endregion
    }
}

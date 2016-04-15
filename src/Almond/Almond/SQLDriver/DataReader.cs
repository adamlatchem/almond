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
        private int Set
        {
            get; set;
        }

        /// <summary>
        /// The row in the current set we are on
        /// </summary>
        private int Row
        {
            get; set;
        }
        #endregion

        public DataReader(ResultSet resultsSetPacket)
        {
            if (resultsSetPacket == null)
                throw new ProtocolException("ResultSet must not be null");
            Data = new List<ResultSet>() { resultsSetPacket };
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
                throw new NotImplementedException();
            }
        }

        public void Close()
        {
            Dispose();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
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

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
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
        }
        #endregion
    }
}

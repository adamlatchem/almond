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
using System.Data;

namespace Almond.SQLDriver
{
    /// <summary>
    /// Almond implementation of the IDbDataParameter interface
    /// </summary>
    public class DbDataParameter : IDbDataParameter
    {
        #region IDbDataParameter
        public DbType DbType
        {
            get; set;
        }

        public ParameterDirection Direction
        {
            get; set;
        }

        public bool IsNullable
        {
            get; /* DbDataParameter specific property*/ set;
        }

        public string ParameterName
        {
            get; set;
        }

        public byte Precision
        {
            get; set;
        }

        public byte Scale
        {
            get; set;
        }

        public int Size
        {
            get; set;
        }

        public string SourceColumn
        {
            get; set;
        }

        public DataRowVersion SourceVersion
        {
            get; set;
        }

        public object Value
        {
            get; set;
        }
        #endregion

        #region Ctors
        public DbDataParameter(
            string parameterName,
            string sourceColumn,
            DbType dbType,
            object value,
            bool isNullable,
            int size,
            byte scale,
            byte precision,
            DataRowVersion sourceVersion,
            ParameterDirection direction)
        {
            ParameterName = parameterName;
            SourceColumn = sourceColumn;
            DbType = dbType;
            Value = value;
            IsNullable = isNullable;
            Size = size;
            Scale = scale;
            Precision = precision;
            SourceVersion = sourceVersion;
            Direction = direction;
        }

        public DbDataParameter(
            string parameterName,
            string sourceColumn,
            DbType dbType)
            : this(parameterName, sourceColumn, dbType, null, false, 0, 0, 0, DataRowVersion.Default, ParameterDirection.InputOutput)
        {
            // NOP
        }

        public DbDataParameter()
            : this(null, null, DbType.Object)
        {
            // NOP
        }
        #endregion
    }
}

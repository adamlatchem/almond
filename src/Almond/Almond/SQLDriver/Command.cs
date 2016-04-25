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
using System;
using System.Data;

namespace Almond.SQLDriver
{
    /// <summary>
    /// An IDbCommand implementation for MySQL.
    /// </summary>
    public class Command : IDbCommand
    {
        internal Command(String commandText, IDbConnection connection)
        {
            CommandText = commandText;
            Connection = connection;
        }

        #region IDbCommand
        public string CommandText
        {
            get; set;
        }

        public int CommandTimeout
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public CommandType CommandType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        private Connection _connection;
        public IDbConnection Connection
        {
            get
            {
                return _connection;
            }
            set
            {
                if (value != null && !(value is Connection))
                    throw new SQLDriverException("Connection is of wrong type for this Command object");
                _connection = value as Connection;
            }
        }

        public IDataParameterCollection Parameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public IDbDataParameter CreateParameter()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // We do not own the connection
            Connection = null;
        }

        public int ExecuteNonQuery()
        {
            using (IDataReader result = ExecuteReader())
            {
                return result.RecordsAffected;
            }
        }

        public IDataReader ExecuteReader()
        {
            return _connection.ExecuteReader(this, CommandBehavior.Default);
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return _connection.ExecuteReader(this, behavior);
        }

        public object ExecuteScalar()
        {
            using (IDataReader result = ExecuteReader())
            {
                if (result.RecordsAffected == 1)
                {
                    result.Read();
                    return result.GetValue(0);
                }
                return result.RecordsAffected;
            }
        }

        public void Prepare()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

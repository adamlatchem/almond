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
using Almond.ProtocolDriver.Packets;
using System;
using System.Data;

namespace Almond.SQLDriver
{
    /// <summary>
    /// A class used to connect to a MySQL instance given a connection string
    /// </summary>
    public class Connection : IDbConnection, IDisposable
    {
        #region Members
        /// <summary>
        /// Used to manage the connection string in a strongly typed fashion.
        /// </summary>
        private ConnectionStringBuilder _connectionStringBuilder = new ConnectionStringBuilder();

        /// <summary>
        /// The line driver used to communicate with the server.
        /// </summary>
        private ProtocolDriver.ProtocolDriver _protocolDriver;
            private ProtocolDriver.ProtocolDriver ProtocolDriver
        {
            get
            {
                if (_protocolDriver == null)
                    throw new Exception("Connection is not connected - missing call to Open() or connection has been disposed.");
                return _protocolDriver;
            }
            set
            {
                _protocolDriver = value;
            }
        }
        #endregion

        /// <summary>
        /// Connect to MySQL using the connection string provided when Open is called.
        /// </summary>
        /// <param name="connectionString"></param>
        public Connection(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        #region IDbConnection
        public string ConnectionString
        {
            get
            {
                return _connectionStringBuilder.ConnectionString;
            }

            set
            {
                _connectionStringBuilder.ConnectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Database
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ConnectionState State
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            if (_protocolDriver != null)
            {
                _protocolDriver.Dispose();
                _protocolDriver = null;
            }
        }

        public IDbCommand CreateCommand()
        {
            return new Command(String.Empty, this);
        }

        public void Open()
        {
            ProtocolDriver = new ProtocolDriver.ProtocolDriver(_connectionStringBuilder);
        }
        #endregion

        /// <summary>
        /// Execute the given command on the server and return the results.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal IDataReader ExecuteReader(Command command)
        {
            ResultSet resultset = ProtocolDriver.ExecuteQuery(command.CommandText);
            throw new NotImplementedException();
        }

        #region IDisposable
        public void Dispose()
        {
            if (_protocolDriver != null)
            {
                _protocolDriver.Dispose();
                _protocolDriver = null;
            }
        }
        #endregion
    }
}

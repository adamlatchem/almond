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
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>
        /// Returns the thread id upon connected.
        /// </summary>
        public UInt64 ThreadId
        {
            get; private set;
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
                return (int)_connectionStringBuilder.ConnectionTimeout;
            }
        }

        private string _database;
        public string Database
        {
            get
            {
                return _database;
            }
        }

        private ConnectionState _connectionState = ConnectionState.Closed;
        public ConnectionState State
        {
            get
            {
                return _connectionState;
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
            if (_protocolDriver != null)
            {
                _protocolDriver.ChangeDatabase(databaseName);
                _database = databaseName;
            }
        }

        public void Close()
        {
            if (_protocolDriver != null)
            {
                _protocolDriver.Dispose();
                _protocolDriver = null;
                _connectionState = ConnectionState.Closed;
            }
        }

        public IDbCommand CreateCommand()
        {
            return new DbCommand(String.Empty, this);
        }

        public void Open()
        {
            if (_protocolDriver == null)
            {
                _connectionState = ConnectionState.Connecting;
                ProtocolDriver = new ProtocolDriver.ProtocolDriver(_connectionStringBuilder);
                _database = _connectionStringBuilder.Database;
                _connectionState = ConnectionState.Open;

                using (DbCommand connectionID = new DbCommand("SELECT CONNECTION_ID()", this))
                {
                    ThreadId = (UInt64)connectionID.ExecuteScalar();
                }
            }
        }
        #endregion

        /// <summary>
        /// Execute the given command on the server and return the results.
        /// </summary>
        /// <param name="command">The command object to execute</param>
        /// <param name="behavior">Behavior required of the command object</param>
        /// <param name="timeout">timeout in seconds for the command</param>
        /// <returns></returns>
        internal IDataReader ExecuteReader(DbCommand command, CommandBehavior behavior, int timeout)
        {
            if (timeout < 0)
                throw new ArgumentException("Timeout must be non negative");

            ManualResetEvent completedSignal = new ManualResetEvent(false);
            completedSignal.Reset();

            object workerResult = null;
            Task.Factory.StartNew(() => {
                try
                {
                    ResultSet<Row> resultset = ProtocolDriver.ExecuteQuery(command.CommandText);
                    workerResult = new DataReader<Row>(resultset, (Connection)command.Connection, behavior);
                }
                catch (Exception e)
                {
                    workerResult = e;
                }
                finally
                {
                    completedSignal.Set();
                }
            });

            bool completed = completedSignal.WaitOne(timeout == 0 ? System.Threading.Timeout.Infinite : 1000 * (int)timeout);
            if (!completed)
                throw new TimeoutException(string.Format("Command timeout after {0} seconds", timeout));
            if (workerResult is Exception)
                throw (Exception)workerResult;

            return workerResult as IDataReader;
        }

        /// <summary>
        /// Prepare the given command and return the prepared statement id
        /// </summary>
        /// <param name="command"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public int PrepareStatement(DbCommand command, int timeout)
        {
            if (timeout < 0)
                throw new ArgumentException("Timeout must be non negative");

            ManualResetEvent completedSignal = new ManualResetEvent(false);
            completedSignal.Reset();

            object workerResult = -1;
            Task.Factory.StartNew(() => {
                try
                {
                    workerResult = ProtocolDriver.PrepareStatement(command.CommandText);
                }
                catch (Exception e)
                {
                    workerResult = e;
                }
                finally
                {
                    completedSignal.Set();
                }
            });

            bool completed = completedSignal.WaitOne(timeout == 0 ? System.Threading.Timeout.Infinite : 1000 * (int)timeout);
            if (!completed)
                throw new TimeoutException(string.Format("Command timeout after {0} seconds", timeout));
            if (workerResult is Exception)
                throw (Exception)workerResult;

            return (int)workerResult;
        }

        #region IDisposable
        public void Dispose()
        {
            if (_protocolDriver != null)
            {
                _protocolDriver.Dispose();
                _protocolDriver = null;
                _connectionState = ConnectionState.Closed;
            }
        }
        #endregion
    }
}

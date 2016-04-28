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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace Almond.SQLDriver.Tests
{
    [TestClass]
    public class ConnectionTests
    {
        /// <summary>
        /// The object under test
        /// </summary>
        public static Connection _connection;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _connection = new Connection("hostname=localhost;username=test;password=test;database=mysql");
            Assert.IsNotNull(_connection);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _connection.Dispose();
            _connection = null;
        }

        [TestMethod]
        public void ConnectionStringTest()
        {
            string oldString = _connection.ConnectionString;
            string connectionString = "hostname=localhost;username=test;password=test";
            _connection.ConnectionString = connectionString;
            Assert.AreEqual(connectionString, _connection.ConnectionString);

            _connection.ConnectionString = oldString;
        }

        [ExpectedException(typeof(TimeoutException))]
        [TestMethod]
        public void ConnectionTimeoutTest()
        {
            // Connect to port 1 which is unlikely to work
            using (Connection connection = new Connection("hostname=localhost;username=test;password=test;port=1;connectiontimeout=1"))
            {
                Assert.AreEqual(1, connection.ConnectionTimeout);
                connection.Open();
            }
            Assert.Fail();
        }

        [TestMethod]
        public void DatabaseTest()
        {
            _connection.Open();
            Assert.AreEqual("mysql", _connection.Database);
        }

        [TestMethod]
        public void StateTest()
        {
            Connection connection = new Connection(_connection.ConnectionString);
            Assert.AreEqual(ConnectionState.Closed, connection.State);

            connection.Open();
            Assert.AreEqual(ConnectionState.Open, connection.State);

            connection.Close();
            Assert.AreEqual(ConnectionState.Closed, connection.State);

            connection.Dispose();
        }

        [TestMethod]
        public void BeginTransactionTest()
        {
            IDbTransaction transaction = _connection.BeginTransaction();
            transaction.Rollback();
            transaction = _connection.BeginTransaction(IsolationLevel.Chaos);
            transaction.Rollback();
            transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
            transaction.Rollback();
            transaction = _connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            transaction.Rollback();
            transaction = _connection.BeginTransaction(IsolationLevel.RepeatableRead);
            transaction.Rollback();
            transaction = _connection.BeginTransaction(IsolationLevel.Serializable);
            transaction.Rollback();
            transaction = _connection.BeginTransaction(IsolationLevel.Snapshot);
            transaction.Rollback();
            transaction = _connection.BeginTransaction(IsolationLevel.Unspecified);
            transaction.Rollback();
        }

        [TestMethod]
        public void ChangeDatabaseTest()
        {
            _connection.Open();
            _connection.ChangeDatabase("test");

            IDbCommand command = new Command("SELECT DATABASE()", _connection);
            string result = (string)command.ExecuteScalar();
            Assert.AreEqual("test", result);
        }

        [TestMethod]
        public void CloseTest()
        {
            _connection.Close();
        }

        [TestMethod]
        public void CreateCommandTest()
        {
            IDbCommand command = _connection.CreateCommand();
            Assert.IsNotNull(command);
            Assert.AreEqual(_connection, command.Connection);
        }

        [TestMethod]
        public void OpenTest()
        {
            _connection.Open();
        }

        [TestMethod]
        public void DisposeTest()
        {
            Connection testConnection = new Connection("hostname=byefornow");
            testConnection.Dispose();
        }

        [TestMethod()]
        public void ExecuteReaderTest()
        {
            _connection.Open();
            Command command = new Command("SELECT 1", _connection);
            IDataReader result = _connection.ExecuteReader(command, CommandBehavior.Default, /*Timeout*/0);
            Assert.IsNotNull(result);
            result.Read();
            Assert.AreEqual(1, result.RecordsAffected);
            Assert.AreEqual("1", result.GetString(0));
        }
    }
}

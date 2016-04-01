﻿#region License
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
using Almond.MySQLDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace Almond.UnitTest
{
    [TestClass]
    public class ConnectionUnitTest
    {
        /// <summary>
        /// The object under test
        /// </summary>
        public static Connection connection;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            connection = new Connection("testing 123");
            Assert.IsNotNull(connection);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            connection.Dispose();
            connection = null;
        }

        [TestMethod]
        public void ConnectionString()
        {
            string connectionString = "testing 123";
            connection.ConnectionString = connectionString;
            Assert.AreEqual(connectionString, connection.ConnectionString);
        }

        [TestMethod]
        public void ConnectionTimeout()
        {
            Assert.AreEqual(30, connection.ConnectionTimeout);
        }

        [TestMethod]
        public void Database()
        {
            Assert.AreEqual("test", connection.Database);
        }

        [TestMethod]
        public void State()
        {
            Assert.AreEqual(ConnectionState.Broken, connection.State);
        }

        [TestMethod]
        public void BeginTransaction()
        {
            IDbTransaction transaction = connection.BeginTransaction();
            transaction.Rollback();
            transaction = connection.BeginTransaction(IsolationLevel.Chaos);
            transaction.Rollback();
            transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            transaction.Rollback();
            transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            transaction.Rollback();
            transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);
            transaction.Rollback();
            transaction = connection.BeginTransaction(IsolationLevel.Serializable);
            transaction.Rollback();
            transaction = connection.BeginTransaction(IsolationLevel.Snapshot);
            transaction.Rollback();
            transaction = connection.BeginTransaction(IsolationLevel.Unspecified);
            transaction.Rollback();
        }

        [TestMethod]
        public void ChangeDatabase()
        {
            connection.ChangeDatabase("UnitTest");
        }

        [TestMethod]
        public void Close()
        {
            connection.Close();
        }

        [TestMethod]
        public void CreateCommand()
        {
            IDbCommand command = connection.CreateCommand();
            Assert.IsNotNull(command);
        }

        [TestMethod]
        public void Open()
        {
            connection.Open();
        }

        [TestMethod]
        public void Dispose()
        {
            Connection testConnection = new Connection("this is a test");
            testConnection.Dispose();
        }
    }
}
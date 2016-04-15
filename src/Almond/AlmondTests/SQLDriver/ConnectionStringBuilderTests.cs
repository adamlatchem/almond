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
using Almond.SQLDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Almond.SQLDriver.Tests
{
    [TestClass]
    public class ConnectionStringBuilderTests
    {
        /// <summary>
        /// The ConnectionStringBuilder under test
        /// </summary>
        private static ConnectionStringBuilder _connectionStringBuilder;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _connectionStringBuilder = new ConnectionStringBuilder();
            Assert.IsNotNull(_connectionStringBuilder);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _connectionStringBuilder = null;
        }

        #region DbConnectionStringBuilder
        [TestMethod]
        public void NullConnectionStringTest()
        {
            _connectionStringBuilder.ConnectionString = null;
            Assert.AreEqual(String.Empty, _connectionStringBuilder.ConnectionString);
        }

        [TestMethod]
        public void SimpleConnectionStringTest()
        {
            string connectionString = "hostname=local";
            _connectionStringBuilder.ConnectionString = connectionString;
            Assert.AreEqual(connectionString, _connectionStringBuilder.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringCleanupTest()
        {
            string connectionString = "HOStname= local;";
            _connectionStringBuilder.ConnectionString = connectionString;
            Assert.AreEqual("hostname=local", _connectionStringBuilder.ConnectionString);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BadConnectionStringThrowsExceptionTest()
        {
            string connectionString = "This will fail";
            _connectionStringBuilder.ConnectionString = connectionString;
        }
        #endregion

        [TestMethod]
        public void HostnameTest()
        {
            string hostname = "localhost.localdomain";
            _connectionStringBuilder.Hostname = hostname;
            Assert.AreEqual(hostname, _connectionStringBuilder.Hostname);
        }

        [TestMethod]
        public void PortTest()
        {
            int port = 1234;
            _connectionStringBuilder.Port = port;
            Assert.AreEqual(port, _connectionStringBuilder.Port);
        }

        [TestMethod]
        public void UsernameTest()
        {
            string username = "test";
            _connectionStringBuilder.Username = username;
            Assert.AreEqual(username, _connectionStringBuilder.Username);
        }

        [TestMethod]
        public void PasswordTest()
        {
            string password = "test";
            _connectionStringBuilder.Password = password;
            Assert.AreEqual(password, _connectionStringBuilder.Password);
        }

        [TestMethod]
        public void DatabaseTest()
        {
            string database = "test";
            _connectionStringBuilder.Database = database;
            Assert.AreEqual(database, _connectionStringBuilder.Database);
        }

        [TestMethod]
        public void DefaultsTest()
        {
            _connectionStringBuilder = new ConnectionStringBuilder();
            Assert.AreEqual("localhost", _connectionStringBuilder.Hostname);
            Assert.AreEqual(3306, _connectionStringBuilder.Port);
            Assert.AreEqual(String.Empty, _connectionStringBuilder.Database);
            Assert.AreEqual(Math.Pow(2, 24) - 1, _connectionStringBuilder.MaxPacketSize);
        }
    }
}

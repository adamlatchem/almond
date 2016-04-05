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
using System;

namespace Almond.UnitTest.MySQLDrvier
{
    [TestClass]
    public class ConnectionStringBuilderUnitTest
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
        public void TestNullConnectionString()
        {
            _connectionStringBuilder.ConnectionString = null;
            Assert.AreEqual(String.Empty, _connectionStringBuilder.ConnectionString);
        }

        [TestMethod]
        public void TestSimpleConnectionString()
        {
            string connectionString = "hostname=local";
            _connectionStringBuilder.ConnectionString = connectionString;
            Assert.AreEqual(connectionString, _connectionStringBuilder.ConnectionString);
        }

        [TestMethod]
        public void TestConnectionStringCleanup()
        {
            string connectionString = "HOStname= local;";
            _connectionStringBuilder.ConnectionString = connectionString;
            Assert.AreEqual("hostname=local", _connectionStringBuilder.ConnectionString);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadConnectionStringThrowsException()
        {
            string connectionString = "This will fail";
            _connectionStringBuilder.ConnectionString = connectionString;
        }
        #endregion

        [TestMethod]
        public void TestHostname()
        {
            string hostname = "localhost.localdomain";
            _connectionStringBuilder.Hostname = hostname;
            Assert.AreEqual(hostname, _connectionStringBuilder.Hostname);
        }

        [TestMethod]
        public void TestPort()
        {
            int port = 1234;
            _connectionStringBuilder.Port = port;
            Assert.AreEqual(port, _connectionStringBuilder.Port);
        }

        [TestMethod]
        public void TestDefaults()
        {
            _connectionStringBuilder = new ConnectionStringBuilder();
            Assert.AreEqual("localhost", _connectionStringBuilder.Hostname);
            Assert.AreEqual(3306, _connectionStringBuilder.Port);
        }
    }
}

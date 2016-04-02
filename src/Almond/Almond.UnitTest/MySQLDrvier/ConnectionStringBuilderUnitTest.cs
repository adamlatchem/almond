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
        private static ConnectionStringBuilder connectionStringBuilder;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            connectionStringBuilder = new ConnectionStringBuilder();
            Assert.IsNotNull(connectionStringBuilder);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            connectionStringBuilder = null;
        }

        #region DbConnectionStringBuilder
        [TestMethod]
        public void TestNullConnectionString()
        {
            connectionStringBuilder.ConnectionString = null;
            Assert.AreEqual(String.Empty, connectionStringBuilder.ConnectionString);
        }

        [TestMethod]
        public void TestSimpleConnectionString()
        {
            string connectionString = "hostname=local";
            connectionStringBuilder.ConnectionString = connectionString;
            Assert.AreEqual(connectionString, connectionStringBuilder.ConnectionString);
        }

        [TestMethod]
        public void TestConnectionStringCleanup()
        {
            string connectionString = "HOStname= local;";
            connectionStringBuilder.ConnectionString = connectionString;
            Assert.AreEqual("hostname=local", connectionStringBuilder.ConnectionString);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadConnectionStringThrowsException()
        {
            string connectionString = "This will fail";
            connectionStringBuilder.ConnectionString = connectionString;
        }
        #endregion

        [TestMethod]
        public void TestHostname()
        {
            string hostname = "localhost.localdomain";
            connectionStringBuilder.Hostname = hostname;
            Assert.AreEqual(hostname, connectionStringBuilder.Hostname);
        }

        [TestMethod]
        public void TestPort()
        {
            int port = 1234;
            connectionStringBuilder.Port = port;
            Assert.AreEqual(port, connectionStringBuilder.Port);
        }
    }
}

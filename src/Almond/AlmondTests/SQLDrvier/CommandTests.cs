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
using System.Data;

namespace Almond.SQLDrvier.Tests
{
    [TestClass]
    public class CommandTests
    {
        /// <summary>
        /// Connection used for the unit test
        /// </summary>
        public static Connection _connection;

        /// <summary>
        /// Command under test
        /// </summary>
        public static IDbCommand _command;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Create the objects to unit test
            _connection = new Connection("hostname=localhost;username=test;password=test");
            _connection.Open();
            _command = _connection.CreateCommand();
            Assert.IsNotNull(_command);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _command = null;
            _connection.Dispose();
            _connection = null;
        }

        [TestMethod]
        public void CommandTextTest()
        {
            string commandText = "SELECT 1";
            _command.CommandText = commandText;
            Assert.AreEqual(commandText, _command.CommandText);
        }

        [TestMethod]
        public void CommandTimeoutTest()
        {
            int commandTimeout = 42;
            _command.CommandTimeout = commandTimeout;
            Assert.AreEqual(commandTimeout, _command.CommandTimeout);
        }

        [TestMethod]
        public void CommandTypeTest()
        {
            CommandType commandType = System.Data.CommandType.Text;
            _command.CommandType = commandType;
            Assert.AreEqual(commandType, _command.CommandType);
        }

        [TestMethod]
        public void ConnectionTest0()
        {
            Assert.AreEqual(_connection, _command.Connection);
        }

        [ExpectedException(typeof(SQLDriverException))]
        [TestMethod]
        public void ConnectionTest1()
        {
            _command.Connection = new System.Data.SqlClient.SqlConnection("");
        }

        [TestMethod]
        public void ParametersTest()
        {
            Assert.IsNotNull(_command.Parameters);
        }

        [TestMethod]
        public void TransactionTest()
        {
            IDbTransaction transaction = _connection.BeginTransaction();
            Assert.AreEqual(transaction, _command.Transaction);
            transaction.Rollback();
        }

        [TestMethod]
        public void UpdatedRowSourceTest()
        {
            UpdateRowSource rowSource = System.Data.UpdateRowSource.None;
            _command.UpdatedRowSource = rowSource;
            Assert.AreEqual(rowSource, _command.UpdatedRowSource);
        }

        [TestMethod]
        public void CancelTest()
        {
            _command.Cancel();
        }

        [TestMethod]
        public void CreateParameterTest()
        {
            IDbDataParameter parameter = _command.CreateParameter();
            Assert.IsNotNull(parameter);
        }

        [TestMethod]
        public void DisposeTest()
        {
            Command testCommand = new Command(string.Empty, _connection);
            testCommand.Dispose();
        }

        [TestMethod]
        public void ExecuteNonQueryTest()
        {
            int result = _command.ExecuteNonQuery();
            Assert.AreEqual(-42, result);
        }

        [TestMethod]
        public void ExecuteReaderTest()
        {
            IDataReader dataReader = _command.ExecuteReader();
            Assert.IsNotNull(dataReader);

            dataReader = _command.ExecuteReader(CommandBehavior.CloseConnection);
            Assert.IsNotNull(dataReader);
            dataReader = _command.ExecuteReader(CommandBehavior.Default);
            Assert.IsNotNull(dataReader);
            dataReader = _command.ExecuteReader(CommandBehavior.KeyInfo);
            Assert.IsNotNull(dataReader);
            dataReader = _command.ExecuteReader(CommandBehavior.SchemaOnly);
            Assert.IsNotNull(dataReader);
            dataReader = _command.ExecuteReader(CommandBehavior.SequentialAccess);
            Assert.IsNotNull(dataReader);
            dataReader = _command.ExecuteReader(CommandBehavior.SingleResult);
            Assert.IsNotNull(dataReader);
            dataReader = _command.ExecuteReader(CommandBehavior.SingleRow);
            Assert.IsNotNull(dataReader);
        }

        [TestMethod]
        public void ExecuteScalarTest()
        {
            object result = _command.ExecuteScalar();
            Assert.AreEqual(result, "fix this");
        }

        [TestMethod]
        public void PrepareTest()
        {
            _command.Prepare();
        }
    }
}

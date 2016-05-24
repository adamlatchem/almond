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
    public class CommandTests
    {
        /// <summary>
        /// Connection used for the unit test
        /// </summary>
        static public Connection _connection;

        /// <summary>
        /// Command under test
        /// </summary>
        static public IDbCommand _command;

        [ClassInitialize]
        static public void ClassInitialize(TestContext context)
        {
            // Create the objects to unit test
            _connection = new Connection("hostname=localhost;username=test;password=test");
            _connection.Open();
            _command = _connection.CreateCommand();
            Assert.IsNotNull(_command);
        }

        [ClassCleanup]
        static public void ClassCleanup()
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

        [ExpectedException(typeof(TimeoutException))]
        [TestMethod]
        public void CommandTimeoutTest()
        {
            // Test default value
            Assert.AreEqual(30, _command.CommandTimeout);

            int commandTimeout = 42;
            _command.CommandTimeout = commandTimeout;
            Assert.AreEqual(commandTimeout, _command.CommandTimeout);

            try
            {
                _command.CommandTimeout = 1;
                _command.CommandText = "SELECT SLEEP (20)";
                int result = _command.ExecuteNonQuery();
            }
            finally
            {
                // ADO.NET does not define the state after a timeout so the best thing to
                // do is create a new connection so other tests to continue
                _command.Cancel();
                ClassInitialize(null);
            }
        }

        [TestMethod]
        public void CommandTypeTest()
        {
            CommandType commandType = CommandType.Text;
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
            DbCommand testCommand = new DbCommand(string.Empty, _connection);
            testCommand.Cancel();
        }

        [TestMethod]
        public void CreateParameterTest()
        {
            IDbDataParameter parameter = _command.CreateParameter();
            Assert.IsNotNull(parameter);

            Assert.AreEqual(false, parameter.IsNullable);
            Assert.AreEqual(null, parameter.SourceColumn);
            Assert.AreEqual(null, parameter.ParameterName);
            Assert.AreEqual(null, parameter.Value);
            Assert.AreEqual(0, parameter.Size);
            Assert.AreEqual(0, parameter.Precision);
            Assert.AreEqual(0, parameter.Scale);
            Assert.AreEqual(ParameterDirection.InputOutput, parameter.Direction);
            Assert.AreEqual(DataRowVersion.Default, parameter.SourceVersion);
            Assert.AreEqual(DbType.Object, parameter.DbType);
        }

        [TestMethod]
        public void DisposeTest()
        {
            DbCommand testCommand = new DbCommand(string.Empty, _connection);
            testCommand.Dispose();
        }

        [TestMethod]
        public void ExecuteNonQueryTest()
        {
            _command.CommandText = "SHOW CREATE TABLE mysql.help_topic;";
            int result = _command.ExecuteNonQuery();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ExecuteReaderTest()
        {
            _command.CommandText = "SELECT 2";
            IDataReader dataReader = _command.ExecuteReader();
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

            dataReader = _command.ExecuteReader(CommandBehavior.CloseConnection);
            Assert.IsNotNull(dataReader);
            dataReader.Close();
            Assert.AreEqual(ConnectionState.Closed, _connection.State);
            _connection.Open();
        }

        [TestMethod]
        public void ExecuteScalarTest()
        {
            _command.CommandText = "SELECT 'get this'";
            object result = _command.ExecuteScalar();
            Assert.AreEqual("get this", result);

            _command.CommandText = "SELECT 4";
            result = _command.ExecuteScalar();
            Assert.AreEqual(4, (Int64)result);

            _command.CommandText = "SELECT * FROM mysql.help_topic LIMIT 20";
            result = _command.ExecuteScalar();
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void PrepareTest()
        {
            _command.CommandText = "SELECT CONCAT(?, ?)";
            _command.Prepare();
            Assert.AreNotEqual(null, ((DbCommand)_command).PreparedStatement);

            string param1 = "ThisIs";
            string param2 = "ATest";

            IDataReader reader = _command.ExecuteReader();
            reader.Read();
            string result = reader.GetString(0);
            Assert.AreEqual(result, param1 + param2);
        }
    }
}

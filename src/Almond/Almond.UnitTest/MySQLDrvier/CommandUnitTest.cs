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
using System.Data;

namespace Almond.UnitTest
{
    [TestClass]
    public class CommandUnitTest
    {
        /// <summary>
        /// Connection used for the unit test
        /// </summary>
        public static Connection connection;

        /// <summary>
        /// Command under test
        /// </summary>
        public static Command command;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Create the objects to unit test
            connection = new Connection("testing 123");
            command = new Command(String.Empty, connection);
            Assert.IsNotNull(command);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            command = null;
            connection.Dispose();
            connection = null;
        }

        [TestMethod]
        public void CommandText()
        {
            string commandText = "testing 123";
            command.CommandText = commandText;
            Assert.AreEqual(commandText, command.CommandText);
        }

        [TestMethod]
        public void CommandTimeout()
        {
            int commandTimeout = 42;
            command.CommandTimeout = commandTimeout;
            Assert.AreEqual(commandTimeout, command.CommandTimeout);
        }

        [TestMethod]
        public void CommandType()
        {
            CommandType commandType = System.Data.CommandType.Text;
            command.CommandType = commandType;
            Assert.AreEqual(commandType, command.CommandType);
        }

        [TestMethod]
        public void Connection()
        {
            Assert.AreEqual(connection, command.Connection);
        }

        [TestMethod]
        public void Parameters()
        {
            Assert.IsNotNull(command.Parameters);
        }

        [TestMethod]
        public void Transaction()
        {
            IDbTransaction transaction = connection.BeginTransaction();
            Assert.AreEqual(transaction, command.Transaction);
            transaction.Rollback();
        }

        [TestMethod]
        public void UpdatedRowSource()
        {
            UpdateRowSource rowSource = System.Data.UpdateRowSource.None;
            command.UpdatedRowSource = rowSource;
            Assert.AreEqual(rowSource, command.UpdatedRowSource);
        }

        [TestMethod]
        public void Cancel()
        {
            command.Cancel();
        }

        [TestMethod]
        public void CreateParameter()
        {
            IDbDataParameter parameter = command.CreateParameter();
            Assert.IsNotNull(parameter);
        }

        [TestMethod]
        public void Dispose()
        {
            Command testCommand = new Command(string.Empty, connection);
            testCommand.Dispose();
        }

        [TestMethod]
        public void ExecuteNonQuery()
        {
            int result = command.ExecuteNonQuery();
            Assert.AreEqual(-42, result);
        }

        [TestMethod]
        public void ExecuteReader()
        {
            IDataReader dataReader = command.ExecuteReader();
            Assert.IsNotNull(dataReader);

            dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            Assert.IsNotNull(dataReader);
            dataReader = command.ExecuteReader(CommandBehavior.Default);
            Assert.IsNotNull(dataReader);
            dataReader = command.ExecuteReader(CommandBehavior.KeyInfo);
            Assert.IsNotNull(dataReader);
            dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly);
            Assert.IsNotNull(dataReader);
            dataReader = command.ExecuteReader(CommandBehavior.SequentialAccess);
            Assert.IsNotNull(dataReader);
            dataReader = command.ExecuteReader(CommandBehavior.SingleResult);
            Assert.IsNotNull(dataReader);
            dataReader = command.ExecuteReader(CommandBehavior.SingleRow);
            Assert.IsNotNull(dataReader);
        }

        [TestMethod]
        public void ExecuteScalar()
        {
            object result = command.ExecuteScalar();
            Assert.AreEqual(result, "fix this");
        }

        [TestMethod]
        public void Prepare()
        {
            command.Prepare();
        }
    }
}

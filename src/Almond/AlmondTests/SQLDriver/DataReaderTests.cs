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
    [TestClass()]
    public class DataReaderTests
    {
        /// <summary>
        /// Connection used for the unit test
        /// </summary>
        public static Connection _connection;

        /// <summary>
        /// Command used for the unit test
        /// </summary>
        public static IDbCommand _command;

        /// <summary>
        /// The data reader under test.
        /// </summary>
        public static IDataReader _datareader;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Create the objects to unit test
            _connection = new Connection("hostname=localhost;username=test;password=test");
            _connection.Open();
            _command = _connection.CreateCommand();
            _command.CommandText = "SELECT * FROM information_schema.FILES";
            _datareader = _command.ExecuteReader();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _datareader.Close();
            _datareader = null;
            _command.Dispose();
            _command = null;
            _connection.Dispose();
            _connection = null;
        }

        [TestMethod()]
        public void PropertiesTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void CloseTest()
        {
            throw new NotImplementedException();
            IDataReader reader = _command.ExecuteReader();
            reader.Close();
            Assert.IsTrue(reader.IsClosed == true);
        }

        [TestMethod()]
        public void GetBooleanTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetByteTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetBytesTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetCharTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetCharsTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetDataTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetDateTimeTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetDecimalTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetDoubleTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetFloatTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetGuidTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetInt16Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetInt32Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetInt64Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetStringTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetValueTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetValuesTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void IsDBNullTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetDataTypeNameTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetFieldTypeTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetNameTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetOrdinalTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetSchemaTableTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void NextResultTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ReadTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            throw new NotImplementedException();
        }
    }
}
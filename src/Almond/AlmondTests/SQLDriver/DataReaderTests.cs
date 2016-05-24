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

/// <summary>
/*
CREATE TABLE `typeTest` (
  `intTest` int(11) DEFAULT NULL,
  `varcharTest` varchar(45) DEFAULT NULL,
  `decimalTest` decimal(10,8) DEFAULT NULL,
  `datetimeTest` datetime DEFAULT NULL,
  `blobTest` blob,
  `floatTest` float DEFAULT NULL,
  `doubleTest` double DEFAULT NULL,
  `bitTest` bit(1) DEFAULT NULL,
  `byteTest` varchar(45) DEFAULT NULL,
  `guidTest` varchar(36) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf16;

INSERT INTO `test`.`typeTest` (
  `intTest`,
  `varcharTest`,
  `decimalTest`,
  `datetimeTest`,
  `floatTest`,
  `doubleTest`,
  `bitTest`,
  `byteTest`,
  `guidtest`
) VALUES (
  1,
  'test',
  1.234567,
  '1976-05-04 09:00',
  64.125,
  128.256,
  true,
  42,
  '9f270d4b-dc68-47db-ace6-554b3bbf2333'
)
*/
/// </summary>
namespace Almond.SQLDriver.Tests
{
    [TestClass()]
    public class DataReaderTests
    {
        /// <summary>
        /// Connection used for the unit test
        /// </summary>
        static public Connection _connection;

        /// <summary>
        /// Command used for the unit test
        /// </summary>
        static public IDbCommand _command;

        /// <summary>
        /// The data reader under test.
        /// </summary>
        static public IDataReader _datareader;

        [ClassInitialize]
        static public void ClassInitialize(TestContext context)
        {
            // Create the objects to unit test
            _connection = new Connection("hostname=localhost;username=test;password=test");
            _connection.Open();
            _command = _connection.CreateCommand();
            _command.CommandText = "SELECT * FROM test.typeTest";
            _datareader = _command.ExecuteReader();
            _datareader.Read();
        }

        [ClassCleanup]
        static public void ClassCleanup()
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
            Assert.AreEqual(0, _datareader.Depth);
            Assert.AreEqual(10, _datareader.FieldCount);
            Assert.AreEqual(false, _datareader.IsClosed);
            Assert.AreEqual(1, _datareader.RecordsAffected);
        }

        [TestMethod()]
        public void CloseTest()
        {
            IDataReader reader = _command.ExecuteReader();
            reader.Close();
            Assert.IsTrue(reader.IsClosed == true);
        }

        [TestMethod()]
        public void GetBooleanTest()
        {
            int boolOrdinal = _datareader.GetOrdinal("bitTest");
            bool testValue = _datareader.GetBoolean(boolOrdinal);
            Assert.AreEqual(true, testValue);
        }

        [TestMethod()]
        public void GetByteTest()
        {
            int byteOrdinal = _datareader.GetOrdinal("byteTest");
            byte testValue = _datareader.GetByte(byteOrdinal);
            Assert.AreEqual(42, testValue);
        }

        [TestMethod()]
        public void GetBytesTest()
        {
            int varcharOrdinal = _datareader.GetOrdinal("varcharTest");
            byte[] buffer = new byte[100];
            long result = _datareader.GetBytes(varcharOrdinal, 0, buffer, 0, 100);
            Assert.AreEqual(4, result);
            Assert.AreEqual((byte)'t', buffer[0]);
            Assert.AreEqual((byte)'e', buffer[1]);
            Assert.AreEqual((byte)'s', buffer[2]);
            Assert.AreEqual((byte)'t', buffer[3]);
        }

        [TestMethod()]
        public void GetCharTest()
        {
            int varcharOrdinal = _datareader.GetOrdinal("varcharTest");
            char testValue = _datareader.GetChar(varcharOrdinal);
            Assert.AreEqual('t', testValue);
        }

        [TestMethod()]
        public void GetCharsTest()
        {
            int varcharOrdinal = _datareader.GetOrdinal("varcharTest");
            char[] buffer = new char[100];
            long result = _datareader.GetChars(varcharOrdinal, 0, buffer, 0, 100);
            Assert.AreEqual(4, result);
            Assert.AreEqual('t', buffer[0]);
            Assert.AreEqual('e', buffer[1]);
            Assert.AreEqual('s', buffer[2]);
            Assert.AreEqual('t', buffer[3]);
        }

        [ExpectedException(typeof(NotImplementedException))]
        [TestMethod()]
        public void GetDataTest()
        {
            _datareader.GetData(0);
        }

        [TestMethod()]
        public void GetDateTimeTest()
        {
            int datetimeOrdinal = _datareader.GetOrdinal("datetimeTest");
            DateTime testValue = _datareader.GetDateTime(datetimeOrdinal);
            Assert.AreEqual(new DateTime(1976, 5, 4, 9, 0, 0), testValue);
        }

        [TestMethod()]
        public void GetDecimalTest()
        {
            int decimalOrdinal = _datareader.GetOrdinal("decimalTest");
            decimal testValue = _datareader.GetDecimal(decimalOrdinal);
            Assert.AreEqual(new Decimal(1.234567), testValue);
        }

        [TestMethod()]
        public void GetDoubleTest()
        {
            int doubleOrdinal = _datareader.GetOrdinal("doubleTest");
            double testValue = _datareader.GetDouble(doubleOrdinal);
            Assert.AreEqual(128.256, testValue);
        }

        [TestMethod()]
        public void GetFloatTest()
        {
            int floatOrdinal = _datareader.GetOrdinal("floatTest");
            float testValue = _datareader.GetFloat(floatOrdinal);
            Assert.AreEqual(64.125, testValue);
        }

        [TestMethod()]
        public void GetGuidTest()
        {
            int guidOrdinal = _datareader.GetOrdinal("guidTest");
            Guid testValue = _datareader.GetGuid(guidOrdinal);
            Guid g = new Guid("9f270d4b-dc68-47db-ace6-554b3bbf2333");
            Assert.AreEqual(g, testValue);
        }

        [TestMethod()]
        public void GetInt16Test()
        {
            int intOrdinal = _datareader.GetOrdinal("intTest");
            Int16 testValue = _datareader.GetInt16(intOrdinal);
            Assert.AreEqual(1, testValue);
        }

        [TestMethod()]
        public void GetInt32Test()
        {
            int intOrdinal = _datareader.GetOrdinal("intTest");
            Int32 testValue = _datareader.GetInt32(intOrdinal);
            Assert.AreEqual(1, testValue);
        }

        [TestMethod()]
        public void GetInt64Test()
        {
            int intOrdinal = _datareader.GetOrdinal("intTest");
            Int64 testValue = _datareader.GetInt64(intOrdinal);
            Assert.AreEqual(1, testValue);
        }

        [TestMethod()]
        public void GetStringTest()
        {
            int stringOrdinal = _datareader.GetOrdinal("varcharTest");
            string testValue = _datareader.GetString(stringOrdinal);
            Assert.AreEqual("test", testValue);
        }

        [TestMethod()]
        public void GetValueTest()
        {
            object value = _datareader.GetValue(2);
            Assert.AreEqual(new decimal(1.234567), value);
        }

        [TestMethod()]
        public void GetValuesTest()
        {
            object[] buffer = new object[100];
            int numberOfValues = _datareader.GetValues(buffer);
            Assert.AreEqual(10, numberOfValues);
        }

        [TestMethod()]
        public void IsDBNullTest()
        {
            int blobOrdinal = _datareader.GetOrdinal("blobTest");
            bool testValue = _datareader.IsDBNull(blobOrdinal);
            Assert.IsTrue(testValue);

            string stringValue = _datareader.GetString(blobOrdinal);
            Assert.IsNull(stringValue);
        }

        [TestMethod()]
        public void GetDataTypeNameTest()
        {
            string typeName = _datareader.GetDataTypeName(0);
            Assert.AreEqual("MYSQL_TYPE_LONG", typeName);
        }

        [TestMethod()]
        public void GetFieldTypeTest()
        {
            Type type = _datareader.GetFieldType(0);
            Assert.AreEqual(typeof(int), type);
        }

        [TestMethod()]
        public void GetNameTest()
        {
            string testName = _datareader.GetName(0);
            Assert.AreEqual("intTest", testName);
        }

        [TestMethod()]
        public void GetOrdinalTest()
        {
            int testOrdinal = _datareader.GetOrdinal("varcharTest");
            Assert.AreEqual(1, testOrdinal);
        }

        [TestMethod()]
        public void GetSchemaTableTest()
        {
            DataTable schema = _datareader.GetSchemaTable();
            Assert.AreEqual(10, schema.Rows.Count);
        }

        [TestMethod()]
        public void NextResultTest()
        {
            IDataReader test = _command.ExecuteReader();
            int count = 0;
            while (true)
            {
                while (test.Read())
                    count++;
                bool ok = test.NextResult();
                if (!ok)
                    break;
            }
            Assert.AreEqual(1, count);
            test.Dispose();
        }

        [TestMethod()]
        public void ReadTest()
        {
            IDataReader test = _command.ExecuteReader();
            int count = 0;
            while (test.Read())
                count++;
            Assert.AreEqual(1, count);
            test.Dispose();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            IDataReader test = _command.ExecuteReader();
            test.Dispose();
        }
    }
}
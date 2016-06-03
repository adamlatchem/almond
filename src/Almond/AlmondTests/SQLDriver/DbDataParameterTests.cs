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
using System.Data;

namespace Almond.SQLDriver.Tests
{
    [TestClass()]
    public class DbDataParameterTests
    {
        #region Test Data
        string parameterName = "testParam";
        string sourceColumn = "testSourceColumn";
        DbType type = DbType.AnsiString;
        object objectValue = "test";
        bool isNullable = false;
        int size = 24;
        byte scale = 10;
        byte precision = 18;
        DataRowVersion sourceVersion = DataRowVersion.Original;
        ParameterDirection direction = ParameterDirection.InputOutput;
        #endregion

        [TestMethod()]
        public void DbDataParameterTest_FullCtor()
        {
            DbDataParameter dbp = new DbDataParameter(
                parameterName,
                sourceColumn,
                type,
                objectValue,
                isNullable,
                size,
                scale,
                precision,
                sourceVersion,
                direction);

            Assert.AreEqual(parameterName, dbp.ParameterName);
            Assert.AreEqual(sourceColumn, dbp.SourceColumn);
            Assert.AreEqual(type, dbp.DbType);
            Assert.AreEqual(objectValue, dbp.Value);
            Assert.AreEqual(isNullable, dbp.IsNullable);
            Assert.AreEqual(size, dbp.Size);
            Assert.AreEqual(scale, dbp.Scale);
            Assert.AreEqual(precision, dbp.Precision);
            Assert.AreEqual(sourceVersion, dbp.SourceVersion);
            Assert.AreEqual(direction, dbp.Direction);
        }

        [TestMethod()]
        public void DbDataParameterTest_SimpleCtor()
        {
            DbDataParameter dbp = new DbDataParameter(parameterName, sourceColumn, type);

            Assert.AreEqual(parameterName, dbp.ParameterName);
            Assert.AreEqual(sourceColumn, dbp.SourceColumn);
            Assert.AreEqual(type, dbp.DbType);
        }

        [TestMethod()]
        public void DbDataParameterTest_DefaultCtor()
        {
            DbDataParameter dbp = new DbDataParameter();

            Assert.AreEqual(null, dbp.ParameterName);
            Assert.AreEqual(null, dbp.SourceColumn);
            Assert.AreEqual(DbType.Object, dbp.DbType);
        }
    }
}
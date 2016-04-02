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

namespace Almond.UnitTest.MySQLDrvier
{
    [TestClass]
    public class AsyncLineDriverUnitTest
    {
        private static AsyncLineDriver lineDriver;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ConnectionStringBuilder connectionStringBuilder = new ConnectionStringBuilder();
            connectionStringBuilder.ConnectionString = "hostname=localhost;port=3306";
            lineDriver = new AsyncLineDriver(connectionStringBuilder);
            Assert.IsNotNull(lineDriver);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            lineDriver.Close();
            lineDriver = null;
        }

        [TestMethod]
        public void Close()
        {
            lineDriver.Close();
        }
    }
}

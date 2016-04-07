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
using Almond.LineDriver;
using Almond.MySQLDriver;
using Almond.ProtocolDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Almond.UnitTest.ProtocolDriver
{
    [TestClass]
    public class ProtocolDriverUnitTest
    {
        private static ConnectionStringBuilder _connectionStringBuilder;
        private static Almond.ProtocolDriver.ProtocolDriver _protocolDriver;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _connectionStringBuilder = new ConnectionStringBuilder();
            _connectionStringBuilder.ConnectionString = "hostname=localhost;port=3306";
            _protocolDriver = new Almond.ProtocolDriver.ProtocolDriver(_connectionStringBuilder);
            Assert.IsNotNull(_protocolDriver);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _protocolDriver.Dispose();
            _protocolDriver = null;
        }

        [TestMethod]
        public void CreatePacket()
        {
            // COM_QUIT
            byte[] byteArray = new byte[] { 01, 00, 00, 01, 01 };
            ChunkReader chunkReader = new ChunkReader();
            chunkReader.AddChunk(new ArraySegment<byte>(byteArray, 0, byteArray.Length));
            IPacket result = _protocolDriver.CreatePacket(chunkReader);

            Assert.AreNotEqual(null, result);
            Assert.AreEqual(1, result.SequenceNumber);
            Assert.AreEqual(1, result.Length);
        }

        [TestMethod]
        public void Dispose()
        {
            _protocolDriver.Dispose();
        }
    }
}

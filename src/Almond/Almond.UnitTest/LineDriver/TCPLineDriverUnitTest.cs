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
using System.IO;

namespace Almond.UnitTest.LineDriver
{
    [TestClass]
    public class TCPLineDriverUnitTest : IPacketFactory
    {
        private static ConnectionStringBuilder _connectionStringBuilder;
        private static TCPLineDriver _lineDriver;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _connectionStringBuilder = new ConnectionStringBuilder();
            _connectionStringBuilder.ConnectionString = "hostname=localhost;port=3306";
            _lineDriver = new TCPLineDriver(_connectionStringBuilder);
            Assert.IsNotNull(_lineDriver);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _lineDriver.Dispose();
            _lineDriver = null;
        }

        [TestMethod]
        public void SoakTestConnection()
        {
            for (int i = 0; i < 100; i++)
            {
                TCPLineDriver connection = new TCPLineDriver(_connectionStringBuilder);
                connection.Dispose();
            }
        }

        [TestMethod]
        public void ReadPacket()
        {
            IPacket packet = _lineDriver.ReadPacket(this);
            Assert.AreNotEqual(null, packet);
            Assert.AreEqual(42, packet.SequenceNumber);
        }

        [TestMethod]
        public void Dispose()
        {
            _lineDriver.Dispose();
        }

        #region IPacketFactory
        public IPacket CreatePacket(BinaryReader packetHeader)
        {
            IPacket result = new GenericPacket();
            result.SequenceNumber = 42;
            result.Length = 0;
            return result;
        }
        #endregion
    }
}

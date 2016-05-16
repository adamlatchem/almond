using Almond.ProtocolDriver.Packets;
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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Almond.LineDriver;
using Almond.SQLDriver;

namespace Almond.ProtocolDriver.Packets.Tests
{
    [TestClass]
    public class ERRTests
    {
        #region Test Data
        static ProtocolDriver _moqDriver;

        ushort errorCode = 1234;
        string state = "TESTS";
        string errorMessage = "This is a test";

        private ArraySegment<Byte> MakePacket(ProtocolDriver driver, bool includeMessage, bool includeState)
        {
            ChunkWriter chunkWriter = new ChunkWriter();
            chunkWriter.NewChunk();
            chunkWriter.WriteMyInt1(1);
            chunkWriter.WriteMyInt2(errorCode);
            if (includeState)
            {
                chunkWriter.WriteByte((byte)'#');
                chunkWriter.WriteTextFix(state, driver.ClientEncoding);
            }
            if (includeMessage)
                chunkWriter.WriteTextFix(errorMessage, driver.ClientEncoding);
            ArraySegment<Byte> chunk = chunkWriter.ExportChunk();

            return chunk;
        }

        private IServerPacket MakeAndParsePacket(ERR packet, ProtocolDriver driver, bool includeMessage, bool includeState)
        {
            ChunkReader chunkReader = new ChunkReader();
            ArraySegment<Byte> chunk = MakePacket(_moqDriver, includeMessage, includeState);
            chunkReader.AddChunk(chunk);

            Assert.AreNotEqual(null, packet);
            IServerPacket p = packet.FromWireFormat(chunkReader, (uint)chunk.Count, _moqDriver);
            return p;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Create the objects to unit test
            ConnectionStringBuilder csb = new ConnectionStringBuilder();
            csb.ConnectionString = "hostname=localhost;username=test;password=test";
            _moqDriver = new ProtocolDriver(csb);
        }
        #endregion

        [TestMethod()]
        public void FromWireFormatTest()
        {
            ERR packet = new ERR();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver, true, true);

            Assert.AreEqual(packet, p);
            Assert.AreEqual(errorCode, packet.ErrorCode);
            Assert.AreEqual(errorMessage, packet.ErrorMessage);
            Assert.AreEqual(state, packet.SQLState);
        }

        [TestMethod()]
        public void FromWireFormatTest_NoSQLState()
        {
            ERR packet = new ERR();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver, true, false);

            Assert.AreEqual(packet, p);
            Assert.AreEqual(errorCode, packet.ErrorCode);
            Assert.AreEqual(errorMessage, packet.ErrorMessage);
            Assert.AreEqual(null, packet.SQLState);
        }

        [TestMethod()]
        public void FromWireFormatTest_NoMessageOrState()
        {
            ERR packet = new ERR();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver, false, false);

            Assert.AreEqual(packet, p);
            Assert.AreEqual(errorCode, packet.ErrorCode);
            Assert.AreEqual(null, packet.ErrorMessage);
            Assert.AreEqual(null, packet.SQLState);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            ERR packet = new ERR();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver, true, true);

            Assert.AreEqual(packet, p);
            Assert.AreEqual("Server Error 1234 : This is a test # TESTS", p.ToString());
        }

        [TestMethod()]
        public void ToStringTest_NoSQLState()
        {
            ERR packet = new ERR();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver, true, false);

            Assert.AreEqual(packet, p);
            Assert.AreEqual("Server Error 1234 : This is a test", p.ToString());
        }

        [TestMethod()]
        public void ToStringTest_NoMessageNoState()
        {
            ERR packet = new ERR();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver, false, false);

            Assert.AreEqual(packet, p);
            Assert.AreEqual("Server Error 1234", p.ToString());
        }
    }
}

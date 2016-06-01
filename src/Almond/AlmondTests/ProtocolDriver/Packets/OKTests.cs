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
using Almond.SQLDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Almond.ProtocolDriver.Packets.Tests
{
    [TestClass]
    public class OKTests
    {
        #region Test Data
        static ProtocolDriver _moqDriver;

        UInt64 affectedRows = 0x12ffffffffu;
        UInt64 lastInsertId = 0x12345678u;
        Status statusFlags = Status.SERVER_STATUS_METADATA_CHANGED;
        UInt16 numberOfWarnings = 666;
        string info = "This is some info";

        private ArraySegment<Byte> MakePacket(ProtocolDriver driver)
        {
            ChunkWriter chunkWriter = new ChunkWriter();
            chunkWriter.NewChunk();
            chunkWriter.WriteMyInt1(0);

            chunkWriter.WriteMyIntLenEnc(affectedRows);
            chunkWriter.WriteMyIntLenEnc(lastInsertId);
            chunkWriter.WriteMyInt2((UInt16)statusFlags);
            chunkWriter.WriteMyInt2((UInt16)numberOfWarnings);
            chunkWriter.WriteTextFix(info, driver.ClientEncoding);

            ArraySegment<Byte> chunk = chunkWriter.ExportChunk();

            return chunk;
        }

        private IServerPacket MakeAndParsePacket(OK packet, ProtocolDriver driver)
        {
            ChunkReader chunkReader = new ChunkReader();
            ArraySegment<Byte> chunk = MakePacket(driver);
            chunkReader.AddChunk(chunk);

            Assert.AreNotEqual(null, packet);
            IServerPacket p = packet.FromWireFormat(chunkReader, (uint)chunk.Count, driver);
            return p;
        }

        [ClassInitialize]
        static public void ClassInitialize(TestContext context)
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
            OK packet = new OK();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver);

            Assert.AreEqual(packet, p);
            Assert.AreEqual(affectedRows, packet.AffectedRows);
            Assert.AreEqual(lastInsertId, packet.LastInsertId);
            Assert.AreEqual(statusFlags, packet.StatusFlags);
            Assert.AreEqual(numberOfWarnings, packet.NumberOfWarnings);
            Assert.AreEqual(info, packet.Info);
        }
    }
}

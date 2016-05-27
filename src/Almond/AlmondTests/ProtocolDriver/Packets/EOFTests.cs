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
    [TestClass()]
    public class EOFTests
    {
        #region Test Data
        static ProtocolDriver _moqDriver;

        UInt16 numberOfWarnings = 7;
        Status statusFlags = Status.SERVER_MORE_RESULTS_EXISTS;

        private ArraySegment<Byte> MakePacket(ProtocolDriver driver)
        {
            ChunkWriter chunkWriter = new ChunkWriter();
            chunkWriter.NewChunk();
            chunkWriter.WriteMyInt1(0xFE);
            chunkWriter.WriteMyInt2(numberOfWarnings);
            chunkWriter.WriteMyInt2((UInt16)statusFlags);
            chunkWriter.Fill(2);
            ArraySegment<Byte> chunk = chunkWriter.ExportChunk();

            return chunk;
        }

        private IServerPacket MakeAndParsePacket(EOF packet, ProtocolDriver driver)
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
            EOF packet = new EOF();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver);

            Assert.AreEqual(packet, p);
            Assert.AreEqual(numberOfWarnings, packet.NumberOfWarnings);
            Assert.AreEqual(statusFlags, packet.StatusFlags);
        }
    }
}
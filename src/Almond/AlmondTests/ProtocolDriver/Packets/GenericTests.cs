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
    public class GenericTests
    {
        #region Test Data
        static ProtocolDriver _moqDriver;

        byte[] payload = new byte[] { 65, 66, 67, 68, 69, 70, 71, 72 };

        private ArraySegment<Byte> MakePacket(ProtocolDriver driver)
        {
            ChunkWriter chunkWriter = new ChunkWriter();
            chunkWriter.NewChunk();
            chunkWriter.WriteMyStringFix(payload, (UInt32)payload.Length);

            ArraySegment<Byte> chunk = chunkWriter.ExportChunk();

            return chunk;
        }

        private IServerPacket MakeAndParsePacket(Generic packet, ProtocolDriver driver)
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
            Generic packet = new Generic();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver);
            Assert.AreEqual(packet, p);
            Assert.AreEqual(_moqDriver.ClientEncoding, packet.ClientEncoding);
            Assert.AreEqual((UInt32)payload.Length, packet.PayloadLength);
            Assert.AreEqual("ABCDEFGH", packet.PayloadAsString);
            for (int i = 0; i < payload.Length; i++)
                Assert.AreEqual(payload[i], packet.Payload.Array[i]);
        }

        [TestMethod()]
        public void ToWireFormatTest()
        {
            ArraySegment<byte> segment = MakePacket(_moqDriver);
            byte[] expected = new byte[] {
                0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48,
            };
            Assert.AreEqual(expected.Length, segment.Count);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], segment.Array[i]);
        }
    }
}

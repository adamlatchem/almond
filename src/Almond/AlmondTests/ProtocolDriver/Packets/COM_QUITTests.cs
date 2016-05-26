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
    public class COM_QUITTests
    {
        #region Test Data
        static ProtocolDriver _moqDriver;

        private ArraySegment<Byte> MakePacket(ProtocolDriver driver)
        {
            COM_QUIT packet = new COM_QUIT();
            ChunkWriter chunkWriter = new ChunkWriter();
            chunkWriter.NewChunk();
            packet.ToWireFormat(chunkWriter, driver);
            ArraySegment<byte> chunk = chunkWriter.ExportChunk();

            return chunk;
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
        public void ToWireFormatTest()
        {
            ArraySegment<byte> segment = MakePacket(_moqDriver);
            byte[] expected = new byte[] { 1 };
            Assert.AreEqual(expected.Length, segment.Count);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], segment.Array[i]);
        }
    }
}
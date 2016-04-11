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
using System.Text;

namespace Almond.LineDriver.Tests
{
    [TestClass]
    public class ChunkWriterTests
    {
        private static ChunkWriter _chunkWriter;
        private static Encoding _encoding;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _chunkWriter = new ChunkWriter();
            _encoding = System.Text.ASCIIEncoding.ASCII;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _chunkWriter = null;
        }

        [TestMethod()]
        public void NewChunkTest()
        {
            _chunkWriter.NewChunk();
            Assert.AreEqual(0, _chunkWriter.WrittenSoFar());
        }

        [TestMethod()]
        public void WrittenSoFarTest()
        {
            _chunkWriter.ExportChunk();
            Assert.AreEqual(0, _chunkWriter.WrittenSoFar());
            _chunkWriter.NewChunk();
            Assert.AreEqual(0, _chunkWriter.WrittenSoFar());
        }

        [TestMethod()]
        public void ExportChunkTest()
        {
            ArraySegment<byte> value = _chunkWriter.ExportChunk();
            Assert.AreNotEqual(null, value.Array);
        }

        [TestMethod()]
        public void FillTest()
        {
            _chunkWriter.NewChunk();
            _chunkWriter.Fill(10);
            ArraySegment<byte> value = _chunkWriter.ExportChunk();
            Assert.AreEqual(10, value.Count);
            for (int i = 4; i < value.Count; i++)
                Assert.AreEqual(0, value.Array[i]);
        }

        [TestMethod()]
        public void WriteByteTest()
        {
            byte code = 56;
            _chunkWriter.NewChunk();
            _chunkWriter.WriteByte(code);
            ArraySegment<byte> value = _chunkWriter.ExportChunk();
            Assert.AreEqual(code, value.Array[0]);
        }

        [TestMethod()]
        public void WriteMyInt1Test()
        {
            byte code = 19;
            _chunkWriter.NewChunk();
            _chunkWriter.WriteMyInt1(code);
            ArraySegment<byte> value = _chunkWriter.ExportChunk();
            Assert.AreEqual(code, value.Array[0]);
        }

        [TestMethod()]
        public void WriteMyInt2Test()
        {
            UInt16 code = 1984;
            _chunkWriter.NewChunk();
            _chunkWriter.WriteMyInt2(code);
            ArraySegment<byte> value = _chunkWriter.ExportChunk();
            Assert.AreEqual((byte)0xC0, value.Array[0]);
            Assert.AreEqual((byte)0x07, value.Array[1]);
        }

        [TestMethod()]
        public void WriteMyInt3Test()
        {
            UInt32 code = 0xC0BEEF;
            _chunkWriter.NewChunk();
            _chunkWriter.WriteMyInt3(code);
            ArraySegment<byte> value = _chunkWriter.ExportChunk();
            Assert.AreEqual((byte)0xEF, value.Array[0]);
            Assert.AreEqual((byte)0xBE, value.Array[1]);
            Assert.AreEqual((byte)0xC0, value.Array[2]);
        }

        [TestMethod()]
        public void WriteMyInt4Test()
        {
            UInt32 code = 0x0BADCAFE;
            _chunkWriter.NewChunk();
            _chunkWriter.WriteMyInt4(code);
            ArraySegment<byte> value = _chunkWriter.ExportChunk();
            Assert.AreEqual((byte)0xFE, value.Array[0]);
            Assert.AreEqual((byte)0xCA, value.Array[1]);
            Assert.AreEqual((byte)0xAD, value.Array[2]);
            Assert.AreEqual((byte)0x0B, value.Array[3]);
        }

        [TestMethod()]
        public void WriteMyStringFixTest()
        {
            byte[] byteArray = new byte[] { 1, 2, 3, 4 };
            _chunkWriter.NewChunk();
            _chunkWriter.WriteMyStringFix(byteArray, 3);
            ArraySegment<byte> value = _chunkWriter.ExportChunk();
            Assert.AreEqual(3, value.Count);
            for (int i = 0; i < value.Count; ++i)
                Assert.AreEqual(byteArray[i], value.Array[i]);
        }

        [TestMethod()]
        public void WriteMyStringNullTest()
        {
            byte[] byteArray = new byte[] { 5, 6, 7, 8 };
            _chunkWriter.NewChunk();
            _chunkWriter.WriteMyStringNull(byteArray, 3);
            ArraySegment<byte> value = _chunkWriter.ExportChunk();
            Assert.AreEqual(4, value.Count);
            for (int i = 0; i < value.Count - 1; ++i)
                Assert.AreEqual(byteArray[i], value.Array[i]);
            Assert.AreEqual(0, value.Array[value.Count - 1]);
        }

        [TestMethod()]
        public void StringToBytesTest()
        {
            byte[] byteArray = ChunkWriter.StringToBytes("ABCD", _encoding);
            Assert.AreEqual(4, byteArray.Length);
            Assert.AreEqual(65, byteArray[0]);
            Assert.AreEqual(66, byteArray[1]);
            Assert.AreEqual(67, byteArray[2]);
            Assert.AreEqual(68, byteArray[3]);
        }

        [TestMethod()]
        public void WriteTextFixTest()
        {
            string testString = "EFGH";
            _chunkWriter.NewChunk();
            _chunkWriter.WriteTextFix(testString, _encoding);
            ArraySegment<byte> value = _chunkWriter.ExportChunk();
            Assert.AreEqual(4, value.Count);
            for (int i = 0; i < value.Count; ++i)
                Assert.AreEqual(69 + i, value.Array[i]);
        }

        [TestMethod()]
        public void WriteTextNullTest()
        {
            string testString = "ABABABAB";
            _chunkWriter.NewChunk();
            _chunkWriter.WriteTextNull(testString, _encoding);
            ArraySegment<byte> value = _chunkWriter.ExportChunk();
            Assert.AreEqual(9, value.Count);
            for (int i = 0; i < value.Count - 1; ++i)
                Assert.AreEqual(65 + (i % 2), value.Array[i]);
            Assert.AreEqual(0, value.Array[value.Count - 1]);
        }
    }
}

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
    public class ChunkReaderTests
    {
        private static byte[] _byteArray;
        private static ChunkReader _chunkReader;
        private static Encoding _encoding;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _chunkReader = new ChunkReader();
            _encoding = System.Text.ASCIIEncoding.ASCII;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _chunkReader = null;
            _byteArray = null;
        }

        [TestMethod()]
        public void StartNewPacketTest()
        {
            _chunkReader.StartNewPacket();
            UInt32 soFar = _chunkReader.ReadSoFar();
            Assert.AreEqual((UInt32)0, soFar);

            SetupPacket(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            soFar = _chunkReader.ReadSoFar();
            Assert.AreEqual((UInt32)0, soFar);

            _chunkReader.ReadMyStringFix(5);
            soFar = _chunkReader.ReadSoFar();
            Assert.AreEqual((UInt32)5, soFar);

            _chunkReader.StartNewPacket();

            soFar = _chunkReader.ReadSoFar();
            Assert.AreEqual((UInt32)0, soFar);

            _chunkReader.ReadMyStringFix(5);
            soFar = _chunkReader.ReadSoFar();
            Assert.AreEqual((UInt32)5, soFar);

            _byteArray = null;
        }

        private void SetupPacket(byte[] byteArray)
        {
            // Empty previous packet contents.
            if (_byteArray != null)
                _chunkReader.ReadMyStringEOF((UInt32)_byteArray.Length);

            _byteArray = byteArray;
            _chunkReader.StartNewPacket();
            _chunkReader.AddChunk(new ArraySegment<byte>(_byteArray));
        }

        [TestMethod()]
        public void AddChunkTest()
        {
            SetupPacket(new byte[] { 1 });
            byte value = _chunkReader.PeekByte();
            Assert.AreEqual(1, value);
        }

        [TestMethod()]
        public void SkipTest()
        {
            SetupPacket(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            _chunkReader.Skip(5);
            byte value = _chunkReader.PeekByte();
            Assert.AreEqual(6, value);
        }

        [TestMethod]
        public void PeekByteTest()
        {
            SetupPacket(new byte[] { 123 });
            byte value = _chunkReader.PeekByte();
            Assert.AreEqual(_byteArray[0], value);
        }

        [TestMethod()]
        public void ReadByteTest()
        {
            SetupPacket(_byteArray = new byte[] { 76 });
            byte value = _chunkReader.ReadByte();
            Assert.AreEqual(_byteArray[0], value);
        }

        [TestMethod]
        public void ReadMyInt1Test()
        {
            SetupPacket(new byte[] { 43 });
            byte value = _chunkReader.ReadMyInt1();
            Assert.AreEqual(_byteArray[0], value);
        }

        [TestMethod]
        public void ReadMyInt2Test()
        {
            SetupPacket(new byte[] { 2, 1 });
            UInt16 value = _chunkReader.ReadMyInt2();
            Assert.AreEqual(0x0102, value);
        }

        [TestMethod]
        public void ReadMyInt3Test()
        {
            SetupPacket(new byte[] { 1, 2, 3 });
            UInt32 value = _chunkReader.ReadMyInt3();
            Assert.AreEqual((UInt32)0x030201, value);
        }

        [TestMethod()]
        public void ReadMyInt4Test()
        {
            SetupPacket(new byte[] { 1, 2, 3, 4 });
            UInt32 value = _chunkReader.ReadMyInt4();
            Assert.AreEqual((UInt32)0x04030201, value);
        }

        [TestMethod()]
        public void ReadMyInt8Test()
        {
            SetupPacket(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            UInt64 value = _chunkReader.ReadMyInt8();
            Assert.AreEqual((UInt64)0x0807060504030201, value);
        }

        [TestMethod()]
        public void ReadMyIntLenEncTest()
        {
            SetupPacket(new byte[] { 1, 0xfc, 3, 4, 0xfd, 6, 7, 8, 9, 0xfe, 1, 2, 3, 4, 5, 6, 7, 8 });
            UInt64 value = _chunkReader.ReadMyIntLenEnc();
            Assert.AreEqual((UInt64)1, value);
            value = _chunkReader.ReadMyIntLenEnc();
            Assert.AreEqual((UInt64)0x0403, value);
            value = _chunkReader.ReadMyIntLenEnc();
            Assert.AreEqual((UInt64)0x09080706, value);
            value = _chunkReader.ReadMyIntLenEnc();
            Assert.AreEqual((UInt64)0x0807060504030201, value);
        }

        private void AssertArraysMatch<T>(T[] expected, ArraySegment<T> actual)
        {
            Assert.AreEqual(expected.Length, actual.Count);
            for (int i = 0; i < expected.Length; ++i)
                Assert.AreEqual(expected[i], actual.Array[actual.Offset + i]);
        }

        private void AssertArraysMatch<T>(T[] expected, ArraySegment<T> actual, int uptoIndex)
        {
            Assert.IsTrue(expected.Length >= actual.Count);
            for (int i = 0; i <= uptoIndex; ++i)
                Assert.AreEqual(expected[i], actual.Array[actual.Offset + i]);
        }

        [TestMethod]
        public void ReadMyStringFixTest()
        {
            SetupPacket(new byte[] { 1, 2, 3, 4 });
            ArraySegment<byte> value = _chunkReader.ReadMyStringFix((UInt32)_byteArray.Length);
            AssertArraysMatch<byte>(_byteArray, value);
        }

        [TestMethod()]
        public void ReadMyStringNullTest()
        {
            SetupPacket(new byte[] { 4, 5, 6, 7, 0, 1 });
            ArraySegment<byte> value = _chunkReader.ReadMyStringNull();
            AssertArraysMatch<byte>(_byteArray, value, 3);
        }

        [TestMethod()]
        public void ReadMyStringLenEncTest()
        {
            SetupPacket(new byte[] { 1, 1, 2, 2, 2 });
            ArraySegment<byte> value = _chunkReader.ReadMyStringLenEnc();
            Assert.AreEqual(1, value.Count);
            Assert.AreEqual(1, value.Array[value.Offset + 0]);
            value = _chunkReader.ReadMyStringLenEnc();
            Assert.AreEqual(2, value.Count);
            Assert.AreEqual(2, value.Array[value.Offset + 0]);
            Assert.AreEqual(2, value.Array[value.Offset + 1]);
        }

        [TestMethod()]
        public void ReadMyStringEOFTest0()
        {
            SetupPacket(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            UInt32 skipCount = 4;
            _chunkReader.Skip(skipCount);
            ArraySegment<byte> value = _chunkReader.ReadMyStringEOF((UInt32)_byteArray.Length);
            Assert.AreEqual(_byteArray.Length - skipCount, value.Count);

            SetupPacket(new byte[] { 1 });
            _chunkReader.ReadMyInt1();
            value = _chunkReader.ReadMyStringEOF((UInt32)_byteArray.Length);
            Assert.IsTrue(value != null);
            Assert.AreEqual(0, value.Count);
        }

        [ExpectedException(typeof(LineDriverException))]
        [TestMethod()]
        public void ReadMyStringEOFTest1()
        {
            SetupPacket(new byte[] { 1, 2, 3, 4 });
            _chunkReader.Skip(3);
            ArraySegment<byte> value = _chunkReader.ReadMyStringEOF(2);
            Assert.Fail();
        }

        [TestMethod()]
        public void BytesToStringTest()
        {
            byte[] byteArray = _encoding.GetBytes("hello");
            string value = ChunkReader.BytesToString(new ArraySegment<byte>(byteArray), _encoding);
            Assert.AreEqual("hello", value);
        }

        [TestMethod()]
        public void ReadTextFixTest()
        {
            SetupPacket(_encoding.GetBytes("testing"));
            string value = _chunkReader.ReadTextFix(4, _encoding);
            Assert.AreEqual("test", value);
        }

        [TestMethod()]
        public void ReadTextNullTest()
        {
            SetupPacket(_encoding.GetBytes("TEST\0"));
            string value = _chunkReader.ReadTextNull(_encoding);
            Assert.AreEqual("TEST", value);
        }

        [TestMethod()]
        public void ReadTextLenEncTest()
        {
            SetupPacket(new byte[] { 5, (byte)'w', (byte)'o', (byte)'r', (byte)'k', (byte)'s' });
            string value = _chunkReader.ReadTextLenEnc(_encoding);
            Assert.AreEqual("works", value);
        }

        [TestMethod()]
        public void ReadTextEOFTest()
        {
            SetupPacket(new byte[] { 65, 66, 67, 68, 69, 70, 71, 72 });
            UInt32 skipCount = 4;
            _chunkReader.Skip(skipCount);
            string value = _chunkReader.ReadTextEOF((UInt32)_byteArray.Length, _encoding);
            Assert.AreEqual("EFGH", value);
        }

        [TestMethod()]
        public void ChunkReaderTest()
        {
            ChunkReader test = new ChunkReader(new ArraySegment<byte>(new byte[] { 65, 65, 65, 65 }));
            string value = test.ReadTextEOF(4, _encoding);
            Assert.AreEqual("AAAA", value);
        }
    }
}

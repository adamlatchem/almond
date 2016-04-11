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

namespace Almond.LineDriver.Tests
{
    [TestClass]
    public class TCPLineDriverTests
    {
        private static ConnectionStringBuilder _connectionStringBuilder;
        private static TCPLineDriver _lineDriver;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _connectionStringBuilder = new ConnectionStringBuilder();
            _connectionStringBuilder.ConnectionString = "hostname=localhost;port=3306";
            _lineDriver = new TCPLineDriver(_connectionStringBuilder);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _lineDriver.Dispose();
            _lineDriver = null;
        }

        [TestMethod]
        public void ChunkReaderTest()
        {
            ChunkReader chunkReader = _lineDriver.ChunkReader;
            Assert.AreNotEqual(null, chunkReader);
        }

        [TestMethod]
        public void ChunkWriterTest()
        {
            ChunkWriter chunkWriter = _lineDriver.ChunkWriter;
            Assert.AreNotEqual(null, chunkWriter);
        }

        [TestMethod]
        public void DisposeTest()
        {
            _lineDriver.Dispose();
            _lineDriver = new TCPLineDriver(_connectionStringBuilder);
        }

        [TestMethod()]
        public void SendChunkTest()
        {
            _lineDriver.SendChunk();
        }

        [TestMethod()]
        public void TCPLineDriverTest()
        {
            _lineDriver = new TCPLineDriver(_connectionStringBuilder);
            Assert.IsNotNull(_lineDriver);
        }
    }
}

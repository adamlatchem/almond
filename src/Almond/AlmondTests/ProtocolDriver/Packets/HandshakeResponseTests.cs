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
using System.Collections.Generic;

namespace Almond.ProtocolDriver.Packets.Tests
{
    [TestClass]
    public class HandshakeResponseTests
    {
        #region Test Data
        static ProtocolDriver _moqDriver;

        uint maxPacketSize = 1234;
        byte characterSet = 15;
        string username = "test";
        string database = "TestDatabase";

        private ArraySegment<Byte> MakePacket(ProtocolDriver driver, Capability capability)
        {
            driver.ClientCapability = capability;

            HandshakeResponse packet = new HandshakeResponse();
            packet.MaxPacketSize = maxPacketSize;
            packet.CharacterSet = characterSet;
            packet.Username = username;
            if (capability.HasFlag(Capability.CLIENT_SECURE_CONNECTION))
            {
                ServerHandshake hs = new ServerHandshake();
                hs.AuthPluginName = "mysql_native_password";
                hs.AuthPluginData = new List<byte>()
                    {1,  2,  3,  4,  5,  6,  7,  8,  9, 10,
                    11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                    21};
                hs.LengthOfAuthPluginData = (byte)hs.AuthPluginData.Count;
                packet.MySQLNativePassword("TestAuthResponse", hs, driver.ClientEncoding);
            }
            if (capability.HasFlag(Capability.CLIENT_CONNECT_WITH_DB))
                packet.Database = database;

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
        public void ToWireFormatTest_SimplestPacket()
        {
            ArraySegment<byte> segment = MakePacket(_moqDriver, 0);
            byte[] expected = new byte[] {
                0x00, 0x00, 0x00, 0x00, 0xD2, 0x04, 0x00, 0x00, 0x0F, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00,
            };
            Assert.AreEqual(expected.Length, segment.Count);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], segment.Array[i]);
        }

        [TestMethod()]
        public void ToWireFormatTest_SecureConnection()
        {
            ArraySegment<byte> segment = MakePacket(_moqDriver, Capability.CLIENT_SECURE_CONNECTION);
            byte[] expected = new byte[] {
                0x00, 0x80, 0x00, 0x00, 0xD2, 0x04, 0x00, 0x00, 0x0F, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x74, 0x65, 0x73, 0x74, 0x00, 0x14, 0x2D, 0x01,
                0x48, 0x68, 0x23, 0x5B, 0x16, 0xDF, 0xF6, 0xCE, 0xC1, 0x42,
                0xAD, 0xD2, 0xBE, 0xA3, 0x20, 0xDB, 0xC3, 0xB2,
            };
            Assert.AreEqual(expected.Length, segment.Count);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], segment.Array[i]);
        }

        [TestMethod()]
        public void ToWireFormatTest_Database()
        {
            ArraySegment<byte> segment = MakePacket(_moqDriver, Capability.CLIENT_CONNECT_WITH_DB);
            byte[] expected = new byte[] {
                0x08, 0x00, 0x00, 0x00, 0xD2, 0x04, 0x00, 0x00, 0x0F, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x54, 0x65,
                0x73, 0x74, 0x44, 0x61, 0x74, 0x61, 0x62, 0x61, 0x73, 0x65,
                0x00,
            };
            Assert.AreEqual(expected.Length, segment.Count);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], segment.Array[i]);
        }


        [TestMethod()]
        public void MySQLNativePasswordTest()
        {
            HandshakeResponse packet = new HandshakeResponse();

            ServerHandshake hs = new ServerHandshake();
            hs.AuthPluginName = "mysql_native_password";
            hs.AuthPluginData = new List<byte>()
                    {1,  2,  3,  4,  5,  6,  7,  8,  9, 10,
                    11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                    21};
            hs.LengthOfAuthPluginData = (byte)hs.AuthPluginData.Count;

            packet.MySQLNativePassword("TestAuthResponse", hs, _moqDriver.ClientEncoding);

            byte[] expected = new byte[] {
                0x2D, 0x01, 0x48, 0x68, 0x23, 0x5B, 0x16, 0xDF, 0xF6, 0xCE,
                0xC1, 0x42, 0xAD, 0xD2, 0xBE, 0xA3, 0x20, 0xDB, 0xC3, 0xB2,
            };
            Assert.AreEqual(expected.Length, packet.AuthResponse.Length);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], packet.AuthResponse[i]);
        }
    }
}

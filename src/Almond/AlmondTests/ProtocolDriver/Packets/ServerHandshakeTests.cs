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
    public class ServerHandshakeTests
    {
        #region Test Data
        static ProtocolDriver _moqDriver;

        string serverVersion = "Test1.0";
        UInt32 connectionThreadId = 0x314156;
        byte[] authPluginData = new byte[] {
            (byte)'a', (byte)'u', (byte)'t', (byte)'h',
            (byte)'d', (byte)'a', (byte)'t', (byte)'a' };
        Capability capability = Capability.CLIENT_PLUGIN_AUTH;
        byte characterSet = 24;
        Status statusFlag = Status.SERVER_MORE_RESULTS_EXISTS;
        string authPluginName = "testAuthPlugin";
        byte[] secureAuthPluginData = new byte[] {
            (byte)'s', (byte)'e', (byte)'c', (byte)'u',
            (byte)'r', (byte)'e', (byte)'!', (byte)'!' };

        private ArraySegment<Byte> MakePacket(ProtocolDriver driver, bool includeStatus, bool includeSecurePlugin)
        {
            if (includeSecurePlugin)
                capability = capability | Capability.CLIENT_SECURE_CONNECTION;
            else
                secureAuthPluginData = new byte[0];

            ChunkWriter chunkWriter = new ChunkWriter();
            chunkWriter.NewChunk();
            chunkWriter.WriteMyInt1(10);
            chunkWriter.WriteTextNull(serverVersion, System.Text.Encoding.UTF8);
            chunkWriter.WriteMyInt4(connectionThreadId);
            chunkWriter.WriteMyStringNull(authPluginData, 8);
            chunkWriter.WriteMyInt2((UInt16)capability);

            if (includeStatus || includeSecurePlugin)
            {
                chunkWriter.WriteByte(characterSet);
                chunkWriter.WriteMyInt2((UInt16)statusFlag);
                chunkWriter.WriteMyInt2((UInt16)(((UInt32)capability & 0xFFFF0000u) >> 16));
                chunkWriter.WriteMyInt1((byte)secureAuthPluginData.Length);
                chunkWriter.Fill(10);

                if (includeSecurePlugin)
                {
                    chunkWriter.WriteMyStringFix(secureAuthPluginData, (uint)secureAuthPluginData.Length);
                    if (secureAuthPluginData.Length < 13)
                        chunkWriter.Fill((uint)(13 - secureAuthPluginData.Length));
                }

                chunkWriter.WriteTextNull(authPluginName, driver.ClientEncoding);
            }

            ArraySegment <Byte> chunk = chunkWriter.ExportChunk();

            return chunk;
        }

        private IServerPacket MakeAndParsePacket(ServerHandshake packet, ProtocolDriver driver, bool includeStatus, bool includeSecurePlugin)
        {
            ChunkReader chunkReader = new ChunkReader();
            ArraySegment<Byte> chunk = MakePacket(driver, includeStatus, includeSecurePlugin);
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
            ServerHandshake packet = new ServerHandshake();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver, false, false);

            Assert.AreEqual(packet, p);
            Assert.AreEqual(serverVersion, packet.ServerVersion);
            Assert.AreEqual(connectionThreadId, packet.ConnectionThreadId);
            Assert.AreEqual(8, packet.AuthPluginData.Count);
            for (int i = 0; i < authPluginData.Length; i++)
                Assert.AreEqual(authPluginData[i], packet.AuthPluginData[i]);

            // Only first 16bits are encoded in this packet format
            Assert.AreEqual((Capability)((UInt32)capability & 0xFFFFu), packet.Capabilities);
        }

        [TestMethod()]
        public void FromWireFormatTest_Status()
        {
            ServerHandshake packet = new ServerHandshake();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver, true, false);

            Assert.AreEqual(packet, p);
            Assert.AreEqual(serverVersion, packet.ServerVersion);
            Assert.AreEqual(connectionThreadId, packet.ConnectionThreadId);
            Assert.AreEqual(8, packet.AuthPluginData.Count);
            for (int i = 0; i < authPluginData.Length; i++)
                Assert.AreEqual(authPluginData[i], packet.AuthPluginData[i]);
            Assert.AreEqual(capability, packet.Capabilities);
            Assert.AreEqual(characterSet, packet.CharacterSet);
            Assert.AreEqual(statusFlag, packet.StatusFlags);
            Assert.AreEqual(authPluginName, packet.AuthPluginName);
        }

        [TestMethod()]
        public void FromWireFormatTest_StatusAndSecure()
        {
            ServerHandshake packet = new ServerHandshake();
            IServerPacket p = MakeAndParsePacket(packet, _moqDriver, true, true);

            Assert.AreEqual(packet, p);
            Assert.AreEqual(serverVersion, packet.ServerVersion);
            Assert.AreEqual(connectionThreadId, packet.ConnectionThreadId);
            Assert.AreEqual(21, packet.AuthPluginData.Count);
            for (int i = 0; i < authPluginData.Length; i++)
                Assert.AreEqual(authPluginData[i], packet.AuthPluginData[i]);
            for (int i = 0; i < secureAuthPluginData.Length; i++)
                Assert.AreEqual(secureAuthPluginData[i], packet.AuthPluginData[i + authPluginData.Length]);
            Assert.AreEqual(capability, packet.Capabilities);
            Assert.AreEqual(characterSet, packet.CharacterSet);
            Assert.AreEqual(statusFlag, packet.StatusFlags);
            Assert.AreEqual(authPluginName, packet.AuthPluginName);
        }
    }
}

﻿#region License
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
using System.Text;

namespace Almond.ProtocolDriver.Packets
{
    /// <summary>
    /// Interface for client packets - allows packets to be sent by
    /// the line driver. Packets are created as data chunks sent to
    /// the network.
    /// </summary>
    public interface IClientPacket
    {
        /// <summary>
        /// Write a packet to a chunk for sending on the wire. The sequence number
        /// for the header is passed in and the header must also be written to the
        /// chunk.
        /// </summary>
        /// <param name="writer">the chunkwriter to build chunk for wire</param>
        /// <param name="driver">Driver - includes client capabiity and default encoding</param>
        void ToWireFormat(ChunkWriter writer, ProtocolDriver driver);
    }
}

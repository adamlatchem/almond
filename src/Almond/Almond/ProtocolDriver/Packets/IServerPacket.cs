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
using System;
using System.Text;

namespace Almond.ProtocolDriver.Packets
{
    /// <summary>
    /// Interface for server packets - allows packets to be received by
    /// the line driver. Packets are created incrementally as data chunks
    /// arrive via buffers.
    /// </summary>
    public interface IServerPacket
    {
        /// <summary>
        /// Read a packet from the reader - the header fields have already been parsed
        /// the reader is positioned after the packet header ready for further parsing.
        /// </summary>
        /// <param name="reader">The reader of the chunk stream</param>
        /// <param name="payloadLength">length of payload excludes header</param>
        /// <param name="clientCapability">servers capacilities as determined at the time of parsing</param>
        /// <param name="clientEncoding">The client encoding for Text strings</param>
        void FromReader(ChunkReader reader, UInt32 payloadLength, Capability clientCapability, Encoding clientEncoding);
    }
}

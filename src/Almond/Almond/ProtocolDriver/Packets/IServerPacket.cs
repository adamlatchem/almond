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

namespace Almond.ProtocolDriver.Packets
{
    /// <summary>
    /// Interface for server packets - allows packets to be parsed from
    /// the line driver wire format to .NET objects. Packets are created
    /// incrementally as data chunks arrive via buffers in the ChunkReader.
    /// The Packet may be meta - it can return itself or any type that it
    /// determines internally should be used to represent the wire data.
    /// </summary>
    public interface IServerPacket
    {
        /// <summary>
        /// Read a packet from wire format chunks - the header fields have already been parsed
        /// the reader is positioned after the packet header ready for further parsing.
        /// </summary>
        /// <param name="reader">The reader of the chunk stream</param>
        /// <param name="payloadLength">length of payload excludes header</param>
        /// <param name="driver">The driver defining client capability and default encoding</param>
        /// <returns>An IServerPacket that was read - this allows factory packets to return different concrete types</returns>
        IServerPacket FromWireFormat(ChunkReader reader, UInt32 payloadLength, ProtocolDriver driver);
    }
}

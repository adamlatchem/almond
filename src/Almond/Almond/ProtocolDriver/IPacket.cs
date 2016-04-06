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
using System;
using System.IO;

namespace Almond.ProtocolDriver
{
    /// <summary>
    /// Packets have optional functionality based on available capabilities
    /// which are desrcibed below.
    /// </summary>
    [Flags]
    public enum Capability
    {
        CLIENT_PROTOCOL_41 = 1
    }

    /// <summary>
    /// Interface for packets - allows packets to be sent/received by
    /// the line driver. Packets are created incrementally as data chunks
    /// arrive via buffers.
    /// </summary>
    public interface IPacket
    {
        #region Header
        /// <summary>
        /// Get/set the sequence number
        /// </summary>
        Int32 SequenceNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the size of the packet payload (excludes the header fields)
        /// </summary>
        Int32 Length
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Incrementally read the packet from a buffer.
        /// </summary>
        /// <returns>is packet complete</returns>
        /// <param name="buffer"></param>
        bool FromReader(BinaryReader buffer, Capability capabilities);

        /// <summary>
        /// Incrementally write packet to a buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>is packet complete</returns>
        bool ToWriter(BinaryWriter buffer, Capability capabilities);
    }
}

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
using System.IO;

namespace Almond.LineDriver
{
    /// <summary>
    /// Enables the line driver to create correct packet.
    /// </summary>
    public interface IPacketFactory
    {
        /// <summary>
        /// Given a packet header return an IPacket of the correct type
        /// </summary>
        /// <param name="packetHeader"></param>
        /// <returns></returns>
        IPacket CreatePacket(BinaryReader packetHeader);
    }
}

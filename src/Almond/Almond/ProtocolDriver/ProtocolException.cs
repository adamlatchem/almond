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
using Almond.ProtocolDriver.Packets;
using System;

namespace Almond.ProtocolDriver
{
    /// <summary>
    /// Represents an error encountered in the Client/Server protocol
    /// </summary>
    public class ProtocolException : Exception
    {
        public ProtocolException(string message) : 
            base(message)
        { }

        public ProtocolException(string message, Exception innerException) : 
            base(message, innerException)
        { }

        public ProtocolException(ERR errorPacket) :
            base(errorPacket.ToString())
        { }
    }
}

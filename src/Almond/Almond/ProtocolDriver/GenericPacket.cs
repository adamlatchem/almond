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
using System.IO;

namespace Almond.ProtocolDriver
{
    /// <summary>
    /// Will count data upto expected length from the buffer. Useful as
    /// a placeholder for unknown packets in the packet stream.
    /// </summary>
    public class GenericPacket : IPacket
    {
        #region IPacket
        public Int32 Length
        {
            get; set;
        }

        public Int32 SequenceNumber
        {
            get; set;
        }

        public void FromReader(ChunkReader reader, Capability capabilities)
        {
            reader.ReadMyStringFix(Length);
        }

        public bool ToWriter(BinaryWriter buffer, Capability capabilities)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

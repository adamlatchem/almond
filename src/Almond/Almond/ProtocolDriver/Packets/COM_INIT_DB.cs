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

namespace Almond.ProtocolDriver.Packets
{
    public class COM_INIT_DB : IClientPacket
    {
        #region Members
        public string Database
        {
            get; set;
        }
        #endregion

        #region IClientPacket
        public void ToWireFormat(ChunkWriter writer, ProtocolDriver driver)
        {
            writer.WriteMyInt1(2);
            writer.WriteTextFix(Database, driver.ClientEncoding);
        }
        #endregion
    }
}

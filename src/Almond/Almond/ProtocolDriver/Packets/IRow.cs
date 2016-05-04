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
using System.Collections.Generic;
using System.Text;

namespace Almond.ProtocolDriver.Packets
{
    /// <summary>
    /// The interface required by objects that convert wireformat messages
    /// continaing row data into .NET objects.
    /// </summary>
    public interface IRow
    {
        /// <summary>
        /// Used to access the raw data for each column of the result.
        /// </summary>
        IList<ArraySegment<byte>> Values
        {
            get;
        }

        /// <summary>
        /// Decode given column to a string value or null
        /// </summary>
        /// <param name="i"></param>
        /// <param name="encodingOverride"></param>
        /// <returns></returns>
        string StringValue(int i, Encoding encodingOverride);
    }
}

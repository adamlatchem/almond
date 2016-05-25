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
using System.Text;

namespace Almond
{
    public class Tools
    {
        /// <summary>
        /// Dump an array segment suitable for inclusion as a raw byte array in code.
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        static public string DumpHexArray(ArraySegment<byte> segment)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < segment.Count; i++)
            {
                if (i % 10 == 0)
                    sb.Append(System.Environment.NewLine);
                sb.AppendFormat("0x{0:X2}, ", segment.Array[i]);
            }

            return sb.ToString();
        }
    }
}

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

namespace Almond.LineDriver
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Peek a single byte from a BinaryReader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static byte PeekByte(this BinaryReader reader)
        {
            byte result = reader.ReadByte();
            reader.BaseStream.Position -= 1;
            return result;
        }

        /// <summary>
        /// Read a MySQL int<3> value
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Int32 ReadMyInt3(this BinaryReader reader)
        {
            Int32 result = reader.ReadByte();
            result |= (reader.ReadByte() << 8);
            result |= (reader.ReadByte() << 16);
            return result;
        }

        /// <summary>
        /// Read a MySQL int<2> value
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Int32 ReadMyInt2(this BinaryReader reader)
        {
            Int32 result = reader.ReadByte();
            result |= (reader.ReadByte() << 8);
            return result;
        }

        /// <summary>
        /// Read a MySQL int<1> value
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static byte ReadMyInt1(this BinaryReader reader)
        {
            return reader.ReadByte();
        }

        /// <summary>
        /// Incrementally read a MySQL string<fix> value.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="length"></param>
        /// <param name="readSoFar"></param>
        /// <returns></returns>
        public static byte[] ReadMyStringFix(this BinaryReader reader, Int32 length, ref Int32 readSoFar)
        {
            byte[] result = reader.ReadBytes(length - readSoFar);
            readSoFar += result.Length;
            return result;
        }
    }
}

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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Almond.LineDriver
{
    /// <summary>
    /// A class to write data from packets to chunks that will be sent on the wire.
    /// </summary>
    public class ChunkWriter
    {
        #region Members
        /// <summary>
        /// Used by the thread reading as the current chunk being processed.
        /// </summary>
        private List<byte> _currentChunk;
        #endregion

        /// <summary>
        /// Prepare a new chunk for writing
        /// </summary>
        public void NewChunk()
        {
            _currentChunk = new List<byte>();
        }

        /// <summary>
        /// Return length of the chunk so far.
        /// </summary>
        /// <returns></returns>
        public int WrittenSoFar()
        {
            if (_currentChunk == null)
                return 0;
            return _currentChunk.Count;
        }

        /// <summary>
        /// The chunk is complete so detach it and return as an ArraySegment.
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> ExportChunk()
        {
            if (_currentChunk == null)
                return new ArraySegment<byte>(new byte[0]);
            byte[] chunk = _currentChunk.ToArray();
            _currentChunk = null;
            return new ArraySegment<byte>(chunk);
        }

        #region Primitives
        /// <summary>
        /// Write a single byte to the chunk and advance the position.
        /// </summary>
        /// <param name="value">the byte to write</param>
        public void WriteByte(byte value)
        {
            _currentChunk.Add(value);
        }

        /// <summary>
        /// Write a MySQL string<fix> value.
        /// </summary>
        /// <param name="value">The byte[] to write in string<fix> format</param>
        /// <param name="length">The length of the byte sequence to write</param>
        public void WriteMyStringFix(byte[] value, UInt32 length)
        {
            if (length > value.Length)
                throw new LineDriverException("Fixed length string is missing trailing values");
            for (int i = 0; i < length; ++i)
                _currentChunk.Add(value[i]);
        }
        #endregion

        #region Compounds
        /// <summary>
        /// Fill bytes in the stream with 0's
        /// </summary>
        /// <param name="count"></param>
        public void Fill(UInt32 count)
        {
            byte[] zeros = new byte[count];
            WriteMyStringFix(zeros, count);
        }

        /// <summary>
        /// Write a MySQL int<1> value
        /// </summary>
        /// <param name="value">The byte to write</param>
        public void WriteMyInt1(byte value)
        {
            WriteByte(value);
        }

        /// <summary>
        /// Write a MySQL int<2> value
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteMyInt2(UInt16 value)
        {
            WriteByte((byte) (value & 0x00ff));
            WriteByte((byte)((value & 0xff00) >> 8));
        }

        /// <summary>
        /// Write a MySQL int<3> value
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteMyInt3(UInt32 value)
        {
            WriteByte((byte) (value & 0x0000ff));
            WriteByte((byte)((value & 0x00ff00) >> 8));
            WriteByte((byte)((value & 0xff0000) >> 16));
        }

        /// <summary>
        /// Write a MySQL int<3> value at a given offset
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <param name="offset">offset to write value to</param>
        public void WriteMyInt3(UInt32 value, int offset)
        {
            _currentChunk[offset++] = (byte)(value & 0x0000ff);
            _currentChunk[offset++] = (byte)((value & 0x00ff00) >> 8);
            _currentChunk[offset++] = (byte)((value & 0xff0000) >> 16);
        }

        /// <summary>
        /// Write a MySQL int<4> value
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteMyInt4(UInt32 value)
        {
            WriteByte((byte) (value & 0x000000ff));
            WriteByte((byte)((value & 0x0000ff00) >> 8));
            WriteByte((byte)((value & 0x00ff0000) >> 16));
            WriteByte((byte)((value & 0xff000000) >> 24));
        }

        /// <summary>
        /// Write a null terminated byte array
        /// </summary>
        /// <returns></returns>
        public void WriteMyStringNull(byte[] value, UInt32 count)
        {
            WriteMyStringFix(value, count);
            WriteByte(0);
        }

        /// <summary>
        /// Convert (null) string to a (empty) byte array.
        /// </summary>
        /// <param name="theString">The string to convert to an array</param>
        /// <param name="encoding">Text encoding to use</param>
        /// <returns>A byte array of exact length for the string</returns>
        public static byte[] StringToBytes(string theString, Encoding encoding)
        {
            if (theString == null)
                return new byte[0];
            return encoding.GetBytes(theString);
        }

        /// <summary>
        /// Return a MyStringFix as a string
        /// </summary>
        /// <param name="theString">string to write</param>
        /// <param name="encoding">Encoding of text to use</param>
        /// <returns></returns>
        public void WriteTextFix(string theString, Encoding encoding)
        {
            byte[] bytes = StringToBytes(theString, encoding);
            int length = bytes.Length;
            WriteMyStringFix(bytes, (UInt32)length);
        }

        /// <summary>
        /// Write a string as a MyStringNull
        /// </summary>
        /// <param name="theString">string to write</param>
        /// <param name="encoding">Encoding of text to use</param>
        /// <returns></returns>
        public void WriteTextNull(string theString, Encoding encoding)
        {
            byte[] bytes = StringToBytes(theString, encoding);
            int length = bytes.Length;
            WriteMyStringNull(bytes, (UInt32)length);
        }
        #endregion
    }
}

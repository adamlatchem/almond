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

namespace Almond.LineDriver
{
    /// <summary>
    /// A class to read data from chunks as they are made available from the wire.
    /// </summary>
    public class ChunkReader
    {
        #region Members
        /// <summary>
        /// Once a byte[] arrives it is placed in this queue.
        /// This allows a second thread to block until data is available.
        /// </summary>
        private BlockingCollection<ArraySegment<byte>> _queue;

        /// <summary>
        /// Used by the thread reading as the current chunk being processed.
        /// </summary>
        private ArraySegment<byte> _currentChunk;

        /// <summary>
        /// Position in the _currentChunk - technically a uint however to save unnecessary
        /// casting throughout the code use an int.
        /// </summary>
        private int _position;

        /// <summary>
        /// Keeps running total of total amount of data read in previous chunks
        /// </summary>
        private UInt32 _previousChunks;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public ChunkReader()
        {
            _queue = new BlockingCollection<ArraySegment<byte>>();
        }

        /// <summary>
        /// Reset the total byte count and abandon the current chunk.
        /// </summary>
        public void StartNewPacket()
        {
            _currentChunk = new ArraySegment<byte>(new byte[0]);
            _previousChunks = 0;
        }

        /// <summary>
        /// Returns the total number of bytes read to the current position.
        /// 
        /// internal for unit testing
        /// </summary>
        /// <returns></returns>
        internal UInt32 ReadSoFar()
        {
            return _previousChunks + (UInt32)_position;
        }

        /// <summary>
        /// Add a chunk to the reader - waking any thread waiting on this data
        /// to arrive.
        /// </summary>
        /// <param name="chunk"></param>
        public void AddChunk(ArraySegment<byte> chunk)
        {
            _queue.Add(chunk);
        }

        #region Primitives
        /// <summary>
        /// Ensures there is a current chunk and position set - will
        /// block if the queue is waiting for a chunk.
        /// </summary>
        private void AdvanceCurrentChunk()
        {
            if (_currentChunk.Offset + _currentChunk.Count <= _position)
            {
                _previousChunks += (UInt32)_currentChunk.Count;
                _currentChunk = _queue.Take();
                _position = _currentChunk.Offset;
            }
        }

        /// <summary>
        /// Read a single byte from the chunk stream and advance the position.
        /// </summary>
        /// <returns>byte read</returns>
        public byte ReadByte()
        {
            AdvanceCurrentChunk();
            return _currentChunk.Array[_position++];
        }

        /// <summary>
        /// Peek a single byte from the chunk stream.
        /// </summary>
        /// <returns>byte read</returns>
        public byte PeekByte()
        {
            AdvanceCurrentChunk();
            return _currentChunk.Array[_position];
        }

        /// <summary>
        /// Read a MySQL string<fix> value.
        /// </summary>
        /// <param name="count"></param>
        /// <returns>value read</returns>
        public byte[] ReadMyStringFix(UInt32 count)
        {
            byte[] array = new byte[count];
            long position = 0;
            while (position < count)
            {
                AdvanceCurrentChunk();
                long length = Math.Min(_currentChunk.Count - _position, count-position);

                Array.Copy(_currentChunk.Array, _position, array, position, length);

                _position += (int)(length);
                position += length;
            }
            return array;
        }

        /// <summary>
        /// Skip bytes in the stream
        /// </summary>
        /// <param name="count"></param>
        public void Skip(UInt32 count)
        {
            long position = 0;
            while (position < count)
            {
                AdvanceCurrentChunk();
                long length = Math.Min(_currentChunk.Count - _position, count - position);
                _position += (int)(length);
                position += length;
            }
        }
        #endregion

        #region Compounds
        public UInt64 ReadMyIntLenEnc()
        {
            byte len = ReadMyInt1();
            if (len < 251)
                return len;
            else if (len == 0xFC)
                return ReadMyInt2();
            else if (len == 0xFD)
                return ReadMyInt4();
            else if (len == 0xFE)
                return ReadMyInt8();
            throw new LineDriverException("Unknown Int<LenEnc> value");
        }

        /// <summary>
        /// Read a MySQL int<1> value
        /// </summary>
        /// <returns>value read</returns>
        public byte ReadMyInt1()
        {
            return ReadByte();
        }

        /// <summary>
        /// Read a MySQL int<2> value
        /// </summary>
        /// <returns>value read</returns>
        public UInt16 ReadMyInt2()
        {
            UInt16 result = ReadMyInt1();
            result |= (UInt16)(ReadMyInt1() << 8);
            return result;
        }

        /// <summary>
        /// Read a MySQL int<3> value
        /// </summary>
        /// <returns>value read</returns>
        public UInt32 ReadMyInt3()
        {
            UInt32 result = ReadMyInt1();
            result |= ((UInt32)ReadMyInt1() << 8);
            result |= ((UInt32)ReadMyInt1() << 16);
            return result;
        }

        /// <summary>
        /// Read a MySQL int<4> value
        /// </summary>
        /// <returns>value read</returns>
        public UInt32 ReadMyInt4()
        {
            UInt32 result = ReadMyInt1();
            result |= ((UInt32)ReadMyInt1() << 8);
            result |= ((UInt32)ReadMyInt1() << 16);
            result |= ((UInt32)ReadMyInt1() << 24);
            return result;
        }

        /// <summary>
        /// Read a MySQL int<4> value
        /// </summary>
        /// <returns>value read</returns>
        public UInt64 ReadMyInt8()
        {
            UInt64 result = ReadMyInt1();
            result |= ((UInt64)ReadMyInt1() << 8);
            result |= ((UInt64)ReadMyInt1() << 16);
            result |= ((UInt64)ReadMyInt1() << 24);
            result |= ((UInt64)ReadMyInt1() << 32);
            result |= ((UInt64)ReadMyInt1() << 40);
            result |= ((UInt64)ReadMyInt1() << 48);
            result |= ((UInt64)ReadMyInt1() << 56);
            return result;
        }

        /// <summary>
        /// Extract a null terminated byte array
        /// </summary>
        /// <returns></returns>
        public byte[] ReadMyStringNull()
        {
            List<byte> result = new List<byte>();
            while (true)
            {
                byte next = ReadByte();
                if (next == 0)
                    break;
                result.Add(next);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Return a length encoded byte string
        /// </summary>
        /// <returns></returns>
        public byte[] ReadMyStringLenEnc()
        {
            UInt64 length = ReadMyIntLenEnc();
            return ReadMyStringFix((UInt32)length);
        }

        /// <summary>
        /// Return a rest of packet byte string.
        /// </summary>
        /// <param name="packetLength">Total length of packet</param>
        /// <returns></returns>
        public byte[] ReadMyStringEOF(UInt32 packetLength)
        {
            return ReadMyStringFix(packetLength - ReadSoFar());
        }

        /// <summary>
        /// Convert byte array to a string
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="encoding">Encoding of text to use</param>
        /// <returns>String that was read</returns>
        public static string BytesToString(byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Return a MyStringFix as a string
        /// </summary>
        /// <param name="length"></param>
        /// <param name="encoding">Encoding of text to use</param>
        /// <returns>String that was read</returns>
        public string ReadTextFix(UInt32 length, Encoding encoding)
        {
            return BytesToString(ReadMyStringFix(length), encoding);
        }

        /// <summary>
        /// Return a MyStringNull as a string
        /// </summary>
        /// <param name="encoding">Encoding of text to use</param>
        /// <returns>String that was read</returns>
        public string ReadTextNull(Encoding encoding)
        {
            return BytesToString(ReadMyStringNull(), encoding);
        }

        /// <summary>
        /// Return a MyStringLenEnc as a string.
        /// </summary>
        /// <param name="encoding">Encoding of text to use</param>
        /// <returns>String that was read</returns>
        public string ReadTextLenEnc(Encoding encoding)
        {
            return BytesToString(ReadMyStringLenEnc(), encoding);
        }

        /// <summary>
        /// Return a MyStringEOF as a string.
        /// </summary>
        /// <param name="encoding">Encoding of text to use</param>
        /// <returns>String that was read</returns>
        public string ReadTextEOF(UInt32 packetLength, Encoding encoding)
        {
            return BytesToString(ReadMyStringEOF(packetLength), encoding);
        }
        #endregion
    }
}

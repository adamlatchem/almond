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
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public ChunkReader()
        {
            _queue = new BlockingCollection<ArraySegment<byte>>();
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
            if (_currentChunk.Array == null || _currentChunk.Offset + _currentChunk.Count <= _position)
            {
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
        public UInt32 ReadMyInt2()
        {
            UInt32 result = ReadByte();
            result |= ((UInt32)ReadByte() << 8);
            return result;
        }

        /// <summary>
        /// Read a MySQL int<3> value
        /// </summary>
        /// <returns>value read</returns>
        public UInt32 ReadMyInt3()
        {
            UInt32 result = ReadByte();
            result |= ((UInt32)ReadByte() << 8);
            result |= ((UInt32)ReadByte() << 16);
            return result;
        }

        /// <summary>
        /// Read a MySQL int<4> value
        /// </summary>
        /// <returns>value read</returns>
        public UInt32 ReadMyInt4()
        {
            UInt32 result = this.ReadByte();
            result |= ((UInt32)ReadByte() << 8);
            result |= ((UInt32)ReadByte() << 16);
            result |= ((UInt32)ReadByte() << 24);
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
        /// Convert byte array to a string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string BytesToString(byte[] bytes)
        {
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// Return a MyStringFix as a string
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string ReadStringFix(UInt32 length)
        {
            return BytesToString(ReadMyStringFix(length));
        }

        /// <summary>
        /// Return a MyStringNull as a string
        /// </summary>
        /// <returns></returns>
        public string ReadStringNull()
        {
            return BytesToString(ReadMyStringNull());
        }
        #endregion
    }
}

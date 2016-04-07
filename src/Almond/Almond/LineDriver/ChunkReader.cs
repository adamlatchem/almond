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
        /// Position in the _currentChunk
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
            if (_currentChunk.Array == null || _currentChunk.Offset + _currentChunk.Count < _position)
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
        #endregion

        #region Compounds
        /// <summary>
        /// Read a MySQL int<1> value
        /// </summary>
        /// <returns>value read</returns>
        public byte ReadMyInt1()
        {
            return this.ReadByte();
        }

        /// <summary>
        /// Read a MySQL int<2> value
        /// </summary>
        /// <returns>value read</returns>
        public Int32 ReadMyInt2()
        {
            Int32 result = this.ReadByte();
            result |= (this.ReadByte() << 8);
            return result;
        }

        /// <summary>
        /// Read a MySQL int<3> value
        /// </summary>
        /// <returns>value read</returns>
        public Int32 ReadMyInt3()
        {
            Int32 result = this.ReadByte();
            result |= (this.ReadByte() << 8);
            result |= (this.ReadByte() << 16);
            return result;
        }

        /// <summary>
        /// Read a MySQL string<fix> value.
        /// </summary>
        /// <param name="length"></param>
        /// <returns>value read</returns>
        public byte[] ReadMyStringFix(Int32 length)
        {
            byte[] array = new byte[length];
            for (int i = 0; i < length; ++i)
                array[i] = ReadByte();
            return array;
        }
        #endregion
    }
}

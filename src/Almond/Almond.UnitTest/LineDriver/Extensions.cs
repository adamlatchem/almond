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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Almond.UnitTest.LineDriver
{
    [TestClass]
    public class Extensions
    {
        private static byte[] _byteArray;
        private static MemoryStream _memoryStream;
        private static BinaryReader _binaryReader;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _byteArray = new byte[] { };
            _memoryStream = new MemoryStream(_byteArray);
            _binaryReader = new BinaryReader(_memoryStream);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _binaryReader.Dispose();
            _binaryReader = null;
            _memoryStream.Dispose();
            _memoryStream = null;
            _byteArray = null;
        }

        [TestMethod]
        public void PeekByte()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ReadMyInt1()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ReadMyInt2()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ReadMyInt3()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ReadMyStringFix()
        {
            throw new NotImplementedException();
        }
    }
}

﻿#region License
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
using Almond.ProtocolDriver.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Almond.ProtocolDriver.Tests
{
    [TestClass]
    public class ProtocolExceptionTests
    {
        [TestMethod]
        public void CtorTest()
        {
            ProtocolException exception = new ProtocolException("this is a test 1");
            Assert.IsNotNull(exception);

            exception = new ProtocolException("this is a test 2", new InvalidCastException());
            Assert.IsNotNull(exception);

            exception = new ProtocolException(new ERR());
            Assert.IsNotNull(exception);
        }
    }
}

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
using System.Data.Common;

namespace Almond.MySQLDriver
{
    /// <summary>
    /// Type safe connection string builder for MySQL connections.
    /// </summary>
    public class ConnectionStringBuilder : DbConnectionStringBuilder
    {
        /// <summary>
        /// The host we want to connect with
        /// </summary>
        public string Hostname
        {
            get { return (string)this["hostname"]; }
            set { this["hostname"] = value; }
        }

        /// <summary>
        /// The TCP port number we want to connect with
        /// </summary>
        public int Port
        {
            get { return int.Parse((string)this["port"]); }
            set { this["port"] = value; }
        }
    }
}

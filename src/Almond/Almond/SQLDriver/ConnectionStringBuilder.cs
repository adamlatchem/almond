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
using System.Data.Common;

namespace Almond.SQLDriver
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
            get
            {
                if (!this.ContainsKey("hostname"))
                    return "localhost";
                return (string)this["hostname"];
            }
            set { this["hostname"] = value; }
        }

        /// <summary>
        /// The TCP port number we want to connect with
        /// </summary>
        public int Port
        {
            get
            {
                if (!this.ContainsKey("port"))
                    return 3306;
                return int.Parse((string)this["port"]);
            }
            set { this["port"] = value; }
        }

        /// <summary>
        /// The maximum packet size supported by the client.
        /// </summary>
        public UInt32 MaxPacketSize
        {
            get
            {
                if (!this.ContainsKey("maxpacketsize"))
                    return (UInt32)Math.Pow(2, 24) - 1;
                return UInt32.Parse((string)this["maxpacketsize"]);
            }
            set { this["maxpacketsize"] = value; }
        }

        /// <summary>
        /// The username to login to the server as
        /// </summary>
        public string Username
        {
            get
            {
                if (!this.ContainsKey("username"))
                    throw new SQLDriverException("Connection string must include a username");
                return (string)this["username"];
            }
            set { this["username"] = value; }
        }

        /// <summary>
        /// The password to login to the server as
        /// </summary>
        public string Password
        {
            get
            {
                if (!this.ContainsKey("password"))
                    throw new SQLDriverException("Connection string must include a password");
                return (string)this["password"];
            }
            set { this["password"] = value; }
        }

        /// <summary>
        /// The Database we want to connect to or empty
        /// </summary>
        public string Database
        {
            get
            {
                if (!this.ContainsKey("database"))
                    return string.Empty;
                return (string)this["database"];
            }
            set { this["database"] = value; }
        }

        /// <summary>
        /// The connection timeout in seconds.
        /// </summary>
        public UInt32 ConnectionTimeout
        {
            get
            {
                if (!this.ContainsKey("connectiontimeout"))
                    return 30;
                return UInt32.Parse((string)this["connectiontimeout"]);
            }
            set { this["connectiontimeout"] = value; }
        }
    }
}

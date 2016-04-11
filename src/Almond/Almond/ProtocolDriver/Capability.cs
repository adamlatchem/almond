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

namespace Almond.ProtocolDriver
{
    /// <summary>
    /// Packets have optional functionality based on available capabilities
    /// which are desrcibed below.
    /// </summary>
    [Flags]
    public enum Capability : UInt32
    {
        CLIENT_LONG_PASSWORD                  = 0x00000001,
        CLIENT_FOUND_ROWS                     = 0x00000002,
        CLIENT_LONG_FLAG                      = 0x00000004,
        CLIENT_CONNECT_WITH_DB                = 0x00000008,
        CLIENT_NO_SCHEMA                      = 0x00000010,
        CLIENT_COMPRESS                       = 0x00000020,
        CLIENT_ODBC                           = 0x00000040,
        CLIENT_LOCAL_FILES                    = 0x00000080,
        CLIENT_IGNORE_SPACE                   = 0x00000100,
        CLIENT_PROTOCOL_41                    = 0x00000200,
        CLIENT_INTERACTIVE                    = 0x00000400,
        CLIENT_SSL                            = 0x00000800,
        CLIENT_IGNORE_SIGPIPE                 = 0x00001000,
        CLIENT_TRANSACTIONS                   = 0x00002000,
        CLIENT_RESERVED                       = 0x00004000,
        CLIENT_SECURE_CONNECTION              = 0x00008000,
        CLIENT_MULTI_STATEMENTS               = 0x00010000,
        CLIENT_MULTI_RESULTS                  = 0x00020000,
        CLIENT_PS_MULTI_RESULTS               = 0x00040000,
        CLIENT_PLUGIN_AUTH                    = 0x00080000,
        CLIENT_CONNECT_ATTRS                  = 0x00100000,
        CLIENT_PLUGIN_AUTH_LENENC_CLIENT_DATA = 0x00200000,
        CLIENT_CAN_HANDLE_EXPIRED_PASSWORDS   = 0x00400000,
        CLIENT_SESSION_TRACK                  = 0x00800000,
        CLIENT_DEPRECATE_EOF                  = 0x01000000,
        CLIENT_UNKNOWN                        = 0x80000000,
    }
}

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
    [Flags]
    public enum Flags : UInt32
    {
        NOT_NULL_FLAG           = 1,            /* Field can't be NULL */
        PRI_KEY_FLAG            = 2,            /* Field is part of a primary key */
        UNIQUE_KEY_FLAG         = 4,            /* Field is part of a unique key */
        MULTIPLE_KEY_FLAG       = 8,            /* Field is part of a key */
        BLOB_FLAG               = 16,           /* Field is a blob */
        UNSIGNED_FLAG           = 32,           /* Field is unsigned */
        ZEROFILL_FLAG           = 64,           /* Field is zerofill */
        BINARY_FLAG             = 128,          /* Field is binary   */

        /* The following are only sent to new clients */
        ENUM_FLAG               = 256,          /* field is an enum */
        AUTO_INCREMENT_FLAG     = 512,          /* field is a autoincrement field */
        TIMESTAMP_FLAG          = 1024,         /* Field is a timestamp */
        SET_FLAG                = 2048,         /* field is a set */
        NO_DEFAULT_VALUE_FLAG   = 4096,         /* Field doesn't have default value */
        ON_UPDATE_NOW_FLAG      = 8192,         /* Field is set to NOW on UPDATE */
        NUM_FLAG                = 32768,        /* Field is num (for clients) */
    }
}

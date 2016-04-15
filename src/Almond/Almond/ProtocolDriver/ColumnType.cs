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

namespace Almond.ProtocolDriver
{
    public enum ColumnType
    {
        MYSQL_TYPE_DECIMAL     = 0x00,
        MYSQL_TYPE_TINY        = 0x01,
        MYSQL_TYPE_SHORT       = 0x02,
        MYSQL_TYPE_LONG        = 0x03,
        MYSQL_TYPE_FLOAT       = 0x04,
        MYSQL_TYPE_DOUBLE      = 0x05,
        MYSQL_TYPE_NULL        = 0x06,
        MYSQL_TYPE_TIMESTAMP   = 0x07,
        MYSQL_TYPE_LONGLONG    = 0x08,
        MYSQL_TYPE_INT24       = 0x09,
        MYSQL_TYPE_DATE        = 0x0A,
        MYSQL_TYPE_TIME        = 0x0B,
        MYSQL_TYPE_DATETIME    = 0x0C,
        MYSQL_TYPE_YEAR        = 0x0D,
        MYSQL_TYPE_NEWDATE     = 0x0E,
        MYSQL_TYPE_VARCHAR     = 0x0F,
        MYSQL_TYPE_BIT         = 0x10,
        MYSQL_TYPE_TIMESTAMP2  = 0x11,
        MYSQL_TYPE_DATETIME2   = 0x12,
        MYSQL_TYPE_TIME2       = 0x13,
        MYSQL_TYPE_NEWDECIMAL  = 0xF6,
        MYSQL_TYPE_ENUM        = 0xF7,
        MYSQL_TYPE_SET         = 0xF8,
        MYSQL_TYPE_TINY_BLOB   = 0xF9,
        MYSQL_TYPE_MEDIUM_BLOB = 0xFA,
        MYSQL_TYPE_LONG_BLOB   = 0xFB,
        MYSQL_TYPE_BLOB        = 0xFC,
        MYSQL_TYPE_VAR_STRING  = 0xFD,
        MYSQL_TYPE_STRING      = 0xFE,
        MYSQL_TYPE_GEOMETRY    = 0xFF,
    }
}

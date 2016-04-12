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
using Almond.SQLDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace Almond.IntegrationTests
{
    [TestClass]
    public class BasicFeatureTests
    {
        #region Connection Strings
        private const string CONNECTION_STRING =
            "Hostname=localhost;Port=3306;Username=test;Password=test;Initial Catalog=Integration";

        private const string IPv4_CONNECTION_STRING =
            "Hostname=127.0.0.1;Port=3306;Username=test;Password=test";

        private const string IPv6_CONNECTION_STRING =
            "Hostname=::1;Port=3306;Username=test;Password=test";
        #endregion

        [TestMethod]
        public void ConnectionTest()
        {
            using (Connection connection = new Connection(CONNECTION_STRING))
            {
                connection.Open();
            }
        }

        [TestMethod]
        public void IPv4ConnectionTest()
        {
            using (Connection connection =
                new Connection(IPv4_CONNECTION_STRING))
            {
                connection.Open();
            }
        }

        [TestMethod]
        public void IPv6ConnectionTest()
        {
            using (Connection connection =
                new Connection(IPv6_CONNECTION_STRING))
            {
                connection.Open();
            }
        }

        [TestMethod]
        public void SelectStatementTest()
        {
            string queryString = "SELECT Value FROM Integration.BasicFeatures";

            using (Connection connection =
                new Connection(CONNECTION_STRING))
            {
                Command command = new Command(queryString, connection);

                connection.Open();
                IDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("\t{0}", reader[0]);
                }
                reader.Close();
            }
        }
    }
}
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
using Almond.MySQLDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace Almond.IntegrationTest
{
    [TestClass]
    public class BasicFeatures
    {
        /// <summary>
        /// Connection string for the server to run tests against
        /// </summary>
        private const string connectionString =
                "Data Source=localhost;Initial Catalog=Integration;";

        [TestMethod]
        public void TestConnection()
        {
            using (Connection connection =
                new Connection(connectionString))
            {
                connection.Open();
            }
        }

        [TestMethod]
        public void TestSelectStatement()
        {
            string queryString = "SELECT Value FROM Integration.BasicFeatures";

            using (Connection connection =
                new Connection(connectionString))
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

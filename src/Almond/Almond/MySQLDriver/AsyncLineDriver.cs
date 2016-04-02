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
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Almond.MySQLDriver
{
    /// <summary>
    /// A class used to make the TCP connection to a MySQL server using asynchronous methods.
    /// </summary>
    public class AsyncLineDriver
    {
        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone = new ManualResetEvent(false);

        private String response = String.Empty;

        private Socket serverSocket;

        private IPEndPoint serverEndPoint;

        /// <summary>
        /// Constructor - connects to the given connection string using Hostname and Port
        /// </summary>
        /// <param name="connectionStringBuilder"></param>
        public AsyncLineDriver(ConnectionStringBuilder connectionStringBuilder)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(connectionStringBuilder.Hostname);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            serverEndPoint = new IPEndPoint(ipAddress, connectionStringBuilder.Port);

            serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.IP);
            serverSocket.BeginConnect(serverEndPoint,
                new AsyncCallback(ConnectCallback), serverSocket);

            connectDone.WaitOne();
        }

        /// <summary>
        /// Close the connection. This object is now of no further use.
        /// </summary>
        public void Close()
        {
            if (serverSocket != null)
            {
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();
                serverSocket.Dispose();
                serverSocket = null;
            }
        }

        /// <summary>
        /// Called when the line driver connects to the end point. Signals the connectDone MRE.
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            Debug.Assert(serverSocket == (Socket)ar.AsyncState);
            serverSocket.EndConnect(ar);

            // Signal that the connection has been made.
            connectDone.Set();
        }
    }
}
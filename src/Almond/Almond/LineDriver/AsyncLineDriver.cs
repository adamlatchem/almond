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
using Almond.MySQLDriver;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Almond.LineDriver
{
    /// <summary>
    /// The Line Driver connects using TCP to the MySQL server and transfers byte arrays
    /// from packets to the server and converts bytes arrays from the server to packets.
    /// </summary>
    public class AsyncLineDriver : IDisposable
    {
        #region Members
        /// <summary>
        /// Signalled when the connection is succesfully made.
        /// </summary>
        private static ManualResetEvent _connectedSignal = new ManualResetEvent(false);

        /// <summary>
        /// The end point we connect with.
        /// </summary>
        private IPEndPoint _serverEndPoint;

        /// <summary>
        /// The socket used for communication with the server. If this is null it also
        /// indicates that the line driver has been disposed.
        /// </summary>
        private Socket _serverSocket;

        /// <summary>
        /// Size of the receive buffer.
        /// </summary>
        private int _receiveBufferSize = 64 * 1024;

        /// <summary>
        /// Once a byte[] arrives it is placed in this queue.
        /// </summary>
        private BlockingCollection<MemoryStream> _receiveQueue;
        #endregion

        /// <summary>
        /// Constructor - connects to the given connection string using Hostname and Port,
        /// initiates reading and returns synchronously.
        /// </summary>
        /// <param name="connectionStringBuilder"></param>
        public AsyncLineDriver(ConnectionStringBuilder connectionStringBuilder)
        {
            _connectedSignal.Reset();
            _receiveQueue = new BlockingCollection<MemoryStream>();

            IPHostEntry ipHostInfo = Dns.GetHostEntry(connectionStringBuilder.Hostname);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            _serverEndPoint = new IPEndPoint(ipAddress, connectionStringBuilder.Port);

            _serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.IP);
            _serverSocket.BeginConnect(_serverEndPoint,
                new AsyncCallback(OnConnect), _serverSocket);

            _connectedSignal.WaitOne();
        }

        /// <summary>
        /// Dispose the connection. The line driver is of no further use.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (_serverSocket != null)
                {
                    _serverSocket.Shutdown(SocketShutdown.Both);
                    _serverSocket.Close();
                    _serverSocket.Dispose();
                    _serverSocket = null;
                }
            }
        }

        /// <summary>
        /// Called when the line driver connects to the end point. Initiates reading and
        /// signals the connectedSignal MRE.
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnect(IAsyncResult ar)
        {
            lock (this)
            {
                Debug.Assert(_serverSocket == (Socket)ar.AsyncState);
                _serverSocket.EndConnect(ar);

                BeginReceive_Locked();

                _connectedSignal.Set();
            }
        }

        /// <summary>
        /// Receives socket data, the received buffer is passed to the incremental packet generator.
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceive(IAsyncResult ar)
        {
            lock (this)
            {
                if (_serverSocket == null)
                    return;

                byte[] receivedBuffer = (byte[])ar.AsyncState;
                int bytesRecieved = _serverSocket.EndReceive(ar);
                _receiveQueue.Add(new MemoryStream(receivedBuffer, 0, bytesRecieved));

                BeginReceive_Locked();
            }
        }

        /// <summary>
        /// Requests data from the socket. Assumes we already are inside a lock(this).
        /// </summary>
        private void BeginReceive_Locked()
        {
            byte[] receiveBuffer = new byte[_receiveBufferSize];
            _serverSocket.BeginReceive(receiveBuffer, 0, _receiveBufferSize, SocketFlags.None, OnReceive, receiveBuffer);
        }
    }
}
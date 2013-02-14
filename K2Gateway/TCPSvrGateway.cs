//-----------------------------------------------------------------------
// <copyright file="TCPSvrGateway.cs" company="KaiTrade LLC">
// Copyright (c) 2013, KaiTrade LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// <author>John Unwin</author>
// <website>https://github.com/junwin/K2RTD.git</website>
//-----------------------------------------------------------------------
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace K2Gateway
{
    public delegate void OnMessageEvent(KaiMessageWrap myMsg);
    public delegate void OnStatusEvent(KaiMessageWrap myMsg);

    /// <summary>
    /// Channel error level
    /// </summary>
    public enum ErrorLevel
    {
        normal, recoverableError, error
    }

    /// <summary>
    /// Channel state
    /// </summary>
    public enum State
    {
        opening, open, closed, closing
    }

    /// <summary>
    /// provide a gatway based on Async TCP Sockets - a receiver gateway
    /// for sockets is bi directional and tracks the various sockets
    /// that have connected i.e. it acts as a server to N clients
    /// </summary>
    public class TCPSvrGateway
    {
        /// <summary>
        /// delegates we event when a message is received
        /// </summary>
        private OnMessageEvent m_Receiver;

        /// <summary>
        /// delegates we event when a status message is raised
        /// </summary>
        private OnStatusEvent m_Status;

        /// <summary>
        /// current state of the gateway
        /// </summary>
        private State m_State;

        /// <summary>
        /// Error level - these can occur at any state
        /// </summary>
        private ErrorLevel m_ErrorLevel;

        // Thread signal.
        public  ManualResetEvent m_AllDone;

        /// <summary>
        /// Port we listen on
        /// </summary>
        private int m_Port;

        /// <summary>
        /// Can we begin receiving
        /// </summary>
        //NOT USED? private bool m_Begin = false;

        /// <summary>
        /// main thread that the server listens for connections on
        /// </summary>
        System.Threading.Thread m_SvrThread;

        // Create a logger for use in this class
        //public log4net.ILog m_Log;

        /// <summary>
        /// Create a logger to record low level details - a wire log
        /// </summary>
        //public log4net.ILog m_WireLog;

        /// <summary>
        /// Set of handler(client) sockets attached
        /// </summary>
        System.Collections.Hashtable m_Handlers;

        /// <summary>
        /// thread that monitors status
        /// </summary>
        System.Threading.Thread m_WatchDog;

        /// <summary>
        /// Should the watchdog thread be running true => yes
        /// </summary>
        private bool m_RunWD;

        /// <summary>
        /// Well known listner socket
        /// </summary>
        Socket m_Listener;

        public TCPSvrGateway(int myPort)
        {
            m_AllDone = new ManualResetEvent(false);
            m_Port = myPort;
            //m_Log = log4net.LogManager.GetLogger("Kaitrade");
            //m_WireLog = log4net.LogManager.GetLogger("KaiTradeWireLog");
            //m_Log.Info("SocketReceiverGateWay - construction");
            m_Handlers = new System.Collections.Hashtable();
        }

        private void setStatus(State myState, ErrorLevel myErrorLevel)
        {
            m_State = myState;
            m_ErrorLevel = myErrorLevel;
            SetStatus( myState, "");
        }

        private void setStatus(State myState, ErrorLevel myErrorLevel, string myCorrelationID)
        {
            m_State = myState;
            m_ErrorLevel = myErrorLevel;
            SetStatus(myState, myCorrelationID);
        }

        /// <summary>
        /// Inform the client of a status change, if a correlationID(EndPoint) is
        /// specified that status relates to a particular connection.
        /// </summary>
        /// <param name="myState"></param>
        /// <param name="myCorrelationID">socket connecton affected , empty string => all</param>
        private void SetStatus(State myState, string myCorrelationID)
        {
            m_State = myState;

            // construct a kai message wrapper
            KaiMessageWrap myWrap = new KaiMessageWrap();
            myWrap.Data = myCorrelationID;
            myWrap.CorrelationID = myCorrelationID;

            switch (m_State)
            {
                case State.closed:
                    myWrap.Label = "CLOSED";
                    break;
                case State.closing:
                    myWrap.Label = "CLOSING";
                    break;
                case State.open:
                    myWrap.Label = "OPEN";
                    break;
                case State.opening:
                    myWrap.Label = "OPENING";
                    break;
                default:
                    myWrap.Label = "NOTKNOWN";
                    break;
            }

            if (m_Status != null)
            {
                m_Status(myWrap);
            }
        }

        /// <summary>
        /// Start listening for incomming connectins on our well known port
        /// </summary>
        public  void StartListening()
        {
            // Data buffer for incoming data.
            //byte[] bytes = new Byte[32768];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer

            string myHostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.Resolve(myHostName);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, m_Port);

            // Create a TCP/IP socket.
            m_Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                m_Listener.Bind(localEndPoint);
                m_Listener.Listen(100);
                setStatus(State.open, ErrorLevel.normal);
                while (true)
                {
                    System.Threading.Thread.Sleep(200);
                    // Set the event to nonsignaled state.
                    m_AllDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    //m_Log.Info("Waiting for a connection...");

                    m_Listener.BeginAccept(new AsyncCallback(AcceptCallback), m_Listener);

                    // Wait until a connection is made before continuing.
                    m_AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                //m_Log.Error("StartListening:", e);
                setStatus(State.open, ErrorLevel.recoverableError);
            }
        }

        /// <summary>
        /// Called when someone connects via sockets
        /// </summary>
        /// <param name="ar"></param>
        public  void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // Signal the main thread to continue.
                m_AllDone.Set();

                // Get the socket that handles the client request.
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = handler;
                string myRemoteEP = handler.RemoteEndPoint.ToString();
                setStatus(State.open, ErrorLevel.normal, myRemoteEP);
                //if (m_WireLog.IsInfoEnabled)
                //{
                //    m_WireLog.Info("GWSvrAcceptCallback:" + myRemoteEP);
                //}
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception myE)
            {
                //m_Log.Error("AcceptCallback", myE);

                setStatus(State.open, ErrorLevel.recoverableError);
            }
        }

        /// <summary>
        /// keep track of all the client sockets - indexed by their endpoints
        /// </summary>
        /// <param name="myHandler"></param>
        private void addhandler(ref Socket myHandler)
        {
            // get the remote endpoint, we will use this when routing messages back
            string myRemoteEP = myHandler.RemoteEndPoint.ToString();
            if (m_Handlers.ContainsKey(myRemoteEP))
            {
                // already exits so replace
                m_Handlers[myRemoteEP] = myHandler;
            }
            else
            {
                m_Handlers.Add(myRemoteEP, myHandler);
            }
        }

        /// <summary>
        /// Remove the socket from the handlers list
        /// </summary>
        /// <param name="myHandler"></param>
        private void removeHandler(ref Socket myHandler)
        {
            // get the remote endpoint, we will use this when routing messages back
            string myRemoteEP = myHandler.RemoteEndPoint.ToString();
            if (m_Handlers.ContainsKey(myRemoteEP))
            {
                // already exits so remove
                m_Handlers.Remove(myRemoteEP);

                // attempt to close the socket
                myHandler.Close();
            }
        }

        /// <summary>
        /// Callback for each socket connection - called when data arrives
        /// </summary>
        /// <param name="ar"></param>
        public  void ReadCallback(IAsyncResult ar)
        {
            Socket handler = null;
            string myRemoteEP="";
            try
            {
                String myContent = String.Empty;

                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                handler = state.workSocket;

                addhandler(ref handler);
                // get the remote endpoint, we will use this when routing messages back
                myRemoteEP = handler.RemoteEndPoint.ToString();

                // Read data from the client socket.
                int bytesRead = handler.EndReceive(ar);
                //if (m_WireLog.IsInfoEnabled)
                //{
                //    m_WireLog.Info("GWSvrBytesRead:" + bytesRead.ToString());
                //}
                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read
                    // more data.

                    while (state.sb.Length > 0)
                    {
                        myContent = state.sb.ToString();
                        long totLength;
                        string myMsg;
                        string myLabel;
                        string myCleintID;
                        totLength = StreamHelper.GetMessage(ref myContent, out myMsg, out myLabel, out myCleintID);
                        if (totLength > -1)
                        {
                            //if (m_WireLog.IsInfoEnabled)
                            //{
                            //    m_WireLog.Info("GWSvrRead:" + myMsg.Length.ToString() + "bytes :" + myMsg);
                            //}

                            // remove the message from the buffer
                            state.sb.Remove(0, (int)totLength);

                            EventGatewayClients(ref myMsg, myLabel, ref handler);
                        }
                        else
                        {
                            break;
                        }
                    }

                    // get the next message
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
            catch (Exception myE)
            {
                //m_Log.Error("ReadCallback", myE);
                setStatus(State.open, ErrorLevel.recoverableError);
                try
                {
                    if (handler != null)
                    {
                        setStatus(State.closed, ErrorLevel.error, myRemoteEP);
                        removeHandler(ref handler);
                        handler.Close();
                    }
                }
                catch (Exception myE2)
                {
                    //m_Log.Error("ReadCallback:handler execption process", myE2);
                    setStatus(State.closed, ErrorLevel.error, myRemoteEP);
                }
                //setStatus(State.closed, ErrorLevel.normal);
            }
        }

        private void EventGatewayClients(ref string myContent, string myLabel, ref Socket myHandler)
        {
            try
            {
                // get my end point
                string myRemoteEP = myHandler.RemoteEndPoint.ToString();

                // construct an kai msg wrapper
                KaiMessageWrap myWrap = new KaiMessageWrap();
                myWrap.Data = myContent;
                myWrap.Label = myLabel;

                // Use the Corrlation ID to store the endpoint
                // all return messages with that correlationID will
                // be sent down the relevant socket
                myWrap.CorrelationID = myRemoteEP;

                // event our gateway clients
                m_Receiver(myWrap);
            }
            catch (Exception myE)
            {
                //m_Log.Error("EventGatewayClients", myE);
                setStatus(State.open, ErrorLevel.recoverableError);
            }
        }

        public void Send(ref KaiMessageWrap myMsg)
        {
            try
            {
                // try to get the socket from the map - using the correlationID
                if (myMsg.CorrelationID.Length > 0)
                {
                    if (m_Handlers.ContainsKey(myMsg.CorrelationID))
                    {
                        Socket myHandler = m_Handlers[myMsg.CorrelationID] as Socket;

                        // add our message headers
                        string myData;
                        StreamHelper.WriteMsg(out myData, myMsg);
                        Send(myHandler, myData);
                    }
                    else
                    {
                        Exception myE = new Exception("Handler not found for:" + myMsg.CorrelationID);
                        setStatus(State.open, ErrorLevel.recoverableError);
                        throw (myE);
                    }
                }
                else
                {
                    Exception myE = new Exception("No correlation ID - can't send message");
                    throw (myE);
                }
            }
            catch (Exception myE)
            {
                if (m_Handlers.ContainsKey(myMsg.CorrelationID))
                {
                    //m_Log.Warn("Send - correlation ID removed from handler list due to fault:" + myMsg.CorrelationID);
                    m_Handlers.Remove(myMsg.CorrelationID);
                }
                try
                {
                    setStatus(State.closed, ErrorLevel.normal, myMsg.CorrelationID);
                }
                catch
                {
                }
                throw (myE);
            }
        }

        private void Send(Socket handler, String data)
        {
            try{
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
            }
            catch(Exception myE)
            {
                //m_Log.Error("Send", myE);
                throw( myE);
            }
        }

        private  void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;
                string myRemoteEP = handler.RemoteEndPoint.ToString();
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);

                //if (m_Log.IsInfoEnabled)
                //{
                //    m_Log.Info("Sent:" + bytesSent.ToString());
                //}

                // If no bytes sent an error
                if (bytesSent == 0)
                {
                    // remove this handler from the list - this will cause
                    // subseqent attempts to send to fail
                    removeHandler(ref handler);
                    setStatus(State.closed, ErrorLevel.error, myRemoteEP);
                }

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
            }
            catch (Exception e)
            {
                //m_Log.Error("SendCallback", e);
                setStatus(State.open, ErrorLevel.recoverableError);
            }
        }

        public OnMessageEvent OnMessage
        {
            get { return m_Receiver; }
            set { m_Receiver = value; }
        }

        public OnStatusEvent OnStatus
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }

        /// <summary>
        /// Get the adapter state
        /// </summary>
        public State State
        {
            get
            {
                return m_State;
            }
        }

        public void Open()
        {
            // Receive the response from the remote device.
            m_RunWD = true;
            m_WatchDog = new System.Threading.Thread(new System.Threading.ThreadStart(this.watchdog));
            m_WatchDog.Start();
        }
        private void watchdog()
        {
            while (m_RunWD)
            {
                try
                {
                    /*
                    if ((m_State == State.opening) && (m_ErrorLevel == ErrorLevel.recoverableError))
                    {
                        // try to reconnect
                        StartClient();
                    }
                    else if ((m_State == State.open) && (m_ErrorLevel == ErrorLevel.recoverableError))
                    {
                        StartClient();
                    }
                    else if ((m_State == State.open) && (m_ErrorLevel == ErrorLevel.normal) && (m_Begin == false))
                    {
                        begin();
                    }
                     */

                    // sleep 10 sec
                    System.Threading.Thread.Sleep(10000);
                }
                catch
                {
                }
            }
        }

        public void Close()
        {
           //NOT USED? m_Begin = false;
            foreach (Socket mySocket in m_Handlers.Values)
            {
                mySocket.Close();
            }
            m_Listener.Close();
        }

        public void Begin()
        {
           //NOT USED? m_Begin = true;
            m_SvrThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.listenProcess));
            m_SvrThread.Start();
        }
        private void listenProcess()
        {
            try
            {
                StartListening();
            }
            catch (Exception myE)
            {
                //m_Log.Error("listenProcess", myE);
            }
        }
    }
}

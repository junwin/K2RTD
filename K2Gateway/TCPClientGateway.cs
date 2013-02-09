/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011 KaiTrade LLC (registered in Delaware)
 *                     All Rights Reserved Worldwide
 *
 * STRICTLY PROPRIETARY and CONFIDENTIAL
 *
 * WARNING:  This file is the confidential property of KaiTrade LLC For
 * use only by those with the express written permission and license from
 * KaiTrade LLC.  Unauthorized reproduction, distribution, use or disclosure
 * of this file or any program (or document) is prohibited.
 *
 ***************************************************************************/
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace K2Gateway
{
    public struct GatewayStatus
    {
        public const string CLOSED = "CLOSED";
        public const string CLOSING = "CLOSING";
        public const string OPEN = "OPEN";
        public const string OPENING = "OPENING";
        public const string NONE = "NOTKNOWN";
    }

    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 64000;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    /// <summary>
    /// Sends  messages to some socket server
    /// </summary>
    public class TCPClientGateway
    {
        // Create a logger for use in this class
        //public log4net.ILog //m_Log;

        /// <summary>
        /// current state of the gateway
        /// </summary>
        private State m_State;

        /// <summary>
        /// Error level - these can occur at any state
        /// </summary>
        private ErrorLevel m_ErrorLevel;

        /// <summary>
        /// State object for the client socket - contains the stream of chars
        /// so dont recreate or you may lose data
        /// </summary>
        StateObject m_ClientState;

        /// <summary>
        /// Endpoint for this connection
        /// </summary>
        string m_EndPoint = "";

        /// <summary>
        /// User tag for this client GW
        /// </summary>
        string m_Tag = "";

        // ManualResetEvent instances signal completion.
        private  ManualResetEvent connectDone;
        private  ManualResetEvent sendDone;
        private  ManualResetEvent m_ReceiveDone;

        /// <summary>
        /// Server we want to connect to
        /// </summary>
        private string m_Server;

        /// <summary>
        /// well known port on the server
        /// </summary>
        private int m_Port;

        /// <summary>
        /// Socket used by client to send messages to the server
        /// </summary>
        private Socket m_Client;

        /// <summary>
        /// delegates we event when a message is received
        /// </summary>
        private OnMessageEvent m_Receiver;

        /// <summary>
        /// Delegates we event when some status message occurs
        /// </summary>
        private OnStatusEvent m_Status;

        /// <summary>
        /// main thread that the client reads on - else Begin call would block
        /// </summary>
        System.Threading.Thread m_ReadThread;

        /// <summary>
        /// thread that monitors status
        /// </summary>
        System.Threading.Thread m_WatchDog;

        /// <summary>
        /// Should the watchdog thread be running true => yes
        /// </summary>
        private bool m_RunWD;

        /// <summary>
        /// Can we begin receiving
        /// </summary>
        private bool m_Begin = false;

        public TCPClientGateway(string myServer, int myPort)
        {
            connectDone = new ManualResetEvent(false);
            sendDone = new ManualResetEvent(false);
            m_ReceiveDone = new ManualResetEvent(false);
            m_Server = myServer;
            m_Port = myPort;
            //m_Log = log4net.LogManager.GetLogger("Kaitrade");
            //m_Log.Info("SocketSenderGateway: Server:" + m_Server + " Port:" + m_Port.ToString());

            setStatus(State.opening, ErrorLevel.normal);
        }

        /// <summary>
        /// Get/Set the user tag
        /// </summary>
        public string Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }

        /// <summary>
        /// Get the endpoint of a connected socket
        /// </summary>
        public string EndPoint
        {
            get
            {
                return m_EndPoint;
            }
        }

        private  void StartClient()
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the
                // remote device is "host.contoso.com".
                IPHostEntry ipHostInfo = Dns.Resolve(m_Server);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, m_Port);

                // Create a TCP/IP socket.
                m_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                m_Client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), m_Client);

                //setStatus(State.opening, ErrorLevel.normal);

                connectDone.WaitOne();
            }
            catch (Exception e)
            {
                //m_Log.Error("StartClient", e);

                // cant recover this error
                setStatus(State.opening, ErrorLevel.error);

                throw e;
            }
        }

        private  void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);
                m_EndPoint = client.RemoteEndPoint.ToString();
                //m_Log.Info("Socket connected to:" + m_EndPoint);

                m_State = State.open;

                setStatus(State.open, ErrorLevel.normal);
                //setStatus(State.open, ErrorLevel.normal);
            }
            catch (Exception e)
            {
                //m_Log.Error("ConnectCallback", e);
                //  cant throw since we are in a windows callback - throw (e);
                // consider it recovereable since we could reconnect
                // setStatus(State.opening, ErrorLevel.recoverableError);
                setStatus(State.opening, ErrorLevel.normal);
            }
            finally
            {
                // Signal that the connection has been made.
                connectDone.Set();
            }
        }

        private  void Receive(Socket client)
        {
            try
            {
                m_ReceiveDone.Reset();

                m_ClientState.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(m_ClientState.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), m_ClientState);
            }
            catch (Exception e)
            {
                setStatus(State.open, ErrorLevel.error);
                //m_Log.Error("Receive", e);
                throw e;
            }
        }

        private  void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read
                    // more data.
                    while (state.sb.Length > 0)
                    {
                        string myContent = state.sb.ToString();
                        long totLength;
                        string myMsg;
                        string myLabel;
                        string myClientID;
                        totLength = StreamHelper.GetMessage(ref myContent, out myMsg, out myLabel, out myClientID);

                        if (totLength > -1)
                        {
                            // All the data has been read from the
                            // client. Display it on the console.
                            ////m_Log.Info("Read:" + myMsg.Length.ToString() + "bytes :" + myMsg);

                            // remove the message from the buffer
                            state.sb.Remove(0, (int)totLength);

                            // remove the EOF
                            //myContent = myContent.Remove((int)totLength);

                            EventGatewayClients(ref myMsg, myLabel, ref client);
                        }
                        else
                        {
                            break;
                        }
                    }

                    /// Get the rest of the data.
                    //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                m_ReceiveDone.Set();
            }
            catch (Exception e)
            {
                //m_Log.Error("ReceiveCallback", e);
                // mark this as recoverable - they will then try to reconnect;
                setStatus(State.closed, ErrorLevel.recoverableError);

                // stop the begin/read thread
                m_Begin = false;

                // unblock the begin/read thread
                m_ReceiveDone.Set();
            }
        }
        private void send(Socket client, String data)
        {
            try
            {
                // Convert the string data to byte data using ASCII encoding.
                //string myData;
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
            }
            catch (Exception myE)
            {
                setStatus(State.closed, ErrorLevel.recoverableError);
                throw myE;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);

                //m_Log.Info("Sent: " + bytesSent.ToString());

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                //m_Log.Error("SendCallback", e);
                setStatus(State.closed, ErrorLevel.recoverableError);
            }
        }

        private void EventGatewayClients(ref string myContent, string myLabel, ref Socket myHandler)
        {
            try
            {
                // get my end point
                m_EndPoint = myHandler.RemoteEndPoint.ToString();

                // construct an kai message wrapper
                KaiMessageWrap myWrap = new KaiMessageWrap();

                myWrap.Data = myContent;
                myWrap.Label = myLabel;

                // Use the Corrlation ID to store the endpoint
                // all return messages with that correlationID will
                // be sent down the relevant socket
                myWrap.CorrelationID = m_EndPoint;
                myWrap.Tag = m_Tag;
                // event our gateway clients
                m_Receiver(myWrap);
            }
            catch
            {
                ////m_Log.Error("EventGatewayClients", myE);
            }
        }

        private void SetStatus(State myState)
        {
            m_State = myState;

            // construct an kai message wrapper
            KaiMessageWrap myWrap = new KaiMessageWrap();
            string myTemp = "";
            switch (m_State)
            {
                case  State.closed:
                    myTemp = GatewayStatus.CLOSED;
                    break;
                case State.closing:
                    myTemp = GatewayStatus.CLOSING;
                    break;
                case State.open:
                    myTemp = GatewayStatus.OPEN;
                    break;
                case State.opening:
                    myTemp = GatewayStatus.OPENING;
                    break;
                default:
                    myTemp = GatewayStatus.NONE;
                    break;
            }
            myWrap.Data = m_EndPoint;
            myWrap.Label = myTemp;
            myWrap.CorrelationID = "";
            myWrap.Tag = m_Tag;

            if (m_Status != null)
            {
                m_Status(myWrap);
            }
        }

        private void send(ref KaiMessageWrap myMsg, Socket myClient)
        {
            string myData;
            StreamHelper.WriteMsg(out myData,  myMsg);
            send(m_Client, myData);
        }

        public void Send(KaiMessageWrap myMsg)
        {
            send(ref myMsg, null);
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

            // Let the watch dog Start the client since that blocks until it connects

            //StartClient();
        }
        private void setStatus(State myState, ErrorLevel myErrorLevel)
        {
            m_State = myState;
            m_ErrorLevel = myErrorLevel;
            SetStatus(myState);
        }

        private void watchdog()
        {
            while (m_RunWD)
            {
                try
                {
                    setStatus(m_State, m_ErrorLevel);
                    if ((m_State == State.opening) && (m_ErrorLevel == ErrorLevel.normal))
                    {
                        // try to connect - this blocks and if this works the status will switcb to open
                        StartClient();
                    }
                    else if ((m_State == State.opening) && (m_ErrorLevel == ErrorLevel.recoverableError))
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
                    else if ((m_State == State.closed) && (m_ErrorLevel == ErrorLevel.recoverableError) && (m_Begin == false))
                    {
                        StartClient();
                    }

                    // sleep 10 sec
                    System.Threading.Thread.Sleep(10000);
                }
                catch
                {
                    setStatus(State.closed, ErrorLevel.error);
                }
            }
            setStatus(State.closed, ErrorLevel.error);
            //m_Log.Error("watchdog: terminating");
        }

        public void Close()
        {
            try
            {
                m_Client.Close();
                m_Begin = false;
                m_RunWD = false;
            }
            catch (Exception myE)
            {
                //m_Log.Error("Close", myE);
            }
        }
        private void begin()
        {
            if (!m_Begin)
            {
                // Receive the response from the remote device.
                m_Begin = true;
                m_ReadThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.readProcess));
                m_ReadThread.Start();
            }
        }
        public void Begin()
        {
            begin();
        }

        private void readProcess()
        {
            try
            {
                // Create the state object.
                m_ClientState = new StateObject();
                while (m_Begin)
                {
                    Receive(m_Client);
                    m_ReceiveDone.WaitOne();
                    //System.Threading.Thread.Sleep(200);
                }
            }
            catch (Exception myE)
            {
                setStatus(State.closed, ErrorLevel.error);
                //m_Log.Error("listenProcess", myE);
            }
        }
    }
}

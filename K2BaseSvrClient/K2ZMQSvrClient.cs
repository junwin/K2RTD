/***************************************************************************
 *    
 *      Copyright (c) 2009,2010 KaiTrade LLC (registered in Delaware)
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Timers;
using System.Threading;


using log4net;
using log4net.Config;

namespace K2BaseSvrClient
{
    //public delegate void OnUpdateNotify();
    //public delegate void OnUpdateStatus(string correlationId, ConnectionState newState, string text);

    //public enum ConnectionState { waiting, opening, open, closing, closed, error };


    /// <summary>
    /// provides helpe functions to use KTARemote
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class K2ZMQSvrClient 
    {
       
        private string m_CorrelationID = "";


        private string m_AppID = "";

        /// <summary>
        /// used to lock the class during updates and reads
        /// </summary>
        private static object s_UpdateLock = new object();


        Dictionary<int, object> m_TopicUpdates;

        /// <summary>
        /// ID used for status reports to Excel
        /// </summary>
        private int m_StatusID = -1;

        /// <summary>
        /// Timer for those algos that require some time based processing
        /// </summary>
        private System.Timers.Timer m_Timer;

        /// <summary>
        /// Timer interval used for the timer
        /// </summary>
        private long m_TimerInterval = 30000;
        private OnUpdateNotify m_UpdateNotify;
        private OnUpdateStatus m_UpdateStatus;


        private Boolean m_ZMQ = false;
        //  Prepare our context and socket
         private ZMQ.Context m_Context;
        private ZMQ.Socket m_Socket;

        private Thread m_RcvThread;
        private bool m_RunRcvThread = true;

        private List<DataConnectionRequest> m_Subscriptions;

        /// <summary>
        /// List of data connection requests that are pending being sent to the server
        /// </summary>
        private List<DataConnectionRequest> m_PendingDataConnect;

        private K2RTDStatus m_frmStatus = null;

        private bool m_InReconnect = false;

        private ConnectionState m_State;

        /// <summary>
        /// client gatway used to communicate with the toolkit server helper
        /// </summary>
        private KaiTCPComm.TCPClientGateway m_DefaultClientGW;

        /// <summary>
        /// Collection of available client gateway
        /// </summary>
        private System.Collections.Hashtable m_ClientGWs;



        // Create a logger for use in this class
        public log4net.ILog m_Log;

        public K2ZMQSvrClient()
        {
            try
            {
                m_TopicUpdates = new Dictionary<int, object>();
                m_Subscriptions = new List<DataConnectionRequest>();
                m_PendingDataConnect = new List<DataConnectionRequest>();
                m_State = ConnectionState.waiting;

                StartTimer();

                //m_frmStatus = new K2RTDStatus();
                //m_frmStatus.Show();

                try
                {
                    // BasicConfigurator replaced with XmlConfigurator.
                    string fileName = @"C:\Program Files\KaiTrade\K2RTD\RTDLogger.xml";
                    XmlConfigurator.Configure(new System.IO.FileInfo(fileName));


                }
                catch (Exception myE)
                {
                }
                log4net.Config.BasicConfigurator.Configure();
                m_Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


                m_Log = LogManager.GetLogger(typeof(K2RTDClientHelper));
                m_Log.Info("K2RTD Started:");
            }
            catch (Exception myE)
            {
                System.Windows.Forms.MessageBox.Show("Error starting  RTD:" + myE.ToString(), "K2 RTD Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

        }

        private void setState(ConnectionState newState)
        {
            m_State = newState;

        }

        public ConnectionState State
        { get { return m_State; } }

        protected void StartTimer()
        {
            if (m_TimerInterval > 0)
            {
                m_Timer = new System.Timers.Timer(m_TimerInterval);
                m_Timer.Elapsed += new ElapsedEventHandler(OnTimer);
                m_Timer.Interval = (double)m_TimerInterval;
                m_Timer.Enabled = true;
            }
        }

        protected void StopTimer()
        {
            if (m_Timer != null)
            {
                m_Timer.Enabled = false;
            }
            m_Timer = null;
        }
        private void OnTimer(object source, ElapsedEventArgs e)
        {
            try
            {
                

            }
            catch (Exception myE)
            {

            }
        }

        public int StatusID
        { get { return m_StatusID; } set { m_StatusID = value; } }

        public OnUpdateNotify UpdateNotify
        {
            get { return m_UpdateNotify; }
            set { m_UpdateNotify = value; }
        }

        public OnUpdateStatus UpdateStatus
        {
            get { return m_UpdateStatus; }
            set { m_UpdateStatus = value; }
        }


        

        public void Connect()
        {
            try
            {

               
            }
            catch (Exception myE)
            {
                //m_Log.Error("Connect", myE);
            }
        }

        public void ReConnect()
        {
            try
            {
               
            }
            catch (Exception myE)
            {
                //m_Log.Error("ReConnect", myE);
            }
        }

        

       
       
        private void setStatusField(string value)
        {
            try
            {
                if (this.StatusID != -1)
                {
                    addTopicUpdate(this.StatusID, value, 0);
                }
            }
            catch (Exception myE)
            {
            }
        }
       




     

        #region updateHandling
        private void addTopicUpdate(int topicID, string topicValue, int topicType)
        {

            try
            {
                if (m_TopicUpdates.ContainsKey(topicID))
                {
                    m_TopicUpdates[topicID] = topicValue;
                }
                else
                {
                    m_TopicUpdates.Add(topicID, topicValue);
                }
            }
            catch (Exception myE)
            {
            }

        }

        /// <summary>
        /// Return the value as an object (string or double in this release)
        /// </summary>
        /// <returns></returns>
        public object GetValue(string value)
        {
            double dVal;
            object myVal = null;
            if (double.TryParse(value, out dVal))
            {
                myVal = dVal;
            }
            else
            {
                myVal = value;
            }
            return myVal;

        }

        public Array RefreshData(ref int TopicCount)
        {
            m_Log.Info("RefreshData:In");
            lock (s_UpdateLock)
            {
                try
                {

                    object[,] myData = new object[2, m_TopicUpdates.Count];
                    TopicCount = 0;
                    foreach (int topicId in m_TopicUpdates.Keys)
                    {
                        myData[0, TopicCount] = topicId;
                        myData[1, TopicCount] = GetValue(m_TopicUpdates[topicId].ToString());
                        TopicCount++;
                    }

                    m_TopicUpdates.Clear();

                    return myData;
                }
                catch (Exception myE)
                {
                    TopicCount = 0;
                    m_Log.Error("RefreshData", myE);
                }

            }
            return null;
        }

        #endregion

        #region IK2RTDSupportCallback Members

        public void OnTopicUpdate(int[] id, string[] value)
        {
            m_Log.Info("OnTopicUpdate");
            lock (s_UpdateLock)
            {
                try
                {

                    for (int i = 0; i < id.Length; i++)
                    {
                        addTopicUpdate(id[i], value[i], 0);
                    }
                    if (m_UpdateNotify != null)
                    {
                        m_UpdateNotify();
                    }
                }
                catch (Exception myE)
                {
                    m_Log.Error("OnTopicUpdate", myE);
                }
            }

        }

        public void OnMessage(ServiceReference1.KTAMessage myMsg)
        {
            /*
            string cc = (myMsg.m_Label + ":" + myMsg.m_Data);
            //m_Results.Items.Add(cc);
            if (m_UpdateNotify != null)
            {
                m_UpdateNotify();
            }
             */
        }
        #endregion

        private string getCorrelationID(string appID)
        {
            string id = appID;
            id += DateTime.Now.Ticks.ToString();
            return id;
        }
        public void StartServer(string appID)
        {
            try
            {
                ZMQConnect();
                StartReqSvr();
                m_Log.Info("StartServer:" + m_CorrelationID);
            }
            catch (Exception myE)
            {
                m_Log.Error("StartServer", myE);
            }
        }

        public string GetAccessID(string myAppID)
        {
            m_CorrelationID = myAppID;
            //m_CorrelationID = Client.GetAccessID(myAppID);
            return m_CorrelationID;
        }

        public void Register()
        {
            //if(m_Client.
            //Client.Register(m_CorrelationID);
        }


        public void Log(bool isError, string src, string msg)
        {
            try
            {
               // Client.Log(m_CorrelationID, isError, src, msg);
            }
            catch (Exception myE)
            {
            }
        }

        private bool isUpdateRequest(string requestType)
        {
            if (requestType == "PX")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ConnectData(int TopicID, string[] Strings)
        {
            try
            {

                string myReqType = Strings.GetValue(0) as string;
                string subjectID = Strings.GetValue(1) as string;
                string headerName = Strings.GetValue(2) as string;
                m_Log.Info("ConnectData:In" + myReqType + ":" + subjectID + ":" + headerName);

                DataConnectionRequest ds = new DataConnectionRequest(m_CorrelationID, TopicID, subjectID, headerName, Strings);
                string data = "";
                m_Socket.Send(Encoding.ASCII.GetBytes(m_CorrelationID.ToCharArray()), 2);
                m_Socket.Send(Encoding.ASCII.GetBytes(TopicID.ToString().ToCharArray()), 2);
                m_Socket.Send(Encoding.ASCII.GetBytes(subjectID.ToCharArray()), 2);
                m_Socket.Send(Encoding.ASCII.GetBytes(headerName.ToCharArray()), 2);
                m_Socket.Send(Encoding.ASCII.GetBytes(data.ToCharArray()));
                //m_Socket.Send(Encoding.ASCII.GetBytes(data.ToCharArray()));

                

                m_Log.Info("PendingSize:" + m_PendingDataConnect.Count.ToString());


            }
            catch (Exception myE)
            {
                m_Log.Error("ConnectData", myE);
            }
        }


        public void ReplaySubscriptions()
        {
            ReplayRequests(m_Subscriptions);
        }

        public void ReplayRequests(List<DataConnectionRequest> reqList)
        {
            try
            {
                foreach (DataConnectionRequest ds in reqList)
                {
                    //Client.ConnectData(ds.AccessID, ds.TopicID, ds.Strings);
                }

            }
            catch (Exception myE)
            {
            }
        }





        public void HeartBeat(string data)
        {
            try
            {
                //Client.HeartBeat(m_CorrelationID, DateTime.Now.ToString());
            }
            catch (Exception myE)
            {
            }
        }

        public void UnSubscribeData(int myID)
        {
            try
            {
                //Client.UnSubscribeData(m_CorrelationID, myID);
            }
            catch (Exception myE)
            {
            }
        }

        public void Init()
        {
            try
            {
                ZMQConnect();
                StartReqSvr();
            }
            catch (Exception myE)
            {
            }
        }

        public void Finalize()
        {
            try
            {
                //Client.Finalize(m_CorrelationID);
                m_RcvThread.Abort();
                m_RcvThread = null;
            }
            catch (Exception myE)
            {
            }
        }

       
        private void ZQMSend(string data)
        {
            m_Socket.Send(Encoding.ASCII.GetBytes(data.ToCharArray()));
        }
        private void ZMQConnect()
        {



            //  Prepare our context and socket
            m_Context= new ZMQ.Context(1);


            m_Socket = m_Context.Socket(ZMQ.XREQ);
            m_Socket.Connect("tcp://localhost:5555");
           

        }

        /// <summary>
        /// Start the watchdog thread
        /// </summary>
        /// <param name="myState"></param>
        private void StartReqSvr()
        {
            try
            {
                m_RunRcvThread = true;
                m_RcvThread = new Thread(new ThreadStart(this.runRecvThread));
                m_RcvThread.SetApartmentState(ApartmentState.STA);
                m_RcvThread.Start();

            }
            catch (Exception myE)
            {
                //this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "Issue starting watchdog thread" + myE.Message);
                m_Log.Error("RcvThread", myE);

            }
        }

        private void runRecvThread()
        {
            try
            {
                // allocate a buffer
                byte[] zmq_buffer = new byte[4096];
                while (m_RunRcvThread)
                {
                    try
                    {
                        Thread.Sleep(5000);
                        //  Wait for next request from client
                        //m_Socket.Recv(out zmq_buffer);
                        try
                        {
                            //string request = Encoding.ASCII.GetString(zmq_buffer);
                            
                        }
                        catch (Exception myE)
                        {
                        }



                    }
                    catch (ZMQ.Exception z)
                    {
                        // report the exception
                        //Console.WriteLine("ZMQ Exception occurred : {0}", z.Message);
                    }

                    
                }

            }
            catch (Exception myE)
            {

                m_Log.Error("runReqSvrThread", myE);

            }
        }

    }
}


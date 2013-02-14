//-----------------------------------------------------------------------
// <copyright file="K2TCPRTDSvrClient.cs" company="KaiTrade LLC">
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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Timers;
using System.Xml.Serialization;



namespace K2BaseSvrClient
{
    public enum RTDConnectionState
    {
        connecting = 0,
        connected = 1,
        stale = 2,
        disconnected = 3,
        none = 4,
    }
    /// <summary>
    /// Provides support for the RTD server, connects to the feeds and handles updates
    /// </summary>
    public class K2SVRRTDSvrClient
    {
         

        /// <summary>
        /// these describe operations that are done as a sequence in the timer loop
        /// </summary>
        public enum TimedOperation { start, stop, resubscribe, none };

        /// <summary>
        /// used to lock the class during updates and reads
        /// </summary>
        private static object s_UpdateLock = new object();
        private static object s_RequestLock = new object();
        private TimedOperation m_TimedOperation = TimedOperation.none;

        public enum ERequest { restart, none };
        private ERequest m_Request = ERequest.none;

        private TimedOperation[] m_Restart;
        private int m_RestartPos = 0;

        RTDConnectionState m_State = RTDConnectionState.none;

        private static K2SVRRTDSvrClient s_instance;

        // Create a logger for use in this class
        //public static log4net.ILog m_Log;

        /// <summary>
        /// Dump data flag - if true and a file is open then dump data onto the file
        /// </summary>
        private bool m_DumpData = false;

        /// <summary>
        /// Path to a file to dump data received
        /// </summary>
        private string m_Path;

        /// <summary>
        /// Last status message recorded from channel or other sources
        /// </summary>
        private K2Gateway.KaiMessageWrap m_LastStatus;

        /// <summary>
        /// Dump file stream
        /// </summary>
        private FileStream m_FileStream;


        Dictionary<int, object> m_TopicUpdates;


        /// <summary>
        /// Writer used to output data to the file stream
        /// </summary>
        private System.IO.StreamWriter m_Writer;

        /// <summary>
        /// Used to register interest in updates
        /// </summary>
        /// <param name="msg"></param>
        public delegate void OnChange(K2Gateway.KaiMessageWrap myWrap);

        /// <summary>
        /// Used to register interest in status changes
        /// </summary>
        /// <param name="msg"></param>
        public delegate void OnStatusChange(K2Gateway.KaiMessageWrap myWrap);

        private OnChange m_Change = null;

        private OnStatusChange m_StatusChange = null;

        /// <summary>
        /// Set of topics one per product
        /// </summary>
        private System.Collections.Hashtable m_Topics;

        /// <summary>
        /// Map linking an Excel topic id to a Header
        /// </summary>
        private System.Collections.Hashtable m_IDHeaderMap;

        /// <summary>
        /// client gatway used to communicate with the toolkit server helper
        /// </summary>
        private K2Gateway.TCPClientGateway m_DefaultClientGW;

        /// <summary>
        /// Collection of available client gatway
        /// </summary>
        private System.Collections.Hashtable m_ClientGWs;

        /// <summary>
        /// Map a feed to a channel that services the 
        /// feed using the FeedName
        /// </summary>
        //private System.Collections.Generic.Dictionary<string, Feed> m_FeedMap;

        /// <summary>
        /// Maps a gateway name to a  list of feeds - a GW can support N feeds
        /// </summary>
        //private System.Collections.Generic.Dictionary<string, List<Feed>> m_FeedGWNameMap;

        /// <summary>
        /// RTD Config info
        /// </summary>
        //private KAI.kaitns.KRTDConfig m_Config;

        /// <summary>
        /// Path to the configuration files
        /// </summary>
        private string m_ConfigPath;

       
        private KRTDConfig m_Config = null;
        private const string configFileName = @"krtdconfig.xml";

        private OnUpdateNotify m_UpdateNotify;
        private OnUpdateStatus m_UpdateStatus;

        /// <summary>
        /// Queues any requests that cannot be sent because the 
        /// comm channel is opening
        /// </summary>
        private System.Collections.Queue m_MsgQueue;

        /// <summary>
        /// Set the initial channel state to closed
        /// </summary>
        private string m_ChannelState = "CLOSED";

        private frmKRTD m_RTDForm;

        /// <summary>
        /// Timer for those algos that require some time based processing
        /// </summary>
        private System.Timers.Timer m_Timer;

        /// <summary>
        /// Timer interval used for the timer
        /// </summary>
        private long m_TimerInterval = 5000;

        private long m_LastHBTicks = 0;
        private long m_TripTime = 0;
        /// <summary>
        /// max delay 10000 ticks = 1ms
        /// </summary>
        private long m_MaxHBDelayTicks = 200000000;

        private int  m_DefaultPort = 11001;
        private string m_DefaultFeedName = "DEFAULT";

        private string m_CorrelationID;

        private List<DataConnectionRequest> m_Subscriptions;

        /// <summary>
        /// List of data connection requests that are pending being sent to the server
        /// </summary>
        private List<DataConnectionRequest> m_PendingDataConnect;

        

        private long m_ClientID = 0;
        

        protected K2SVRRTDSvrClient()
        {
            try
            {
                m_RTDForm = new frmKRTD();
                m_Subscriptions = new List<DataConnectionRequest>();
                m_PendingDataConnect = new List<DataConnectionRequest>();
                m_TopicUpdates = new Dictionary<int, object>();

                //m_ConfigPath = @"C:\Program Files\KaiTrade\K2Accelerator\config\krtdconfig.xml";
                m_ConfigPath = getK2RTDPath();
                LoadConfig(this.ConfigFilePath);

                /*  Log support
                 //log4net.Config.BasicConfigurator.Configure();
                //m_Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                 

                ////m_Log = LogManager.GetLogger(typeof(KRTDSupport));
                //m_Log.Info("RTDSupport Started:");

                 */



                m_Topics = new System.Collections.Hashtable();
                //m_IDHeaderMap = new System.Collections.Hashtable();
                m_ClientGWs = new System.Collections.Hashtable();
                //m_FeedMap = new Dictionary<string, Feed>();
                //m_FeedGWNameMap = new Dictionary<string, List<Feed>>();
                m_MsgQueue = new System.Collections.Queue();

                //initRTDStatusSubject();


                m_RTDForm.Config = m_Config;
                m_RTDForm.Client = this;
                m_RTDForm.ConfigPath = m_ConfigPath;

                StartTimer();
            }
            catch (Exception myE)
            {
            }
        }

        public string ConfigPath
        {
            get { return m_ConfigPath; }
            set { m_ConfigPath = value; }
        }

        public string ConfigFilePath
        { get { return ConfigPath + configFileName; } }

        string getK2RTDPath()
        {
            string binPath = "";
            try
            {

                // get the location of the current assembly i.e. K2RTDServerKit, assume dependancies are there
                string[] reqAssyDef = System.Reflection.Assembly.GetExecutingAssembly().FullName.Split(',');
                int reqDllNamePos = System.Reflection.Assembly.GetExecutingAssembly().Location.IndexOf(reqAssyDef[0]);
                binPath = System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, reqDllNamePos - 1);

                //m_PlugInLog.Info("DynamicLoad:" + myPath);
                binPath+=  @"\";


            }
            catch
            {
            }
            return binPath;

        }

        public void SaveConfig(string path)
        {
            try
            {
                TextWriter writer = new StreamWriter(path,false);
                XmlSerializer serializer = new XmlSerializer(typeof(KRTDConfig));
                serializer.Serialize(writer, m_Config);
                writer.Close();

                
            }
            catch (Exception myE)
            {
            }
        }

        public void LoadConfig(string path)
        {
            try
            {
                //m_ConfigPath = path;
                m_Config = new KRTDConfig();

                TextReader reader = new StreamReader(path);
                XmlSerializer serializer = new XmlSerializer(typeof(KRTDConfig));
                m_Config = (KRTDConfig)serializer.Deserialize(reader);
                reader.Close();

                try
                {

                    if (m_Config.IPEndpoint.Server.Length == 0)
                    {
                        m_Config.IPEndpoint.Server = System.Environment.MachineName;
                    }
                }
                catch
                {
                    m_Config.IPEndpoint.Server = System.Environment.MachineName;
                }
                m_RTDForm.Config = m_Config;
                // BasicConfigurator replaced with XmlConfigurator.

                //string fileName = @"C:\kairoot\K2\RTDLogger.xml";
                string fileName = m_Config.LogConfigPath;
                //XmlConfigurator.Configure(new System.IO.FileInfo(fileName));

 
            }
            catch (Exception myE)
            {
                m_Config.IPEndpoint = new IPEndpoint();
                m_Config.IPEndpoint.Server = System.Environment.MachineName;
                m_Config.IPEndpoint.Port = 11001;
                m_RTDForm.Config = m_Config;

            }
        }

        public static K2SVRRTDSvrClient Instance()
        {
            // Uses "Lazy initialization"
            if (s_instance == null)
                s_instance = new K2SVRRTDSvrClient();

            return s_instance;
        }

        public long TripTime
        {
            get{return m_TripTime;}
        }

        public void OnTopicUpdate(int[] id, string[] value)
        {
            ////m_Log.Info("OnTopicUpdate");
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
                   //m_Log.Error("OnTopicUpdate", myE);
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
        //public log4net.ILog Log
        //{ get { return m_Log; } }

        
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

        /// <summary>
        /// called on each timer tick - implimented by the base class
        /// </summary>
        protected virtual void TimerTick()
        {
            
            try
            {
                sendDataReq("HB", DateTime.Now.Ticks.ToString());

                // are we stale - use the time of the last HB
                if ((DateTime.Now.Ticks - m_LastHBTicks) > m_MaxHBDelayTicks)
                {
                    // dont report stale if alread disconnected
                    if (ConnectionState != RTDConnectionState.disconnected)
                    {
                        SetConnectionState(RTDConnectionState.stale);
                    }
                }
                else
                {
                    SetConnectionState(RTDConnectionState.connected);
                }

                // each type of request defines primitive operations that need to 
                // be done a sequence
                switch (this.Request)
                {
                    case ERequest.restart:
                        m_TimedOperation = m_Restart[m_RestartPos];
                        m_RestartPos++;
                        if (m_RestartPos >= m_Restart.Length)
                        {
                            // we are done
                            this.Request = ERequest.none;
                        }
                        break;
                    default:
                        m_TimedOperation = TimedOperation.none;
                        break;
                }

                // process any primitive operations - these are set above 
                switch (m_TimedOperation)
                {
                    case TimedOperation.start:
                        this.Start();
                        break;

                    case TimedOperation.stop:
                        this.Stop();
                        break;

                    case TimedOperation.resubscribe:
                        //ReSubscribe("*", 0);
                        break;
                }

            }
            catch (Exception myE)
            {
                //m_Log.Error("TimerTick", myE);
            }
        }

        private void OnTimer(object source, ElapsedEventArgs e)
        {
            try
            {
                m_Timer.Enabled = false;
                lock (s_RequestLock)
                {
                    TimerTick();
                }
            }
            catch (Exception myE)
            {
                //m_Log.Error("OnTimer", myE);
            }
            finally
            {
                m_Timer.Enabled = true;
            }
        }
        /// <summary>
        /// get/set timer interval - note this doen not change a running timer
        /// </summary>
        public long TimerInterval
        {
            get { return m_TimerInterval; }
            set { m_TimerInterval = value; }
        }

        private string stateAsString(RTDConnectionState inState)
        {
            string status = "";
            try
            {

                switch (inState)
                {
                    case RTDConnectionState.connecting:
                        //txtConnectionStatus.Text = "Connecting";
                        status = "Connecting";
                        break;
                    case RTDConnectionState.connected:
                        //txtConnectionStatus.Text = "Connected";
                        status = "Connected";
                        break;
                    case RTDConnectionState.disconnected:
                        //txtConnectionStatus.Text = "Not Connected";
                        status = "Not Connected";
                        break;
                    case RTDConnectionState.stale:
                        status = "Stale Data";
                        break;

                    default:
                        break;
                }
            }
            catch (Exception myE)
            {

            }
            return status;

        }

        public RTDConnectionState ConnectionState
        { get { return m_State; } set { SetConnectionState(value); } }

        public void SetConnectionState(RTDConnectionState state)
        {
            try
            {
                m_State = state;
                m_RTDForm.SetConnectionState(state);

                applyGWStatus("DEFAULT", state.ToString());
                applyFeedStatus("DEFAULT", state.ToString());

                switch (m_State)
                {
                    case RTDConnectionState.connecting:

                        break;
                    case RTDConnectionState.connected:

                        break;
                    case RTDConnectionState.disconnected:

                        break;

                    default:
                        break;
                }
            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Get the default header value for some header
        /// </summary>
        /// <param name="myHeaderName"></param>
        /// <returns></returns>
        public object GetDefaultValue(string myHeaderName)
        {
            return "N/A";
        }

        public OnChange OnObserverChange
        {
            get
            {
                return m_Change;
            }
            set
            {
                m_Change = value;
            }
        }

        public OnStatusChange OnOnObserverStatusChange
        {
            get
            {
                return m_StatusChange;
            }
            set
            {
                m_StatusChange = value;
            }
        }

        public ERequest Request
        { get { return m_Request; } set { m_Request = value; } }

        public void RestartRequest()
        {

            try
            {
                //m_Log.Info("RestartRequest:called");
                m_Restart = new TimedOperation[3] { TimedOperation.stop, TimedOperation.start, TimedOperation.resubscribe };
                m_RestartPos = 0;
                this.Request = ERequest.restart;
            }
            catch (Exception myE)
            {
                //m_Log.Error("RestartRequest", myE);
            }
        }

        /// <summary>
        /// Start the RTDSupport by connecting to the server
        /// </summary>
        public void Start()
        {
            try
            {
                ////m_Log.Info("Start Called");
                SetConnectionState(RTDConnectionState.connecting);

                if (m_RTDForm == null)
                {
                    m_RTDForm = new frmKRTD();
                    m_RTDForm.IP = System.Environment.MachineName;
                    m_RTDForm.Port = m_DefaultPort.ToString();
                    m_RTDForm.Show();
                }



                try
                {

                    //m_Log = log4net.LogManager.GetLogger("KAITRADE");
                    //m_Log.Info("KRTDSupport - started");
                }
                catch (Exception myE)
                {
                }



                openGW(m_DefaultFeedName, m_RTDForm.IP, int.Parse(m_RTDForm.Port));


                SetConnectionState(RTDConnectionState.connected);
            }
            catch (Exception myE)
            {
                //m_Log.Error("Start:", myE);
            }

        }
        /// <summary>
        /// Init the push client
        /// </summary>
        public void Init()
        {
            try
            {
                Start();
            }
            catch (Exception myE)
            {
                //m_Log.Error("", myE);
            }

        }

        /// <summary>
        /// Stop the connection to server
        /// </summary>
        public void Stop()
        {
            try
            {
                SendLogMessage("Stop called");
                

                foreach (K2Gateway.TCPClientGateway myGW in m_ClientGWs.Values)
                {
                    myGW.Close();
                }

                SetConnectionState(RTDConnectionState.disconnected);

            }
            catch (Exception myE)
            {
            }
        }

        public void Finalize()
        {
            try
            {
                SendLogMessage("Finalize called");

                Stop();

                // close the RTD support form
                m_RTDForm.Close();
                m_RTDForm = null;

            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Opens a TCP/IP connection to the driver/feed server
        /// </summary>
        /// <param name="myGW"></param>
        /// <param name="myName">feed name - return on message tag</param>
        /// <param name="myServer"></param>
        /// <param name="myPort"></param>
        public void openClientGW(out K2Gateway.TCPClientGateway myGW, string myName, string myServer, int myPort)
        {
            m_RTDForm.ConnectionStatus = "openClientGW";
            myGW = new K2Gateway.TCPClientGateway(myServer, myPort);
            myGW.Tag = myName;

            myGW.OnMessage += new K2Gateway.OnMessageEvent(OnRTDMessage);
            myGW.OnStatus += new K2Gateway.OnStatusEvent(OnChannelStatusMessage);
            myGW.Open();
        }

        /// <summary>
        /// Open a RTD TCP channel
        /// </summary>
        /// <param name="myName">name of endpoint</param>
        /// <param name="myServer">server address</param>
        /// <param name="myPort">port </param>
        private void openGW(string myName, string myServer, int myPort)
        {
            try
            {

                K2Gateway.TCPClientGateway myGW;


                openClientGW(out myGW, myName, myServer, myPort);


                if (m_ClientGWs.ContainsKey(myName))
                {
                    m_ClientGWs[myName] = myGW;
                }
                else
                {
                    m_ClientGWs.Add(myName, myGW);

                   
                }

                if (myName.ToUpper() == "DEFAULT")
                {
                    m_DefaultClientGW = myGW;
                }

            }
            catch (Exception myE)
            {
                //m_Log.Error("openChannel:" + myName, myE);
            }
        }

       

       

        /// <summary>
        /// send a RTD request on the channel specified - no defaults are applied
        /// </summary>
        /// <param name="myMsg">note the target ID must contain a valid channel ID</param>
        /// <returns>true if the message was sent</returns>
        private bool sendChannelRequest(K2Gateway.KaiMessageWrap myWrap)
        {
            try
            {
                if (m_ClientGWs.ContainsKey(myWrap.TargetID))
                {
                    K2Gateway.TCPClientGateway myGW = m_ClientGWs[myWrap.TargetID] as K2Gateway.TCPClientGateway;

                    if (myGW.State == K2Gateway.State.open)
                    {
                        myGW.Send(myWrap);
                        return true;
                    }

                }
            }
            catch (Exception myE)
            {
                //m_Log.Error("sendChannelRequest", myE);
            }

            return false;
        }

        private void applyGWStatus(string myGWName, string myStatus)
        {
            try
            {
                /*
                // get the feed list for supported by this gateway 
                //m_Log.Info("applyGWStatus:" + myGWName + ": " + myStatus);

                if (m_FeedGWNameMap.ContainsKey(myGWName))
                {
                    // process each feed
                    foreach (Feed myFeed in m_FeedGWNameMap[myGWName])
                    {
                        applyFeedStatus(myFeed.Name, myStatus);
                    }
                    //Header myHeader = GetTopicHeader(myGWName, "STATUS");
                    Topic myTopic = GetTopic(myGWName);
                    int nChanged = 0;
                    myTopic.ApplyUpdate(ref nChanged, "STATUS", myStatus);
                    //myHeader.Value = myStatus;
                    // inform RTD client that something has changed
                    m_Change(null);
                   }
                */
                
            }
            catch (Exception myE)
            {
                //m_Log.Error("applyGWStatus", myE);
            }

        }
        private void applyFeedStatus(string myFeedName, string myStatus)
        {
            try
            {
                /*
                //m_Log.Info("applyGWStatus:" + myFeedName + ": " + myStatus);
                // get the feed
                if (m_FeedMap.ContainsKey(myFeedName))
                {
                    Feed myFeed = m_FeedMap[myFeedName];
                    // has the status changed
                    if (myFeed.Status != myStatus)
                    {
                        myFeed.Status = myStatus;
                        try
                        {
                            Topic myTopic = GetTopic(myFeedName);
                            int nChanged = 0;
                            myTopic.ApplyUpdate(ref nChanged, "STATUS", myStatus);
                            // inform RTD client that something has changed
                            m_Change(null);

                        }
                        catch (Exception myE)
                        {
                            //m_Log.Error("applyFeedStatus", myE);
                        }

                    }
                }
                 */
            }
            catch (Exception myE)
            {
                //m_Log.Error("applyFeedStatus", myE);
            }
        }
        /// <summary>
        /// Handle channel status messages these are from the TCP Comms object
        /// e.g. when it cannot connect, or the connection fails
        /// </summary>
        /// <param name="myMessage"></param>
        private void OnChannelStatusMessage(K2Gateway.KaiMessageWrap myWrap)
        {
            lock (this)
            {
                try
                {
                    // only report changed status
                    if (m_ChannelState != myWrap.Label)
                    {
                        // get the name of the feed
                        string myGWName = myWrap.Tag;

                        string myGWState = myWrap.Label;

                        string myGWPoint = myWrap.Data;
                        m_RTDForm.ConnectionStatus = myGWName + ":" + myGWState + ":" + myGWPoint;
                        applyGWStatus(myGWName, myGWState);


                        // We really want to know if the channel is open or not

                        m_ChannelState = myWrap.Label;
                        if (m_ChannelState == K2Gateway.GatewayStatus.OPEN)
                        {
                            m_RTDForm.ConnectionStatus = "resendQueuedMessages";
                            SetConnectionState(RTDConnectionState.connected);
                            lock (s_RequestLock)
                            {
                                ReplayRequests(m_PendingDataConnect);
                            }

                        }
                    }
                    
                }
                catch (Exception myE)
                {
                    //m_Log.Error("OnChannelStatusMessage", myE);
                }
            }

        }

        /// <summary>
        /// resend any messages that have been queued, for example
        /// requests made for a dead tcp/ip connection
        /// </summary>
        private void resendQueuedMessages()
        {
            try
            {
                K2Gateway.KaiMessageWrap myQMsg;
                Timer myTimer = new Timer();
                myTimer.Elapsed += new ElapsedEventHandler(HandleTimerEvent);
                // wait 5 secs - to allow the server more time to start up
                myTimer.Interval = 1000;
                myTimer.Start();
            }
            catch (Exception myE)
            {
                //m_Log.Error("resendQueuedMessages", myE);
            }

        }

        public void HandleTimerEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                lock (m_MsgQueue)
                {
                    // since we are open if there are any messages in the
                    // queue send them
                    K2Gateway.KaiMessageWrap myQMsg;
                    while (m_MsgQueue.Count > 0)
                    {
                        myQMsg = m_MsgQueue.Peek() as K2Gateway.KaiMessageWrap;
                        if (sendChannelRequest(myQMsg))
                        {
                            m_MsgQueue.Dequeue();
                            m_RTDForm.ConnectionStatus = "Dequeue:" + m_MsgQueue.Count.ToString();
                        }
                        System.Threading.Thread.Sleep(100);

                    }
                }
            }
            catch (Exception myE)
            {
                //m_Log.Error("HandleTimerEvent", myE);
            }
        }

        /// <summary>
        /// Handle an incomming RTD messsage received from the server- this will contain
        /// data that may be used in excel
        /// </summary>
        /// <param name="myMessage"></param>
        private void OnRTDMessage(K2Gateway.KaiMessageWrap myWrap)
        {
            lock (this)
            {
                try
                {
                    if (myWrap.Label == "HB")
                    {
                        m_LastHBTicks = long.Parse(myWrap.Data);
                        m_TripTime = DateTime.Now.Ticks - long.Parse(myWrap.Data);
                    }
                    else
                    {
                        string[] dataValues;
                        K2Gateway.StreamHelper.GetStringArray(out dataValues, myWrap.Data);

                        int len = dataValues.Length / 2;
                        int[] ids = new int[len];

                        string[] values = new string[len];
                        int j = 0;
                        for (int i = 0; i < len; i++)
                        {
                            ids[i] = int.Parse(dataValues[j++]);
                            values[i] = dataValues[j++];
                        }

                        this.OnTopicUpdate(ids, values);
                    }
                }
                catch (Exception myE)
                {
                    ////m_Log.Error("OnRTDMessage", myE);
                }
            }

        }

        /// <summary>
        /// Set and open a file to dump incomming data
        /// </summary>
        public string DumpDataPath
        {
            get
            {
                return m_Path;
            }
            set
            {
                try
                {
                    m_Path = value;
                    closeFile();

                    // if the path is some value try open it
                    if (m_Path.Length > 0)
                    {
                        openFile();
                        m_DumpData = true;
                    }
                    else
                    {
                        m_DumpData = false;
                    }
                }
                catch (Exception myE)
                {
                    m_DumpData = false;
                    //m_Log.Warn("DumpDataPath", myE);
                }
            }
        }

        /// <summary>
        /// Returns if we are dumping data 
        /// </summary>
        public bool DumpData
        {
            get
            {
                return m_DumpData;
            }
        }

        private void openFile()
        {
            // open the file - and append

            m_FileStream = new FileStream(m_Path, FileMode.Append);
            m_Writer = new StreamWriter(m_FileStream);

        }

        private void closeFile()
        {
            try
            {
                m_Writer.Flush();
                m_Writer.Close();
                m_FileStream.Close();
            }
            catch (Exception myE)
            {
                //m_Log.Warn("closeFile", myE);
            }
        }

        /// <summary>
        /// Write a message to file
        /// </summary>
        /// <param name="myMsg"></param>
        private void dumpData(K2Gateway.KaiMessageWrap myWrap)
        {
            try
            {



            }
            catch (Exception myE)
            {
                //m_Log.Error("dumpData", myE);
            }
        }

        /// <summary>
        /// create an RTD Subject for status
        /// </summary>
        private void initRTDStatusSubject()
        {
            try
            {
                

            }
            catch (Exception myE)
            {
                //m_Log.Error("initRTDStatusSubject", myE);
            }
        }

        /// <summary>
        /// Send a log message to the RTD Server support, the svr support 
        /// can then optionally log that
        /// </summary>
        /// <param name="myMsg"></param>
        public void SendLogMessage(string myMsg)
        {
            try
            {
                //m_Log.Info(myMsg);
                K2Gateway.KaiMessageWrap myWrap = new K2Gateway.KaiMessageWrap();
                myWrap.Data = myMsg;
                myWrap.Label = "RTDLogMessage";
                //sendRequest("DEFAULT", myWrap);
            }
            catch (Exception myE)
            {
                //m_Log.Error("SendLogMessage", myE);
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
            ////m_Log.Info("RefreshData:In");
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
                    //m_Log.Error("RefreshData", myE);
                }

            }
            return null;
        }

        #endregion

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

        public void StartServer(string appID)
        {
            try
            {
                m_CorrelationID = appID;
                Start();

                // //m_Log.Info("StartServer:" + m_CorrelationID);
            }
            catch (Exception myE)
            {
                //m_Log.Error("StartServer", myE);
            }
        }

        private void sendDataReq(DataConnectionRequest ds)
        {
            try
            {
                
                // frame up a request to md sent over the channel to the tool kit
                K2Gateway.KaiMessageWrap myWrap = new K2Gateway.KaiMessageWrap();
                myWrap.Label = "ConnectData";
                string[] env = new string[2];
                env[0] = ds.AccessID.ToString();
                env[1] = ds.TopicID.ToString();
                string myData = K2Gateway.StreamHelper.GetAsString(env);
                myData += "\t";
                myData += K2Gateway.StreamHelper.GetAsString(ds.Strings);
                myWrap.Data = myData;

                sendRequest(m_DefaultFeedName, myWrap);
                
            }
            catch (Exception myE)
            {
            }
        }
        private void sendDataReq(string label, string data)
        {
            try
            {
                // frame up a request to md sent over the channel to the tool kit
                K2Gateway.KaiMessageWrap myWrap = new K2Gateway.KaiMessageWrap();
                myWrap.Label = label;
                myWrap.Data = data;
                sendRequest(m_DefaultFeedName, myWrap);

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

        /// <summary>
        /// Handle the connect data request - this is the primary way
        /// we make requests from Excel
        /// </summary>
        /// <param name="TopicID"></param>
        /// <param name="Strings"></param>
        public void ConnectData(int TopicID, string[] Strings)
        {
            lock (s_RequestLock)
            {
                try
                {

                    string myReqType = Strings.GetValue(0) as string;
                    string subjectID = Strings.GetValue(1) as string;
                    string headerName = Strings.GetValue(2) as string;
                    ////m_Log.Info("ConnectData:In" + myReqType + ":" + subjectID + ":" + headerName);
                    m_RTDForm.LastRequest = "ConnectData:In" + myReqType + ":" + subjectID + ":" + headerName;
                    DataConnectionRequest ds = new DataConnectionRequest(m_CorrelationID, TopicID, subjectID, headerName, Strings);

                    // We keep track for any request to published streams
                    // one off requests are not resent or rerequested
                    if (isUpdateRequest(myReqType))
                    {
                        if (headerName == "K2RTDStatus")
                        {
                            //this.StatusID = TopicID;
                            //updateCommState();
                        }

                        m_Subscriptions.Add(ds);
                    }
                    if (m_PendingDataConnect == null)
                    {
                        m_PendingDataConnect = new List<DataConnectionRequest>();
                    }
                    m_PendingDataConnect.Add(ds);


                    if (m_DefaultClientGW.State == K2Gateway.State.open)
                    {
                        if (m_PendingDataConnect.Count > 0)
                        {
                            ReplayRequests(m_PendingDataConnect);
                            m_PendingDataConnect.Clear();
                        }

                    }




                }
                catch (Exception myE)
                {
                    ////m_Log.Error("ConnectData", myE);
                }
            }
        }

       
        private void sendRequest(string myChannelName, K2Gateway.KaiMessageWrap myWrap)
        {
            myWrap.TargetID = myChannelName;
            myWrap.ClientID = (m_ClientID++).ToString();
            // get the channel
            if (m_ClientGWs.ContainsKey(myChannelName))
            {
                K2Gateway.TCPClientGateway myGW = m_ClientGWs[myChannelName] as K2Gateway.TCPClientGateway;

                if (myGW.State == K2Gateway.State.open)
                {
                    myGW.Send(myWrap);
                }
                else
                {
                    // we will queue the request that cannot be sent - note that this is an exception condition 
                    // since the ConnectData check the state of the GW before sending the request
                    if (myWrap.Label != "HB")
                    {
                        m_MsgQueue.Enqueue(myWrap);
                        m_RTDForm.ConnectionStatus = "Enqueue:" + m_MsgQueue.Count.ToString();
                    }
                }
            }
            else
            {
                if (m_DefaultClientGW != null)
                {
                    myWrap.TargetID = "DEFAULT";
                    if (m_DefaultClientGW.State == K2Gateway.State.open)
                    {
                        m_DefaultClientGW.Send(myWrap);
                    }
                    else
                    {
                        //  queue the susbcription request  - will be sent if and
                        // when the channel opens
                        m_MsgQueue.Enqueue(myWrap);
                        m_RTDForm.ConnectionStatus = "Enqueue:" + m_MsgQueue.Count.ToString();
                    }
                    ////m_Log.Warn("sendRequest for feed:" + myChannelName + " using default channel");
                }
                else
                {
                    ////m_Log.Error("sendRequest for feed:" + myChannelName + " no default channel available");
                }
            }
        }
        public void ReplaySubscriptions()
        {
            try
            {
                ReplayRequests(m_Subscriptions);
            }
            catch (Exception myE)
            {
            }
        }

        public void ReplayRequests(List<DataConnectionRequest> reqList)
        {
            try
            {
                foreach (DataConnectionRequest ds in reqList)
                {
                    this.sendDataReq(ds);
                }
                reqList.Clear();

            }
            catch (Exception myE)
            {
            }
        }

         
        public void UnSubscribeData(int myID)
        {
            try
            {
                sendDataReq("UnSubscribeData", myID.ToString());
            }
            catch (Exception myE)
            {
            }
        }
         

       


    }
}

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
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace K2RTDRequestHandler
{
 
    public class RTDServer : K2RTDServerKit.IRTDServer
    {
        // Declare the event using EventHandler<T>
        public event EventHandler<K2RTDServerKit.RequestEventArgs> RTDRequestEvent;

        public event EventHandler<K2RTDServerKit.StatusEventArgs> RTDStatusEvent;




        /// <summary>
        /// used to lock the inbound message proc
        /// </summary>
        private object m_MessageProc = new object();

        /// <summary>
        /// Status delegates
        /// </summary>
        private K2RTDServerKit.OnStatusUpdate m_OnStatusUpdate = null;

        /// <summary>
        /// Request delegates
        /// </summary>
        private K2RTDServerKit.OnRequest m_OnRequest = null;

        

        /// <summary>
        /// Current state
        /// </summary>
        private Status m_State;

        private Dictionary<string, K2RTDPushSubscriber> m_Obsevers;


        /// <summary>
        /// Used for the watchdog process - sends heartbeats etc
        /// </summary>
        System.Threading.Thread m_WatchDogThread;

        /// <summary>
        /// Used to control when the thread runs
        /// </summary>
        bool m_bRunWatchDog = false;

        /// <summary>
        /// State passed in during the start
        /// </summary>
        string m_StartState = "";

     

        /// <summary>
        /// The channel used for comms to the RTD server
        /// </summary>
        private K2Gateway.TCPSvrGateway m_SvrGW;

        // Create a logger for use in this class
        public static log4net.ILog m_Log;



        public RTDServer()
        {
            try
            {
                m_Log = log4net.LogManager.GetLogger("RTDRequestHandler");
                m_Obsevers = new Dictionary<string, K2RTDPushSubscriber>();
            }
            catch (Exception myE)
            {
            }
        }

        public K2RTDServerKit.OnStatusUpdate OnStatusUpdateHandler
        {
            get { return m_OnStatusUpdate; }
            set { m_OnStatusUpdate = value; }
        }

        public K2RTDServerKit.OnRequest OnRequestHandler
        {
            get { return m_OnRequest; }
            set { m_OnRequest = value; }
        }

       

        /// <summary>
        /// Start the handler running - will allow KaiTrade RTDs to connect to the server
        /// </summary>
        /// <param name="myState">for future use</param>
        public  void StartHandler(string myState)
        {
            try
            {
                m_StartState = myState;

                m_WatchDogThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.wdProc));
                m_bRunWatchDog = true;
                m_WatchDogThread.Start();

                start();
                 
            }
            catch (Exception myE)
            {
                m_Log.Error("Driver.Start:", myE);
            }
        }

        /// <summary>
        /// Stop the hander - all sessions will be closed
        /// </summary>
        public void StopHandler()
        {
            try
            {
                m_SvrGW.Close();
                m_SvrGW = null;
                m_bRunWatchDog = false;
                m_WatchDogThread.Abort();
                m_WatchDogThread = null;
                m_State = Status.closed;

            }
            catch (Exception myE)
            {
                m_Log.Error("StopHandler", myE);
            }
        }

        /// <summary>
        /// watch dog process - not used in this release
        /// </summary>
        private void wdProc()
        {
            try
            {
                while (m_bRunWatchDog)
                {
                    System.Threading.Thread.Sleep(10000);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("wdProc", myE);
            }
        }



        /// <summary>
        /// start the channel for incomming requests from RTD
        /// </summary>
        /// <param name="myState"></param>
        private void start()
        {
            // give the other adapters some time to start - since when we open the channel we 
            // may get a set of subscription requests that the need to be processed by various 
            // adapters
            System.Threading.Thread.Sleep(5000);
            try
            {
                int myPort;
                //myPort = (int) m_Config.ListenPort;
                myPort = 11001;
                startSvrGW(myPort);

                // our RTD Status obsever works over the channel
                //m_RTDStatus.SvrGW = m_SvrGW;

                m_State = Status.open;

              

            }
            catch (Exception myE)
            {
                m_State = Status.error;
                
                m_Log.Error("KaiRTD Helper is not working", myE);
            }

           
           
        }

        /// <summary>
        /// Start the TCP server gateway
        /// </summary>
        /// <param name="myPort">port that we will listen on</param>
        private void startSvrGW(int myPort)
        {

            m_SvrGW = new K2Gateway.TCPSvrGateway(myPort);
            m_SvrGW.OnMessage += new K2Gateway.OnMessageEvent(OnMessage);
            m_SvrGW.OnStatus += new K2Gateway.OnStatusEvent(OnStatusMessage);
            m_SvrGW.Open();
            m_SvrGW.Begin();
        }


        /// <summary>
        /// Publish an update for the subject and header specified
        /// </summary>
        /// <param name="subject">subject that you want to publish data against - e.g. IBM</param>
        /// <param name="header">header that the data is for - e.g. QUANTITY </param>
        /// <param name="value">value you want to publish</param>
        public void Publish(string subject, string header, string value)
        {
            try
            {
                IPublisher myPub = null;

                myPub = K2RTDRequestHandler.PublisherMgr.Instance().GetPublisher("Publisher", subject, true);

                if (myPub != null)
                {
                    myPub.OnUpdate(header, value);

                }
            }
            catch(Exception myE)
            {
            }
        }

        /// <summary>
        /// publish an array of values
        /// </summary>
        /// <param name="subject">subject that you want to publish data against - e.g. IBM</param>
        /// <param name="values">array of double values</param>
        public void Publish(string subject, double[] values)
        {
            try
            {
                IPublisher myPub = K2RTDRequestHandler.PublisherMgr.Instance().GetPublisher("Publisher", subject, true);
                List<Field> fieldList = new List<Field>();
                int i = 0;
                foreach(double v in values)
                {
                    Field field = new K2RHField(i++.ToString(), v.ToString());
                    fieldList.Add(field);
                }
                if (myPub != null)
                {
                    myPub.OnUpdate(fieldList);

                }
            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// publish an array of int values
        /// </summary>
        /// <param name="subject">subject that you want to publish data against - e.g. IBM</param>
        /// <param name="values">array of int values</param>
        public void Publish(string subject, int[] values)
        {
            try
            {
                IPublisher myPub = K2RTDRequestHandler.PublisherMgr.Instance().GetPublisher("Publisher", subject, true);
                List<Field> fieldList = new List<Field>();
                int i = 0;
                foreach (int v in values)
                {
                    Field field = new K2RHField(i++.ToString(), v.ToString());
                    fieldList.Add(field);
                }

                if (myPub != null)
                {
                    myPub.OnUpdate(fieldList);

                }
            }
            catch (Exception myE)
            {
            }
        }
        

        /// <summary>
        /// Handle incomming status messages from the comm channel
        /// </summary>
        /// <param name="myMessage"></param>
        private void OnStatusMessage(K2Gateway.KaiMessageWrap myWrap)
        {
            try
            {
                // We really want to know if the channel is open or not
                string myChannelState = myWrap.Label;
                string myStateText = "";
                
                if (myChannelState == "OPEN")
                {
                    m_State = Status.open;
                    myStateText = "Kai RTD Helper is  Listening";

                }
                else
                {
                    m_State = Status.closed;
                    myStateText = "Kai RTD Helper channel state is:" + myChannelState;

                }
                K2RTDServerKit.StatusEventArgs e = new K2RTDServerKit.StatusEventArgs(myChannelState, myStateText);
                if (m_OnStatusUpdate != null)
                {
                    
                    m_OnStatusUpdate(this, e);
                }
                // Make a temporary copy of the event to avoid possibility of
                // a race condition if the last subscriber unsubscribes
                // immediately after the null check and before the event is raised.
                EventHandler<K2RTDServerKit.StatusEventArgs> handler = RTDStatusEvent;

                // Event will be null if there are no subscribers
                if (handler != null)
                {
                    // Format the string to send inside the CustomEventArgs parameter
                    // e.Message += String.Format(" at {0}", DateTime.Now.ToString());

                    // Use the () operator to raise the event.
                    handler(this, e);
                }
                 
            }
            catch (Exception myE)
            {
                m_Log.Error("OnStatusMessage", myE);
            }

        }

        /// <summary>
        /// Handle incomming messages from the channel - these should be from the RTD server fo
        /// this application
        /// </summary>
        /// <param name="msg"></param>
        private void OnMessage(K2Gateway.KaiMessageWrap myWrap)
        {
            try
            {
                lock (m_MessageProc)
                {
                    if (myWrap.Label == "ConnectData")
                    {
                        string[] inStrings;
                        K2Gateway.StreamHelper.GetStringArray(out inStrings, myWrap.Data);
                     
                        this.ConnectData(myWrap.CorrelationID, inStrings);
                    }
                    else if (myWrap.Label == "HB")
                    {
                        // send it back
                        SendTCP(myWrap.CorrelationID, "HB", myWrap.Data);
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("OnMessage", myE);
            }
        }

        /// <summary>
        /// Handle requests from RTD - this routes the request to any subscribers of the
        /// handler
        /// </summary>
        /// <param name="accessID"></param>
        /// <param name="Strings"></param>
        private void ConnectData(string accessID, string[] Strings)
        {
            
            try
            {
 
                string topicID = Strings.GetValue(1) as string;

                int rtdTopicId = int.Parse(topicID);
                K2RTDServerKit.RequestEventArgs e = new K2RTDServerKit.RequestEventArgs(accessID, rtdTopicId, Strings);
                if (m_OnRequest != null)
                {
                    
                    m_OnRequest(this, e);

                }
                // Make a temporary copy of the event to avoid possibility of
                // a race condition if the last subscriber unsubscribes
                // immediately after the null check and before the event is raised.
                EventHandler<K2RTDServerKit.RequestEventArgs> handler = RTDRequestEvent;

                // Event will be null if there are no subscribers
                if (handler != null)
                {
                    // Format the string to send inside the CustomEventArgs parameter
                    // e.Message += String.Format(" at {0}", DateTime.Now.ToString());

                    // Use the () operator to raise the event.
                    handler(this, e);
                }
               
            }
            catch (Exception myE)
            {
                m_Log.Error("ConnectData", myE);
            }

        }

        /// <summary>
        /// Get a string array from tab delimited data
        /// </summary>
        /// <param name="myStringArray"></param>
        /// <param name="myData"></param>
        static public void GetStringArray(out string[] myStringArray, string myData)
        {
            myStringArray = myData.Split('\t');


        }

        /// <summary>
        /// convert a string array to a tab delimted string
        /// </summary>
        /// <param name="myStringArray"></param>
        /// <returns></returns>
        public string GetAsString(string[] myStringArray)
        {
            string myString = "";
            bool bFirst = true;
            foreach (string myVal in myStringArray)
            {
                if (bFirst)
                {
                    bFirst = false;
                }
                else
                {
                    myString += "\t";
                }
                myString += myVal;
            }


            return myString;


        }

        /// <summary>
        /// Send a heartbeat to Excel
        /// </summary>
        /// <param name="accessID"></param>
        /// <param name="data"></param>
        public void HeartBeat(string accessID, string data)
        {
            try
            {
                K2Gateway.KaiMessageWrap myWrap = new K2Gateway.KaiMessageWrap();
                myWrap.Data = data;
                myWrap.Label = "HeartBeat";
                myWrap.CorrelationID = accessID;
                m_SvrGW.Send(ref myWrap);


            }
            catch (Exception myE)
            {
                m_Log.Error("HeartBeat", myE);
            }

        }

        /// <summary>
        /// send the string to the app with the accessID sepcified
        /// </summary>
        /// <param name="accessID"></param>
        /// <param name="label"></param>
        /// <param name="data"></param>
        private void SendTCP(string accessID, string label, string data)
        {
            try
            {
                K2Gateway.KaiMessageWrap myWrap = new K2Gateway.KaiMessageWrap();
                myWrap.Data = data;
                myWrap.Label = label;
                myWrap.CorrelationID = accessID;
                m_SvrGW.Send(ref myWrap);


            }
            catch (Exception myE)
            {
                m_Log.Error("SendTCP", myE);
            }
        }

        /// <summary>
        /// Single topic update- this allows you to explicitly set
        /// some Excel topics. Note that in general you should use a publish method
        /// </summary>
        /// <param name="accessID">access id for the particular instance of Excel</param>
        /// <param name="id">excel topic id</param>
        /// <param name="value">value</param>
        public void TopicUpdate(string accessID, int id, string value)
        {
            try
            {
                int[] idArray = new int[1];
                idArray[0] = id;

                string[] valueArray = new string[1];
                valueArray[0] = value;

                TopicUpdate(accessID, idArray, valueArray);

            }
            catch (Exception myE)
            {
                m_Log.Error("TopicUpdate - single", myE);
            }
        }



        /// <summary>
        /// Update a set of topics in Excel - this allows you to explicitly set
        /// some Excel topics. Note that in general you should use a publish method
        /// </summary>
        /// <param name="accessID">access id for the particular instance of Excel</param>
        /// <param name="id">array of excel topic ids to update</param>
        /// <param name="value">array of values to write into the topics</param>
        public void TopicUpdate(string accessID, int[] id, string[] value)
        {
            try
            {
                string[] dataArray = new string[id.Length * 2];
                int j = 0;
                for (int i = 0; i < id.Length; i++)
                {
                    dataArray[j++] = id[i].ToString();
                    dataArray[j++] = value[i];
                }
                string sendData = GetAsString(dataArray);
                SendTCP(accessID, "TopicUpdate", sendData);

            }
            catch (Exception myE)
            {
                m_Log.Error("TCPUpdate-array", myE);
            }
        }




        /// <summary>
        /// Subscribe based on the topic def and monitor changes for the correlation ID
        /// if the subject is open then the corrID will be added to the interested parties
        /// </summary>
        /// <param name="myTopicDef">This is the identifer for the product or subject required</param>
        /// <param name="myCorrelationID">used to identify the channel used to respond on</param>
        public void Subscribe(string subject, string headerName, int excelID, string accessID)
        {
            try
            {
                RTDStatusReport("LASTREQ", "Subscribe:" + subject);

                
                IPublisher myPub = null;

                myPub = PublisherMgr.Instance().GetPublisher("Publisher", subject, true);

                if (myPub != null)
                {
                    K2RTDPushSubscriber  myObs;
                    getObserver(out myObs, subject);
                    myObs.AddAccessID(accessID);
                    myObs.AddHeader(accessID, headerName, excelID);
                    myPub.Subscribe(myObs);
                    myPub.Open(subject);
                    m_Log.Info("K2RTDDriver:Subscribed:" + myPub.TopicID());
                    //this.SendAdvisoryMessage("K2RTDDriver:Subscribed:" + myPub.TopicID());
                }
                else
                {
                    throw new Exception("No publisher returned from KaiTrade facade");
                }

            }
            catch (Exception myE)
            {
                RTDStatusReport("LASTREQSTA", "FAIL");
                RTDStatusReport("LASTREQDETAIL", myE.ToString());
                m_Log.Error("Subscribe:", myE);
                m_Log.Error("K2RTDDriver:Subscribed:fail:" + subject);
            }

        }
        /// <summary>
        /// Report a status field to the RTD Support accross the channel
        /// </summary>
        /// <param name="myHeader"></param>
        /// <param name="myValue"></param>
        private void RTDStatusReport(string myHeader, string myValue)
        {
            try
            {
                System.Collections.Generic.List<Field> myList = new System.Collections.Generic.List<Field>();
                K2RHField myField = new K2RHField(myHeader, myValue);
                myList.Add(myField);
                //(m_RTDStatus as Subscriber).OnImage(this myList);
            }
            catch (Exception myE)
            {

            }
        }

        /// <summary>
        /// Get or add a push observer using the ID
        /// </summary>
        /// <param name="myObs"></param>
        /// <param name="myID"></param>
        private void getObserver(out K2RTDPushSubscriber myObs, string myID)
        {
            if (m_Obsevers.ContainsKey(myID))
            {
                myObs = m_Obsevers[myID] as K2RTDPushSubscriber;
            }
            else
            {
                myObs = new K2RTDPushSubscriber(this);
                myObs.SubjectID = myID;
                m_Obsevers.Add(myObs.SubjectID, myObs);
            }

        }

        private string trimMnemonic(string myMnemonic)
        {
            string myRet = "";
            try
            {
                myRet = myMnemonic.Split(' ')[0];
            }
            catch (Exception myE)
            {
            }
            return myRet;
        }

        /*
        public  void publishPrices(string myMnemonic, string myOfferSz, string myOfferPx, string myBidPx, string myBidSz, string myTradePx, string myTradeSz)
        {
             
            try
            {
                Facade myFacade = KTAFacade.KTAFacade.Instance();

                // trim the mnemonic removes anything after a space
                myMnemonic = trimMnemonic(myMnemonic);

                // try get the product and create one if needed - this wont work for all Venues
                myFacade.CheckCreateMnemonic(myMnemonic);

                // get a PX publisher for the mnemonic
                KaiTrade.TradeObjects.PXPublisher myPub = myFacade.GetPXPublisher(myMnemonic) as KaiTrade.TradeObjects.PXPublisher;
                if (myPub != null)
                {
                    myPub.SetField(MDConstants.BID1_PRICE, myBidPx);
                    myPub.SetField(MDConstants.BID1_SIZE, myBidSz);
                    myPub.SetField(MDConstants.OFFER1_PRICE, myOfferPx);
                    myPub.SetField(MDConstants.OFFER1_SIZE, myOfferSz);
                    myPub.SetField(MDConstants.TRADE_PRICE, myTradePx);
                    myPub.SetField(MDConstants.TRADE_SIZE, myTradeSz);
                    (myPub as Publisher).OnUpdate(null);
                }

            }
            catch (Exception myE)
            {
                m_DriverLog.Error("publishPrices", myE);
            }
              
        }
    */

        

        /// <summary>
        /// Get a list of the available subjects
        /// </summary>
        /// <returns>List of subject names</returns>
        public List<string> GetSubjects()
        {
            List<string> subjects = new List<string>();
            try
            {
                subjects = PublisherMgr.Instance().GetPublisherSubjects();
            }
            catch (Exception myE)
            {

                m_Log.Error("GetSubjects:", myE);

            }
            return subjects;
        }


    }
}

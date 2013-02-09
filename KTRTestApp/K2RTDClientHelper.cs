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
using System.Drawing;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace KTRTestApp
{
    /// <summary>
    /// provides helpe functions to use KTARemote
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class K2RTDClientHelper : ServiceReference2.IK2RTDSupportCallback, IDisposable
    {
        private ServiceReference2.K2RTDSupportClient m_Client = null;
        string m_ID = "K2RTD";

        /// <summary>
        /// used to lock the class during updates and reads
        /// </summary>
        private static object s_UpdateLock = new object();

        Dictionary<int, object> m_TopicUpdates;

        public K2RTDClientHelper()
        {

            m_TopicUpdates = new Dictionary<int, object>();
            
        }

        public void Connect()
        {
            try
            {

                
                if (m_Client == null)
                {
                    InstanceContext context = new InstanceContext(this);
                    //m_Client = new ServiceReference2.K2RTDSupportClient(context, "NetTcpBinding_IK2RTDSupport");
                    //Specify the binding to be used for the client.
                    System.ServiceModel.NetTcpBinding binding = new NetTcpBinding();
                    

                    //Specify the address to be used for the client.
                    EndpointAddress address = new EndpointAddress("net.tcp://localhost:11000/");
                    m_Client = new ServiceReference2.K2RTDSupportClient(context, binding, address);
                    //m_Client = new ServiceReference2.K2RTDSupportClient(context, "NetTcpBinding_IK2RTDSupport");
                }
            }
            catch (Exception myE)
            {
            }
        }

        ServiceReference2.K2RTDSupportClient Client
        {
            get
            {
                if (m_Client == null)
                {
                    Connect();
                }
                return m_Client;
            }
        }
       

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion

        private void addTopicUpdate(int topicID, string topicValue, int topicType)
        {
            lock (s_UpdateLock)
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
        }

        public Array RefreshData(ref int TopicCount)
        {
            lock (s_UpdateLock)
            {
                try
                {
                    object[,] myData = new object[m_TopicUpdates.Count, 2];
                    TopicCount = 0;
                    foreach (int topicId in m_TopicUpdates.Keys)
                    {
                        myData[TopicCount, 0] = topicId;
                        myData[TopicCount, 1] = m_TopicUpdates[topicId];
                        TopicCount++;
                    }

                    m_TopicUpdates.Clear();

                    return myData;
                }
                catch (Exception myE)
                {
                    TopicCount = 0;
                    //KRTDSupport.KRTDSupport.Instance().Log.Error("Microsoft.Office.Interop.Excel.IRtdServer.RefreshData", myE);
                }
               
            }
            return null;
        }

        #region IK2RTDSupportCallback Members

        public void OnTopicUpdate(int[] id, string[] value)
        {
            try
            {

                for (int i = 0; i < id.Length; i++)
                {
                    addTopicUpdate(id[i], value[i], 0);
                }
            }
            catch (Exception myE)
            {
            }
        }

        public void OnMessage(KTRTestApp.ServiceReference2.KTAMessage myMsg)
        {
            string cc = (myMsg.m_Label + ":" + myMsg.m_Data);
            //m_Results.Items.Add(cc);
        }
        #endregion

        public void Register(string myID)
        {
            //if(m_Client.
            Client.Register(m_ID);
        }

        public void Log(string myAccessID, bool isError, string src, string msg)
        {
            Client.Log(myAccessID, isError, src, msg);
        }

        public void SubscribeData(string myAccessID, int myID, string myTopicID, string myHeaderName)
        {
            Client.SubscribeData(myAccessID, myID, myTopicID, myHeaderName);
        }

        public void SubmitOrder(string myAccessID, int myID, string myTopicID, string myHeaderName, string[] myOrderParms)
        {
            Client.SubmitOrder(myAccessID, myID, myTopicID, myHeaderName, myOrderParms);
        }

        public void PublishTrigger(string myAccessID, int myID, string myTopicID, string myHeaderName, string[] myTriggerParms)
        {
            Client.PublishTrigger(myAccessID, myID, myTopicID, myHeaderName, myTriggerParms);
        }

        public void GPPublishRequest(string myAccessID, int myID, string myTopicID, string myHeaderName, string[] myParms)
        {
            Client.GPPublishRequest(myAccessID,myID, myTopicID, myHeaderName, myParms);
        }

        public void UnSubscribeData(string myAccessID, int myID)
        {
            Client.UnSubscribeData(myAccessID, myID);
        }

        public void Init(string myAccessID)
        {
            Client.Init(myAccessID);
        }

        public void Finalize(string myAccessID)
        {
            Client.Finalize(myAccessID);
        }
    }
}

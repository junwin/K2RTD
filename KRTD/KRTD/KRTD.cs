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
using System.Text;
using System.Runtime.InteropServices;
using System.Timers;

namespace KRTD
{
    [
    Guid("84E1D19E-5F01-4E19-AA3A-AA74AE1E63FD"),
    ProgId("Kaitrade.K2RTD"),
    ]
    [ComVisible(true)]
    public class KRTD : Microsoft.Office.Interop.Excel.IRtdServer
    {

        private Microsoft.Office.Interop.Excel.IRTDUpdateEvent m_callback;
        //private K2BaseSvrClient.K2RTDClientHelper m_RTDSupport;
        private K2BaseSvrClient.K2SVRRTDSvrClient m_RTDSupport;
        //private string m_AccessID = "";
       

        /// <summary>
        /// Called any time there is a change in our data
        /// </summary>
        /// <param name="msg"></param>
        public void OnUpdateNofiy()
        {
            try
            { 
                m_callback.UpdateNotify();
            }
            catch (Exception myE)
            {
                //KRTDSupport.KRTDSupport.Instance().Log.Error("OnChange", myE);
            }
        }


        private void asStringArray(ref string[] outData, Array Strings)
        {
            try
            {
                int len = Strings.Length;
                outData = new string[len];
                for (int i = 0; i < len; i++)
                {
                    outData[i] = Strings.GetValue(i) as string;
                }
            }
            catch (Exception myE)
            {
            }
        }
     
        #region IRtdServer Members

        object Microsoft.Office.Interop.Excel.IRtdServer.ConnectData(int TopicID, ref Array Strings, ref bool GetNewValues)
        {
            object myRet = null;
            try
            {
                string myReqType = Strings.GetValue(0) as string;
                string myTopicID = Strings.GetValue(1) as string;
                string myHeaderName = Strings.GetValue(2) as string;
                string[] stringArray = null;
                
                
                switch (myReqType)
                {
                   
                    case "RESUB":
                        // do a resubscribe
                        m_RTDSupport.ReplaySubscriptions();
                        
                        break;
                    case "RESUBGW":
                        // do a resubscribe for a gateway
                        //KRTDSupport.KRTDSupport.Instance().ReSubscribeGW(myTopicID);
                        break;
                   
                    case "NOP":
                        // ignore the operation
                        break;
                    case "PX":
                        // ignore the operation
                       asStringArray(ref stringArray, Strings);
                        m_RTDSupport.ConnectData(TopicID, stringArray);
                        break;
                    default:
                        asStringArray(ref stringArray, Strings);
                        m_RTDSupport.ConnectData(TopicID, stringArray);
                        myRet = myTopicID;
                        break;
                }


                // force excel to use the new value
                GetNewValues = true;
            }
            catch (Exception myE)
            {
                //KRTDSupport.KRTDSupport.Instance().Log.Error("Microsoft.Office.Interop.Excel.IRtdServer.ConnectData", myE);
            }
            return myRet;

        }

        void Microsoft.Office.Interop.Excel.IRtdServer.DisconnectData(int TopicID)
        {
            try
            {
                m_RTDSupport.UnSubscribeData( TopicID);
                
            }
            catch (Exception myE)
            {
                //KRTDSupport.KRTDSupport.Instance().Log.Error("Microsoft.Office.Interop.Excel.IRtdServer.DisconnectData", myE);
            }
           
        }

        int Microsoft.Office.Interop.Excel.IRtdServer.Heartbeat()
        {
            return 1;
        }

        Array Microsoft.Office.Interop.Excel.IRtdServer.RefreshData(ref int TopicCount)
        {
            try
            {
                
                return m_RTDSupport.RefreshData(ref TopicCount);

               
            }
            catch (Exception myE)
            {
                TopicCount = 0;
                //KRTDSupport.KRTDSupport.Instance().Log.Error("Microsoft.Office.Interop.Excel.IRtdServer.RefreshData", myE);
            }
            return null;
        }

        int Microsoft.Office.Interop.Excel.IRtdServer.ServerStart(Microsoft.Office.Interop.Excel.IRTDUpdateEvent CallbackObject)
        {
            try
            {
                m_RTDSupport = K2BaseSvrClient.K2SVRRTDSvrClient.Instance();

                m_callback = CallbackObject;
                m_RTDSupport.UpdateNotify += new K2BaseSvrClient.OnUpdateNotify(OnUpdateNofiy);
                m_RTDSupport.StartServer("K2RTDX");

            }
            catch (Exception myE)
            {
                
            }
            
            return 1;
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private  void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                //KRTDSupport.KRTDSupport.Instance().ReSubscribe("*", 0);
                
            }
            catch (Exception myE)
            {
            }
        }

        void Microsoft.Office.Interop.Excel.IRtdServer.ServerTerminate()
        {
            try
            {
                m_RTDSupport.Finalize();
                m_RTDSupport = null;
            }
            catch (Exception myE)
            {
                
            }
        }

        #endregion
    }
}

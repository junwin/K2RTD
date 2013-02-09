/***************************************************************************
 *    
 *      Copyright (c) 2009,2010,2011,2012 KaiTrade LLC (registered in Delaware)
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
using System.Linq;
using System.Text;

namespace K2RTDRequestHandler
{
    /// <summary>
    /// This class provides the means to push some multi field update out to all
    /// interested Excel RTD calls for a spcific topic or subject. It does this by maintaining a list of headers
    /// against some common subject/topic, for Example differenet fields for some stock
    /// 
    /// </summary>
    public class K2RTDPushSubscriber : ISubscriber
    {
        /// <summary>
        /// This references the handler used for communications with remote clients
        /// </summary>
        private RTDServer  m_RequestHandler;

        /// <summary>
        /// subject ID for this particular observer - this is not the Excel topic ID
        /// it is a string that identifies the subject that we are interested in
        /// </summary>
        private string m_SubjectID;

        /// <summary>
        /// List of all the know correlationIDs interested in this topic
        /// </summary>
        private System.Collections.Hashtable m_CorrelationID;

        /// <summary>
        /// List of subjects (some excel header) that we may publish
        /// </summary>
        //private Dictionary<string, K2RTDSubject> m_Subjects;
        
        private Dictionary<string, K2Header> m_Headers;
        private Dictionary<string, Dictionary<int, object>> m_CorrelationChangedIDs;


        /// <summary>
        /// Construct a push subscriber 
        /// </summary>
        /// <param name="handler">reference to the request handler used to communicate with RTD</param>
        public K2RTDPushSubscriber(RTDServer handler)
        {
            m_RequestHandler = handler;
            m_CorrelationID = new System.Collections.Hashtable();
            //m_Subjects = new Dictionary<string, K2RTDSubject>();
            m_CorrelationChangedIDs = new Dictionary<string, Dictionary<int, object>>();
            m_Headers = new Dictionary<string, K2Header>();
        }

       
        /// <summary>
        /// Add some header that is of 
        /// </summary>
        /// <param name="accessID">access if of the comms channel</param>
        /// <param name="headerName">Name of the header</param>
        /// <param name="excelID">Excel id of the cell referencing the header</param>
        public void AddHeader(string accessID, string headerName, int excelID)
        {
            try
            {

                if (m_Headers.ContainsKey(headerName))
                {
                    m_Headers[headerName].AddExcelField(accessID, excelID);
                }
                else
                {
                    m_Headers.Add(headerName, new K2Header());
                    m_Headers[headerName].AddExcelField(accessID, excelID);
                    m_Headers[headerName].HeaderName = headerName;
                }
            }
            catch (Exception myE)
            {
            }
        }
        /// <summary>
        /// Add some correlationID to the list know for this topic
        /// </summary>
        /// <param name="accessID"></param>
        public void AddAccessID(string accessID)
        {
            if (!m_CorrelationID.ContainsKey(accessID))
            {
                m_CorrelationID.Add(accessID, accessID);
            }
        }


        /// <summary>
        /// Remove some correlationID from the list known for this topic
        /// </summary>
        /// <param name="accessID"></param>
        public void RemoveAccessID(string accessID)
        {
            m_CorrelationID.Remove(accessID);
        }

        

        /// <summary>
        /// get/set the subject ID - this is written on all updates, images and status(for example a stcok name DELL, IBM etc)
        /// </summary>
        public string SubjectID
        {
            get { return m_SubjectID; }
            set { m_SubjectID = value; }
        }

       

        /// <summary>
        /// Send out the updates for some accessID
        /// </summary>
        /// <param name="accessID"></param>
        /// <param name="changedIDValues"></param>
        private void updateChangedIds(string accessID, Dictionary<int, object> changedIDValues)
        {
            try
            {
                int idCount = changedIDValues.Count;
                int[] id = new int[idCount];

                string[] values = new string[idCount];
                int i = 0;
                foreach (int j in changedIDValues.Keys)
                {
                    id[i] = j;
                    values[i] = changedIDValues[j].ToString();
                    i++;
                }
                m_RequestHandler.TopicUpdate(accessID, id, values);
            }
            catch (Exception myE)
            {
            }
        }
     
        /// <summary>
        /// Sends out the update to Excel clients for all changed ID's
        /// </summary>
        private void resendChangedHeaders()
        {
            try
            {
                foreach (string corrID in m_CorrelationChangedIDs.Keys)
                {
                    updateChangedIds(corrID, m_CorrelationChangedIDs[corrID]);
                }
            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Updates the arrays of ExcelID's and Values that have changed
        /// </summary>
        /// <param name="mySender"></param>
        /// <param name="field"></param>
        /// <param name="header"></param>
        private void updateIDValue(IPublisher mySender, Field field, K2Header header)
        {
            try
            {
                foreach (K2ExcelField excelField in header.Fields)
                {
                    if (m_CorrelationChangedIDs.ContainsKey(excelField.AccessID))
                    {
                        if (m_CorrelationChangedIDs[excelField.AccessID].ContainsKey(excelField.ExcelFieldID))
                        {
                            m_CorrelationChangedIDs[excelField.AccessID][excelField.ExcelFieldID] = header.DataField.Value;
                        }
                        else
                        {
                            m_CorrelationChangedIDs[excelField.AccessID].Add(excelField.ExcelFieldID, header.DataField.Value);
                        }
                    }
                    else
                    {
                        m_CorrelationChangedIDs.Add(excelField.AccessID, new Dictionary<int, object>());
                        m_CorrelationChangedIDs[excelField.AccessID].Add(excelField.ExcelFieldID, header.DataField.Value);

                    }
                }
                
            }
            catch (Exception myE)
            {
                 
            }
        }
      
        /// <summary>
        /// if this something we are interested in then update the relvant header Updates the arrays of ExcelID's and Values that have changed
        /// excel ID 
        /// </summary>
        /// <param name="mySender"></param>
        /// <param name="field"></param>
        private void updateHeader(IPublisher mySender,Field field)
        {
            try
            {
                if (m_Headers.ContainsKey(field.ID))
                {
                   
                }
                else
                {
                    m_Headers.Add(field.ID, new K2Header());
                    
                }
                m_Headers[field.ID].DataField = field;
                m_Headers[field.ID].Changed = true;

                //Updates the arrays of ExcelID's and Values that have changed
                updateIDValue(mySender, field, m_Headers[field.ID]);
            }
            catch (Exception myE)
            {
            }
        }

        private void updateHeaders(IPublisher mySender, List<Field> itemList)
        {
            try 
            {
                foreach(Field field in itemList)
                {
                    updateHeader(mySender, field);
                }
                resendChangedHeaders();
            }
            catch(Exception myE)
            {
            }
        }

        #region Subscriber Members

        /// <summary>
        /// Handle an Image i.e. a full set of fields for the subject concerned
        /// </summary>
        /// <param name="mySender"></param>
        /// <param name="itemList"></param>
        public void OnImage(IPublisher mySender, List<Field> itemList)
        {
            updateHeaders(mySender,  itemList);
        }

        /// <summary>
        /// Handle a status chanage for the subject
        /// </summary>
        /// <param name="mySender"></param>
        /// <param name="itemList"></param>
        public void OnStatusChange(IPublisher mySender, List<Field> itemList)
        {
            updateHeaders(mySender, itemList);
        }

        /// <summary>
        /// Handle an update - i.e. some fields that have changed for the subject
        /// </summary>
        /// <param name="mySender"></param>
        /// <param name="itemList"></param>
        public void OnUpdate(IPublisher mySender, List<Field> itemList)
        {
            updateHeaders(mySender, itemList);
        }

        #endregion
    }
}

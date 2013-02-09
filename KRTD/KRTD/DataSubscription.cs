
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
using System.Linq;
using System.Text;

namespace KRTD
{
    class DataSubscription
    {
        private string m_AccessID;
        private int m_ID;
        private string m_TopicID;
        private string m_HeaderName;

        public DataSubscription(string myAccessID, int myID, string myTopicID, string myHeaderName)
        {
            m_AccessID = myAccessID;
            m_ID = myID;
            m_TopicID = myTopicID;
            m_HeaderName = myHeaderName;
        }

        public string AccessID
        { get { return m_AccessID; } }

        public int ID
        { get { return m_ID; } }

        public string TopicID 
        { get { return m_TopicID; } }

        public string  HeaderName
        { get { return m_HeaderName; } }
    }
}

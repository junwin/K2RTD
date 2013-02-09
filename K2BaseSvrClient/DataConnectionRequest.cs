
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
using System.Linq;
using System.Text;

namespace K2BaseSvrClient
{
    public class DataConnectionRequest
    {
        private string m_AccessID;
        private int m_TopicID;
        private string m_SubjectID;
        private string m_HeaderName;
        private string[] m_Strings;

        public DataConnectionRequest(string accessID, int topicID, string subjectID, string headerName, string[] Strings)
        {
            m_AccessID = accessID;
            m_TopicID = topicID;
            m_SubjectID = subjectID;
            m_HeaderName = headerName;
            m_Strings = Strings;
        }

        public string AccessID
        { get { return m_AccessID; } }

        public int TopicID
        { get { return m_TopicID; } }

        public string SubjectID
        { get { return m_SubjectID; } }

        public string HeaderName
        { get { return m_HeaderName; } }

        public string[] Strings
        { get { return m_Strings; } }
    }
}

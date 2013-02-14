//-----------------------------------------------------------------------
// <copyright file="DataConnectionrequest.cs" company="KaiTrade LLC">
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

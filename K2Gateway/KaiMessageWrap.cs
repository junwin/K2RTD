//-----------------------------------------------------------------------
// <copyright file="KaiMessageWrap.cs" company="KaiTrade LLC">
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace K2Gateway
{
    /// <summary>
    /// Simple class used to wrap string data to/from a socket
    /// </summary>
    [DataContract]
    public class KaiMessageWrap : IMessage
    {
        private string m_Data;
        private string m_CorrelationID;
        private string m_Label;

        /// <summary>
        /// Target ID (destination) for the message - not transmitted
        /// </summary>
        private string m_TargetID = "";

        private string m_Tag = "";
        private string m_Format = "";

        private long m_AppSpecific = 0;
        private int m_AppState = 0;
        private string m_AppType = "";

        private string m_ClientID ="";
        private string m_ClientSubID = "";
        private string m_TargetSubID = "";
        private string m_CreationTime ="";
        private string m_VenueCode = "";
        private string m_UserID ="";
        private string m_Identity = "";

        public KaiMessageWrap()
        {
            m_Identity = System.Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            string myRet = "";
            try
            {
                myRet += string.Format("Label {0} ClientID {1}  \r\n", this.Label, this.ClientID);
                myRet += string.Format("Data {0}  \r\n", this.Data);
            }
            catch
            {
            }
            return myRet;
        }


        [DataMember]
        public string Identity
        {
            get { return m_Identity; }
            set { m_Identity = value; }
        }


        [DataMember]
        public string Data
        {
            get { return m_Data; }
            set { m_Data = value; }
        }
        [DataMember]
        public string CorrelationID
        {
            get { return m_CorrelationID; }
            set { m_CorrelationID = value; }
        }
        [DataMember]
        public string Label
        {
            get { return m_Label; }
            set { m_Label = value; }
        }
        /// <summary>
        /// Get/Set format - not transmitted
        /// </summary>
        [DataMember]
        public string Format
        {
            get { return m_Format; }
            set { m_Format = value; }
        }
        /// <summary>
        /// Get/Set AppSpecific - not transmitted
        /// </summary>
        [DataMember]
        public long AppSpecific
        {
            get { return m_AppSpecific; }
            set { m_AppSpecific = value; }
        }
        /// <summary>
        /// Get/Set AppState - not transmitted
        /// </summary>
        [DataMember]
        public int AppState
        {
            get { return m_AppState; }
            set { m_AppState = value; }
        }

        /// <summary>
        /// Get/Set App type not transmitted
        /// </summary>
        [DataMember]
        public string AppType
        {
            get { return m_AppType; }
            set { m_AppType = value; }
        }

        /// <summary>
        /// Get/Set Tag - not transmitted
        /// </summary>
        [DataMember]
        public string Tag
        {
            get { return m_Tag; }
            set { m_Tag = value; }
        }
        /// <summary>
        /// get/Set cliewnt ID not transmitted
        /// </summary>
        [DataMember]
        public string ClientSubID
        {
            get { return m_ClientSubID; }
            set { m_ClientSubID = value; }
        }
        [DataMember]
        public string ClientID
        {
            get { return m_ClientID; }
            set { m_ClientID = value; }
        }
        [DataMember]
        public string TargetSubID
        {
            get { return m_TargetSubID; }
            set { m_TargetSubID = value; }
        }
        /// <summary>
        /// get set creation time not transmitted
        /// </summary>
        [DataMember]
        public string CreationTime
        {
            get { return m_CreationTime; }
            set { m_CreationTime = value; }
        }
        /// <summary>
        /// get/Set user ID not transmitted
        /// </summary>
        public string UserID
        {
            get { return m_UserID; }
            set { m_UserID = value; }
        }

        /// <summary>
        /// get set the Venue Code that the message is intended for
        /// </summary>
        public string VenueCode
        {
            get { return m_VenueCode; }
            set { m_VenueCode = value; }
        }
        /// <summary>
        /// get/set TargetID (destination) for the message - NOTE *not* transmitted
        /// </summary>
        [DataMember]
        public string TargetID
        {
            get
            {
                return m_TargetID;
            }
            set
            {
                m_TargetID = value;
            }
        }

        
    }
}

//-----------------------------------------------------------------------
// <copyright file="K2RHField.cs" company="KaiTrade LLC">
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

namespace K2RTDRequestHandler
{
   
     /// <summary>
    /// Represents a simple data field used in OnImage and OnUpdate
    /// </summary>
    public class K2RHField : Field
    {
        private string m_Value;
        private string m_ID;
        private bool m_Changed = false;
        private bool m_IsValid = false;

        public K2RHField()
        {
            m_Value = "";
            m_ID = "";
            m_Changed = false;
            m_IsValid = false;
            
        }

        public K2RHField(string myID, string myValue)
        {
            m_Value = myValue;
            m_ID = myID;
            m_Changed = false;
            m_IsValid = false;
        }

        public K2RHField(string myID)
        {
            m_Value = "";
            m_ID = myID;
            m_Changed = false;
            m_IsValid = false;
        }
        

        #region Field Members

        public string ID
        {
            get
            {
                return m_ID;
            }
            set
            {
                m_ID = value;
            }
        }

        public string Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
                m_Changed = true;
                m_IsValid = true;
            }
        }

        public bool Changed
        {
            get
            {
                return m_Changed;
            }
            set
            {
                m_Changed = value;
            }
        }
         
        public bool IsValid
        {
            get
            {
                return m_IsValid;
            }
            
        }

        #endregion
    }
}

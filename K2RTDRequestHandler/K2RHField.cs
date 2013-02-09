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

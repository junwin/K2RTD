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
    /// This class represents some named data item that we are interested, for example
    /// the ASK Price for some product.  It is able to keep a list of ExcelIDs(cells with an RTD call) that have
    /// some interest in the data item.  It is used is to help fan out a change to some data item to set of headers
    /// </summary>
    public class K2Header
    {
        /// <summary>
        /// Header Name
        /// </summary>
        private string m_Header;
        /// <summary>
        /// Fields sent to from the publisher
        /// </summary>
        private Field m_DataField;

        private string m_Value;

        private int m_Type;

        private bool m_Changed = false;

        /// <summary>
        /// Excel fields
        /// </summary>
        private List<K2ExcelField> m_Fields;

        public K2Header()
        {
            m_Fields = new List<K2ExcelField>();
        }

        /// <summary>
        /// Get/Set if the field has changed
        /// </summary>
        public bool Changed
        {
            get { return m_Changed; }
            set { m_Changed = value; }
        }

        /// <summary>
        /// Get/Set the underlying data field - that is the data kept for this header
        /// </summary>
        public Field DataField
        {
            get { return m_DataField; }
            set { m_DataField = value; }
        }

        /// <summary>
        /// Get/Set the name of the header
        /// </summary>
        public string HeaderName
        { get { return m_Header; } set { m_Header = value; } }

       
        /// <summary>
        /// Get/Set the list of ExcelFields that have some interest in this header
        /// </summary>
        public List<K2ExcelField> Fields
        { get { return m_Fields; } set { m_Fields = value; } }


        /// <summary>
        /// Add a new excel field to the headers interest list
        /// </summary>
        /// <param name="corrID"></param>
        /// <param name="excelFieldID"></param>
        public void AddExcelField(string accessID, int excelFieldID)
        {
            try
            {
                bool found = false;
                foreach (K2ExcelField field in m_Fields)
                {
                    if ((accessID == field.AccessID) && (excelFieldID == field.ExcelFieldID))
                    {
                        // its in there 
                        found = true;
                        break;
                    }
                    
                }
                if (!found)
                {
                    K2ExcelField excelField = new K2ExcelField(accessID, excelFieldID);
                    m_Fields.Add(excelField);
                }

            }
            catch (Exception myE)
            {
            }
        }

    }
}

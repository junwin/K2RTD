//-----------------------------------------------------------------------
// <copyright file="K2Header.cs" company="KaiTrade LLC">
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

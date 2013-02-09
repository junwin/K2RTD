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
    /// This class models some field(cell) that makes an RTD call in terms of the
    /// connection to the K2 RTD Hanlder and the Excel ID
    /// </summary>
    public class K2ExcelField
    {
        string m_AccessID;
        int m_ExcelFieldID;

        /// <summary>
        /// Create an Excell field
        /// </summary>
        /// <param name="accessID">access id  of the inbound session</param>
        /// <param name="excelFieldID">excel id of the cell that makes the RTD call</param>
        public K2ExcelField(string accessID, int excelFieldID)
        {
            m_AccessID = accessID;
            m_ExcelFieldID = excelFieldID;
        }

        /// <summary>
        /// Get/Set the accessID - this represents a communication to some workbook
        /// </summary>
        public string AccessID
        { get { return m_AccessID; } set { m_AccessID = value; } }

        /// <summary>
        /// Get/Set the ExcelFieldID - this is an identifier allocated by Excel for
        /// some cell with an RTD call
        /// </summary>
        public int ExcelFieldID
        { get { return m_ExcelFieldID; } set { m_ExcelFieldID = value; } }
    }
}

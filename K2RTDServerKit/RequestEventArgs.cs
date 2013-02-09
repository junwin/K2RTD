using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2RTDServerKit
{
    /// <summary>
    /// Event arguments for a request from Excel
    /// </summary>
    public class RequestEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessID">ID for the connection used to Excel</param>
        /// <param name="rtdTopicID">Excel rtd field ID</param>
        /// <param name="parameters">Array of string paramters</param>
        public RequestEventArgs(string accessID, int rtdTopicID, string[] parameters)
        {
            this.accessID = accessID;
            this.rtdTopicID = rtdTopicID;
            this.parameters = parameters;
        }

        /// <summary>
        /// ID for the connection used to Excel
        /// </summary>
        public string accessID;

        /// <summary>
        /// Excel rtd field ID
        /// </summary>
        public int rtdTopicID;

        /// <summary>
        /// Array of string paramters
        /// </summary>
        public string[] parameters;


    }
}

//-----------------------------------------------------------------------
// <copyright file="RequestEventArgs.cs" company="KaiTrade LLC">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2RTDServerKit
{
    /// <summary>
    /// Evant arguments used on a status updates
    /// </summary>
    public class StatusEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelStatus">new status of chennel (used for comms)</param>
        /// <param name="text">text description of the new state</param>
        public StatusEventArgs(string channelStatus, string text)
        {
            this.channelStatus = channelStatus;
            this.text = text;
        }

        /// <summary>
        /// new status of chennel (used for comms)
        /// </summary>
        public string channelStatus;

        /// <summary>
        /// text description of the new state
        /// </summary>
        public string text;


    }
    
}

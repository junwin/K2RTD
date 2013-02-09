using System;
using System.Collections.Generic;
using System.Text;

namespace K2BaseSvrClient
{
    /// <summary>
    /// This class represents some feed used to communicate with the RTD Support and the server
    /// </summary>
    public class Feed
    {
        /// <summary>
        /// LOcal name for the feed
        /// </summary>
        string m_FeedName = "";

        /// <summary>
        /// GW name the feed will use
        /// </summary>
        string m_FeedGWName = "";

        /// <summary>
        /// Feed status
        /// </summary>
        string m_FeedState = "CLOSED";

        public Feed(string myName, string myGWName)
        {
            m_FeedName = myName;
            m_FeedGWName = myGWName;
            m_FeedState = "CLOSED";
        }

        /// <summary>
        /// Get the local feed name
        /// </summary>
        public string Name
        {
            get
            {
                return m_FeedName;
            }
        }

        /// <summary>
        /// get the feed's GW Name - this maps a feed name to a specific TCP Gateway
        /// </summary>
        public string GWName
        {
            get
            {
                return m_FeedGWName;
            }
            set
            {
                m_FeedGWName = value;
            }
        }

        /// <summary>
        /// get the feed status
        /// </summary>
        public string Status
        {
            get
            {
                return m_FeedState;
            }
            set
            {
                m_FeedState = value;
            }
        }

        
    }
}

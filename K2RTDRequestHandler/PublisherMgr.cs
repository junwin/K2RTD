//-----------------------------------------------------------------------
// <copyright file="PublisherMgr.cs" company="KaiTrade LLC">
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
using System.Text;

namespace K2RTDRequestHandler
{
    /// <summary>
    /// Class for managing a set of publishers used by the system and
    /// external clients - this is the class that contains a set of publishers
    /// </summary>
    public class PublisherMgr : IPublisherManager
    {
        private static volatile PublisherMgr s_instance;

        private static object syncRoot = new object();

        private Dictionary<string, IPublisher> m_PublisherMap;

        private List<K2InstanceFactory> m_PublisherFactories;

        /// <summary>
        /// Logger
        /// </summary>
        public log4net.ILog m_Log = log4net.LogManager.GetLogger("Kaitrade");

        protected PublisherMgr()
		{
            m_PublisherMap = new Dictionary<string, IPublisher>();
            m_PublisherFactories = new List<K2InstanceFactory>();
		}

        public static PublisherMgr Instance()
		{
			// Singleton method
            if (s_instance == null)
            {
                lock (syncRoot)
                {
                    if (s_instance == null)
                    {
                        s_instance = new PublisherMgr();
                    }
                }
            }
			return s_instance;
		}

        /// <summary>
        /// Add some external instance factory that will be used to create algos of a given type - this is used to allow
        /// plugins and other external assemblies register their oen algos
        /// </summary>
        /// <param name="algoType"></param>
        /// <param name="factory"></param>
        public void AddInstanceFactory(PublisherType publisherType, K2InstanceFactory factory)
        {
            switch (publisherType)
            {
                case PublisherType.General:
                    m_PublisherFactories.Add(factory);
                    break;
                case PublisherType.Price:
                    m_PublisherFactories.Add(factory);
                    break;

                default:
                    m_PublisherFactories.Add(factory);
                    break;
            }
        }

        /// <summary>
        /// Get a list of all the available publisher types
        /// </summary>
        /// <returns></returns>
        public List<string> GetPublisherTypes()
        {
            List<string> typeNames = new List<string>();
            try
            {
                typeNames.Add("PXPublisher");
                typeNames.Add("Publisher");
                foreach (K2InstanceFactory factory in m_PublisherFactories)
                {
                    try
                    {
                        foreach (string pubName in factory.Names)
                        {
                            typeNames.Add(pubName);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch(Exception myE)
            {
                m_Log.Error("GetPublisherTypes", myE);
            }
            return typeNames;
        }

        /// <summary>
        /// Add/replace a publisher to the manager
        /// </summary>
        /// <param name="myPub"></param>
        public void Add(IPublisher myPub)
        {
            if(m_PublisherMap.ContainsKey(myPub.TopicID()))
            {
                m_PublisherMap[myPub.TopicID()] = myPub;
            }
            else
            {
                m_PublisherMap.Add(myPub.TopicID(), myPub);
            }
        }

        /// <summary>
        /// remove a publisher
        /// </summary>
        /// <param name="myPub"></param>
        public void Remove(IPublisher myPub)
        {
            m_PublisherMap.Remove(myPub.TopicID());
        }

        /// <summary>
        /// Create an instance of a particular publisher
        /// </summary>
        /// <param name="myType">type of publisher wanted</param>
        /// <returns></returns>
        private Publisher createPub(string myType)
        {
            Publisher newPub = null;
            try
            {
                switch (myType)
                {
                    case "PXPublisher":
                        //newPub = new KaiTrade.TradeObjects.PXPublisher();
                        break;
                    case "Publisher":
                        newPub = new Publisher();
                        break;

                    default:
                        newPub = null;
                        break;
                }
                if (newPub == null)
                {
                    foreach (K2InstanceFactory factory in m_PublisherFactories)
                    {
                        try
                        {
                            newPub = factory.CreateInstance(myType) as Publisher;
                        }
                        catch (Exception)
                        {
                        }
                        if (newPub != null)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("createPub", myE);
            }
            return newPub;
        }

        #region PublisherManager Members

        public IPublisher GetPublisher(string myType, string myTopicID, bool register)
        {
            Publisher myRet = null;

            try
            {
                IPublisher myPub = null;
                if (register)
                {
                    // Does the subject exist in the subject map
                    if (m_PublisherMap.ContainsKey(myTopicID))
                    {
                        myPub = m_PublisherMap[myTopicID];
                    }
                    else
                    {
                        myPub = createPub(myType);

                        myPub.TopicID(myTopicID);
                        m_PublisherMap.Add(myTopicID, myPub);
                    }
                }
                else
                {
                    myPub = createPub(myType);
                    myPub.TopicID(myTopicID);
                }

                return myPub;
            }
            catch (Exception myE)
            {
                m_Log.Error("GetPublisher", myE);
            }

            return myRet;
        }

        /// <summary>
        /// Get a publisher for the topic specified - return null if not found
        /// </summary>
        /// <param name="myTopicID"></param>
        /// <returns>null => not found</returns>
        public IPublisher GetPublisher(string myTopicID)
        {
            IPublisher myRet = null;
            if (m_PublisherMap.ContainsKey(myTopicID))
            {
                myRet = m_PublisherMap[myTopicID];
            }
            return myRet;
        }

        /// <summary>
        /// Get a list of all the available publisher topics
        /// </summary>
        /// <returns>list of topics</returns>
        public List<string> GetPublisherSubjects()
        {
            List<string> subjectList = new List<string>();
            try
            {
                foreach (string subject in m_PublisherMap.Keys)
                {
                    subjectList.Add(subject);
                }
            }
            catch
            {
            }
            return subjectList;
        }


        #endregion
    }
}

//-----------------------------------------------------------------------
// <copyright file="Publisher.cs" company="KaiTrade LLC">
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
    /// General purpsoe publisher for an arbitary set of fields 
    /// </summary>
    public class Publisher : IPublisher
    {
        private System.Collections.Hashtable m_Image;
        /// <summary>
        /// Fields that have been updated and are waiting to be sent using
        /// an Update
        /// </summary>
        private Dictionary<string, Field> m_UpdateFields;

        /// <summary>
        /// Current image of all the fields we have indexed with the field(header) name
        /// </summary>
        private Dictionary<string,  Field>  m_Fields;

        /// <summary>
        /// List of all our current subscribers
        /// </summary>
        private System.Collections.Generic.List<ISubscriber> m_Subscribers;

        /// <summary>
        /// List of status information fields
        /// </summary>
        private System.Collections.Generic.List<Field> m_StatusInfo;

        public log4net.ILog m_Log = log4net.LogManager.GetLogger("KaiTrade");
        private string m_Key;

        protected string m_Name;

        protected Status m_PubStatus;

        protected string m_PublisherType = "";

        /// <summary>
        /// User defined string tag
        /// </summary>
        protected string m_Tag;

        /// <summary>
        /// Is the subject changed
        /// </summary>
        //NOT USED? private bool m_isChanged = false;

        /// <summary>
        /// Status field used to publish our internal status
        /// </summary>
        private Field m_Status;

        public Publisher()
        {
            m_Log = log4net.LogManager.GetLogger("KaiTrade");

            m_Fields = new Dictionary<string, Field>();

            m_UpdateFields = new Dictionary<string, Field>();
            m_Subscribers = new System.Collections.Generic.List<ISubscriber>();

            m_StatusInfo = new System.Collections.Generic.List<Field>();
            m_Image = new System.Collections.Hashtable();

            //NOT USED? m_isChanged = false;
            m_PubStatus = Status.opening;
        }

        /// <summary>
        /// Send a list of all the fields to our observers
        /// </summary>
        protected void doImage()
        {
            try
            {
                // if we have some observers
                if (m_Subscribers.Count > 0)
                {
                    // chance to set any default fields in the image
                    resetDefaultFields();

                    setImageField("UPDTIME", System.Environment.TickCount.ToString());

                    List<Field> myFieldList;
                    getImageFields(out myFieldList);

                    // update all of our observers
                    foreach (ISubscriber mySubscriber in m_Subscribers)
                    {
                        try
                        {
                            mySubscriber.OnImage(this, myFieldList);
                        }
                        catch (Exception myE)
                        {
                            m_Log.Error("doImage - invoke", myE);
                        }
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("doImage", myE);
            }
        }

        protected virtual void resetDefaultFields()
        {
        }

        /// <summary>
        /// Add a field into the image
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myValue"></param>
        protected void setImageField(string myID, string myValue)
        {
            try
            {
                Field myField;
                if (m_Fields.ContainsKey(myID))
                {
                    myField = m_Fields[myID];
                    myField.Value = myValue;
                }
                else
                {
                    myField = new K2RHField(myID, myValue);
                    m_Fields.Add(myID, myField);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("setImageField", myE);
            }
        }

        /// <summary>
        /// Update a field, this adds/updates the feilds awaiting an update
        /// and updates the corresponding image field
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myValue"></param>
        protected virtual void updateField(string myID, string myValue)
        {
            try
            {
                // first set the field in the iamge
                setImageField(myID, myValue);

                // Now add/update the set of updated fields
                Field myField;
                if (m_UpdateFields.ContainsKey(myID))
                {
                    myField = m_UpdateFields[myID];
                    myField.Value = myValue;
                }
                else
                {
                    myField = new K2RHField(myID, myValue);
                    m_UpdateFields.Add(myID, myField);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("updateField", myE);
            }
        }

        /// <summary>
        /// Add our image fields to a new list - we do this to avoid subscribers messing
        /// with the main image list.
        /// </summary>
        /// <param name="myFieldList"></param>
        private void getImageFields(out List<Field> myFieldList)
        {
            myFieldList = new List<Field>();
            try
            {
                foreach (Field myField in m_Fields.Values)
                {
                    myFieldList.Add(myField);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("getImageFields", myE);
            }
        }

        /// <summary>
        /// Add our update fields to a new list - we do this to avoid subscribers messing
        /// with the main update list.
        /// </summary>
        /// <param name="myFieldList"></param>
        private void getUpdateFields(out List<Field> myFieldList)
        {
            myFieldList = new List<Field>();
            try
            {
                lock (m_Fields)
                    foreach (Field myField in m_UpdateFields.Values)
                    {
                        myFieldList.Add(myField);
                    }
            }
            catch (Exception myE)
            {
                m_Log.Error("getImageFields", myE);
            }
        }

        /// <summary>
        /// send the updated fields to our client observers
        /// </summary>
        protected void doUpdate()
        {
            try
            {
                // if we have some observers
                if (m_Subscribers.Count > 0)
                {
                    // add the current time as the update time to the list
                    this.updateField("UPDTIME", System.Environment.TickCount.ToString());
                    K2RHField myUpdTimeField;
                    myUpdTimeField = new K2RHField("UPDTIME", System.Environment.TickCount.ToString());

                    // create a list of changed fields with their current values
                    System.Collections.Generic.List<Field> myFieldList;
                    this.getUpdateFields(out myFieldList);
                    // clear out our list of updates
                    m_UpdateFields.Clear();

                    // send the update to all our observers
                    foreach (ISubscriber mySubscriber in m_Subscribers)
                    {
                        try
                        {
                            mySubscriber.OnUpdate(this, myFieldList);
                        }
                        catch (Exception myE)
                        {
                            m_Log.Error("doUpdate - invoke", myE);
                        }
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("doUpdate", myE);
            }
        }

        /// <summary>
        /// Apply any change in status as a status message and
        /// update to the internal status header
        /// </summary>
        /// <param name="myStatus"></param>
        private void updateStatus(Status myStatus)
        {
            try
            {
                // if the status has changes then do an
                // status update
                if (m_PubStatus != myStatus)
                {
                    m_PubStatus = myStatus;
                    m_Status = new K2RHField("STATUS", myStatus.ToString());
                    m_StatusInfo = new System.Collections.Generic.List<Field>();
                    m_StatusInfo.Add(m_Status);
                    applyStatus(m_StatusInfo);
                }

                // then update the synthetic status field

                doUpdate();
            }
            catch (Exception myE)
            {
                m_Log.Error("updateStatus", myE);
            }
        }

        /// <summary>
        /// Take a list of status fields and fire status calls on subscribers
        /// </summary>
        /// <param name="myStatusInfo"></param>
        private void applyStatus(System.Collections.Generic.List<Field> myStatusInfo)
        {
            try
            {
                m_StatusInfo = myStatusInfo;
                foreach (ISubscriber mySubscriber in m_Subscribers)
                {
                    try
                    {
                        mySubscriber.OnStatusChange(this, m_StatusInfo);
                    }
                    catch (Exception myE)
                    {
                        m_Log.Error("updateStatus - invoke", myE);
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("applyStatus", myE);
            }
        }

        #region Publisher Members

        public void Close()
        {
            try
            {
                m_Log.Info("Subject:" + m_Name + "is closing");
            }
            catch
            {
            }

            updateStatus(Status.closed);
            m_Subscribers.Clear();
            m_Image.Clear();
            m_UpdateFields.Clear();
            m_StatusInfo.Clear();
            m_Fields.Clear();

            m_Name = "";
        }

        /// <summary>
        /// Get/Set the fields list for the publisher - setting this will replace all
        /// existing fields in the publisher and issue an image
        /// </summary>
        public System.Collections.Generic.List<Field> FieldList
        {
            get
            {
                System.Collections.Generic.List<Field> myFieldList = new List<Field>();
                foreach (Field myField in m_Fields.Values)
                {
                    myFieldList.Add(myField);
                }
                return myFieldList;
            }
            set
            {
                (this as Publisher).OnImage(value);
            }
        }

        public void OnImage(List<Field> itemList)
        {
            // note we expect to be updated by the properties - not this way
            try
            {
                if (itemList != null)
                {
                    foreach(Field myField in itemList)
                    {
                        setImageField(myField.ID, myField.Value);
                    }
                }
                doImage();
            }
            catch (Exception myE)
            {
                m_Log.Error("Publisher.OnImage", myE);
            }
        }

        public void OnStatusChange(List<Field> itemList)
        {
            try
            {
                if (itemList != null)
                {
                    applyStatus(itemList);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Publisher.OnStatusChange", myE);
            }
        }

        /// <summary>
        /// get/set the publisher base status - will event all subscribers
        /// </summary>
        public Status Status
        {
            get { return m_PubStatus; }
            set { updateStatus(value); }
        }
        /// <summary>
        /// Get/Set the publisher type - this is user defined
        /// </summary>
        public string PublisherType
        { get { return m_PublisherType; } set { m_PublisherType = value; } }

        public void OnUpdate(List<Field> itemList)
        {
            // note we expect to be updated by the properties - not this way
            try
            {
                if (itemList != null)
                {
                    foreach (Field myField in itemList)
                    {
                        updateField(myField.ID, myField.Value);
                    }
                }
                doUpdate();
            }
            catch (Exception myE)
            {
                m_Log.Error("Publisher.OnUpdate", myE);
            }
        }

        /// <summary>
        /// update some arbitary field in the publisher - note not all publishers
        /// may support this
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myValue"></param>
        public void OnUpdate(string myID, string myValue)
        {
            try
            {
                this.updateField(myID, myValue);
                doUpdate();
            }
            catch (Exception myE)
            {
                m_Log.Error("OnUpdate:field:", myE);
            }
        }

        /// <summary>
        /// Update the publisher with some price update - not all publishers
        /// will action this.
        /// </summary>
        /// <param name="pxUpdate"></param>
        public void OnUpdate(string mnemonic, PXUpdate pxUpdate)
        {
            // No action
        }

        public string Open(string myData)
        {
            // we will let the GPS use the string passed in as a default name and Key
            m_Name = myData;

            try
            {
                m_Key = m_Name;

                m_Log.Info("Subject:" + m_Name + "is opening");
                //m_Status = Status.open;
                updateStatus(Status.open);
                doImage();
            }
            catch (Exception myE)
            {
                m_Log.Error("Open", myE);
            }

            return myData;
        }

        private void addSubscriber(ISubscriber mySubscriber)
        {
            try
            {
                foreach (ISubscriber mySub in m_Subscribers)
                {
                    if (mySub == mySubscriber)
                    {
                        // Do not add duplicate susbcribers
                        return;
                    }
                }
                m_Subscribers.Add(mySubscriber);
            }
            catch (Exception myE)
            {
                m_Log.Error("addSubscriber", myE);
            }
        }

        public void Subscribe(ISubscriber mySubscriber)
        {
            try
            {
                addSubscriber(mySubscriber);

                //myObserver.OnStatusChange(m_StatusInfo); //PTR

                // send an initial image
                System.Collections.Generic.List<Field> myFieldList;
                // chance to set any default fields in the image
                resetDefaultFields();
                getImageFields(out myFieldList);
                mySubscriber.OnImage(this, myFieldList);
            }
            catch (Exception myE)
            {
                m_Log.Error("Publisher.Subscribe", myE);
            }
        }

        public string Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }

        public string TopicID()
        {
            try
            {
                return m_Name;
            }
            catch (Exception myE)
            {
                m_Log.Error("TopicID:", myE);
            }
            return "";
        }

        public string TopicID(string myData)
        {
            try
            {
                return m_Name;
            }
            catch (Exception myE)
            {
                m_Log.Error("TopicID():", myE);
            }
            return "";
        }

        public void UnSubscribe(ISubscriber mySubscriber)
        {
            try
            {
                m_Subscribers.Remove(mySubscriber);
            }
            catch (Exception myE)
            {
                m_Log.Error("Publisher.UnSubscribe:", myE);
            }
        }

        #endregion
    }
}

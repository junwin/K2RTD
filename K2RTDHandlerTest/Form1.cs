using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace K2RTDHandlerTest
{
    public partial class Form1 : Form
    {
        private K2RTDServerKit.IRTDServer  m_Handler = null;

        public Form1()
        {
            InitializeComponent();
        }

        #region StartStopHandler


        private void btnStartHandler_Click(object sender, EventArgs e)
        {
            try
            {
                // you can optionally specify where dependencies can be loaded from
                // if this is blank the system will assume that all dependencies 
                // are in the same folder as K2RTDServerKit.dll
                // string binPath = @"C:\kairoot\K2Remote\build\bin";
                // the usual case is to leave this as an empty string
                string binPath = "";
                m_Handler = K2RTDServerKit.Factory.Instance().GetRTDServer(binPath); 

                // wire the callbacks for request from Excel and Status changes
                m_Handler.OnRequestHandler += new K2RTDServerKit.OnRequest(OnRequest);
                m_Handler.OnStatusUpdateHandler += new K2RTDServerKit.OnStatusUpdate(OnStatusUpdate);

                // start the Handler
                m_Handler.StartHandler("");

                btnStopHandler.Enabled = true;
                btnStartHandler.Enabled = false;
                 
            }
            catch 
            {
            }
        }

        private void btnStopHandler_Click(object sender, EventArgs e)
        {
            try
            {
                m_Handler.StopHandler();
                m_Handler = null;
                btnStopHandler.Enabled = false;
                btnStartHandler.Enabled = true;
            }
            catch
            {
            }
        }


        #endregion


        #region CallBackHandling

        /// <summary>
        /// Handle status updates from the connection to the RTD running in Excel
        /// </summary>
        /// <param name="channelStatus"></param>
        /// <param name="text"></param>
        public void OnStatusUpdate(object sender, K2RTDServerKit.StatusEventArgs e)
        {
            // Note you must use Invoke since the incomming call is on
            // a different thread from the GUI
            this.Invoke((MethodInvoker)(() => toolStripStatusLabel1.Text = e.text));
             
        }


        /// <summary>
        /// Handle a request from Excel
        /// </summary>
        /// <param name="accessID">ID of incoming rtd instance - used by the comm channel</param>
        /// <param name="rtdTopicID">Excel topic ID</param>
        /// <param name="parameters">Array of parameters from the RTD call</param>
        public void OnRequest(object sender, K2RTDServerKit.RequestEventArgs e) 
        {

            // By convention, at KaiTrade we use the first 4 parameters as follows
            //=RTD("kaitrade.k2rtd","","WPUB","IBM", "BIDPX", "123.45", ) - reqType = WPUB subject =IBM, Header="BIDPX", Value="123.45"
            //=RTD("kaitrade.k2rtd","","PX","HPQ","ASKPX") - reqType PX, subject=HPQ, Header="ASKPX"

            string notUsed = e.parameters[0];     // not used
            string topicID = e.parameters[1];     // Excel topicID
            string reqType = e.parameters[2];     // Request Type
            string subject = e.parameters[3];     // Subject
            string headerName = e.parameters[4];  // Header Name

            // the use of other parameters may vary depending on your
            // application, if specified in this case parameter 5 is the value
            // assigned to the header
            if (e.parameters.Count() > 5)
            {
                string headerValue = e.parameters[5];  // Header value
            }


            int rtdTopicId = int.Parse(topicID);
            switch (reqType)
            {
                case "WPUB":
                    // Web Publish 
                    // Swap the inbound parameter array into a string for debug
                    string request = m_Handler.GetAsString(e.parameters);

                    // Display the string in the list box - note that this needs to use
                    // Invoke to swap threads to the GUI thread
                    listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(request) ));

                    /// write a response into Excel
                    m_Handler.TopicUpdate(e.accessID, e.rtdTopicID, DateTime.Now.ToString());

                    break;
                case "PX":
                    // this is a request to subscribe
                    m_Handler.Subscribe(subject, headerName, rtdTopicId, e.accessID);

                    break;

                default:
                    break;
            }
        }

       

#endregion

        #region Publishing
        private void btnTestPub_Click(object sender, EventArgs e)
        {
            try
            {
                if ((cboSubject.Text.Length > 0) && (txtHeader.Text.Length > 0) && (txtValue.Text.Length > 0))
                {
                    m_Handler.Publish(cboSubject.Text, txtHeader.Text, txtValue.Text);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("please enter a subject, header and value");
                }
                
            }
            catch (Exception myE)
            {
            }
        }

        private void btnPubIntArray_Click(object sender, EventArgs e)
        {
            try
            {
                int[] testValues = new int[15];
                int baseValue = DateTime.Now.Second;
                for(int i = 0; i < 15; i++)
                {
                    testValues[i]=baseValue+i;
                }

                m_Handler.Publish(cboSubject.Text, testValues);

            }
            catch (Exception myE)
            {
            }
        }

        private void btnGetSubjects_Click(object sender, EventArgs e)
        {
            try
            {
                cboSubject.Items.Clear();
                foreach (string s in m_Handler.GetSubjects())
                {
                    cboSubject.Items.Add(s);
                }

            }
            catch (Exception myE)
            {
            }
        }

        #endregion
    }
}

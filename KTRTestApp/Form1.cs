using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KTRTestApp
{
    public partial class Form1 : Form
    {
         //public delegate void UpdateData(KaiTCPComm.KaiMessageWrap myMsg);
        public K2BaseSvrClient.OnUpdateNotify m_UpdateData;

        //public delegate void UpdateStatusData(KaiTCPComm.KaiMessageWrap myMsg);
        public K2BaseSvrClient.OnUpdateStatus m_UpdateStatusData;
        // Create a logger for use in this class
        //private log4net.ILog m_Log;
        private long m_UpdateCount = 0;
        private int m_TopicID = 0;
        
        // NEW STUFF

        //private K2BaseSvrClient.K2ZMQSvrClient m_Client;
        private K2BaseSvrClient.K2SVRRTDSvrClient m_Client;
        public Form1()
        {
            InitializeComponent();
            //log4net.Config.XmlConfigurator.Configure();

            //m_Log = log4net.LogManager.GetLogger("Kaitrade");
            //m_Log.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

           // m_Log.Info("frmKTApp Started");
            m_UpdateData = new K2BaseSvrClient.OnUpdateNotify(this.DoUpdateData);
            m_UpdateStatusData = new K2BaseSvrClient.OnUpdateStatus(this.DoUpdateStatusData);
        }

        private void btnInitSupport_Click(object sender, EventArgs e)
        {
            m_Client = K2BaseSvrClient.K2SVRRTDSvrClient.Instance();
            m_Client.UpdateNotify += new K2BaseSvrClient.OnUpdateNotify(OnUpdateNotify);
            m_Client.UpdateStatus += new K2BaseSvrClient.OnUpdateStatus(OnStatusNotify);
            m_Client.StartServer("RTDTEST");

        }

        /// <summary>
        /// Called any time there is a change in our data
        /// </summary>
        /// <param name="msg"></param>
        public void OnUpdateNotify()
        {
            try
            {
               // this.Invoke(m_UpdateData);
            }
            catch (Exception myE)
            {
                //KRTDSupport.KRTDSupport.Instance().Log.Error("OnChange", myE);
            }
        }
        
             /// <summary>
        /// Called any time there is a change in our data
        /// </summary>
        /// <param name="msg"></param>
        public void OnStatusNotify(string correlationId, K2BaseSvrClient.ConnectionState newState, string text)
        {
            try
            {
               //this.Invoke(m_UpdateStatusData,correlationId, newState,   text );
            }
            catch (Exception myE)
            {
                //KRTDSupport.KRTDSupport.Instance().Log.Error("OnChange", myE);
            }
        }


        /*
        public void OnStatusChange(KaiTCPComm.KaiMessageWrap msg)
        {
            this.Invoke(m_UpdateStatusData, msg);
        }

        public void OnChange(KaiTCPComm.KaiMessageWrap msg)
        {
            this.Invoke(m_UpdateData, msg);
        }
         * */

        /// <summary>
        /// Do updates on the GUI thread
        /// </summary>
        /// <param name="myMsg"></param>
        public void DoUpdateData()
        {
            m_UpdateCount++;
            //updateTree(); 
        }

        public void DoUpdateStatusData(string correlationId, K2BaseSvrClient.ConnectionState newState, string text)
        {
            string myInfo = correlationId + ":" + text;
            lstStatus.Items.Add(myInfo);
        }

        private void btnFinalizeSupport_Click(object sender, EventArgs e)
        {
            m_Client.Finalize();
        }

        private void btnSubscribe_Click(object sender, EventArgs e)
        {
            //IB:OSE.JPN:N225:FXXXXX:20081211
            subProduct(txtTopicID.Text, txtHeader.Text);
        }

        private void subProduct(string myProduct, string myHeader)
        {
            /*
            int myID = int.Parse(txtID.Text);
            KRTDSupport.KRTDSupport.Instance().SubscribeData(myProduct, myID, myHeader);
            myID++;
            txtID.Text = myID.ToString();
             */
            string[] data = new string[10];
            data[0] = "PX";
            data[1] = myProduct;
            data[2] = myHeader;
            m_Client.ConnectData(m_TopicID, data);
            m_TopicID++;

            
        }

        private void subDummyOrder(string myProduct)
        {
            /*
             * =RTD("kaitrade.k2rtd","","OR",A2,"StrategyName",H2,B2,C2,D2,E2,F2)
            int myID = int.Parse(txtID.Text);
            KRTDSupport.KRTDSupport.Instance().SubscribeData(myProduct, myID, myHeader);
            myID++;
            txtID.Text = myID.ToString();
             */
            string[] data = new string[10];
            data[0] = "OR";
            data[1] = "KTTESTHRN"; // strat
            data[2] = "StrategyName"; 

            data[3] = "1"; // Enabled
            data[4] = myProduct;
            data[5] = "1"; //qty
            data[6] = "1.5"; //px
            data[7] = "BUY"; //qty
            data[8] = "LIMIT"; //px


            m_Client.ConnectData(m_TopicID, data);
            m_TopicID++;


        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            long myRate = (m_UpdateCount * 1000) / timer1.Interval;
            txtRate.Text = m_UpdateCount.ToString();
            m_UpdateCount = 0;
        }

        private void updateTree()
        {
            try
            {
                Array newData;
                int newDataCount = 0;
                newData = m_Client.RefreshData(ref newDataCount);
                int topicID = 0;
                string dataValue = "";
                TreeNode myNode;
                tvTopicInfo.Nodes.Clear();
                myNode = new TreeNode();
                myNode.Text = "newdata";
                //myNode.Tag = myTopic;
                tvTopicInfo.Nodes.Add(myNode);

                for (int i = 0; i < newDataCount; i++)
                {
                    topicID = (int)newData.GetValue(0, i);
                    object zz = newData.GetValue(1, i);
                    dataValue = zz.ToString();
                    myNode.Nodes.Add(topicID.ToString() + " val =" + dataValue);

                }


            }
            catch (Exception myE)
            {
            }



        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            updateTree();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_Client.Finalize();

            // Force the app to shutdown
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void btnStore_Click(object sender, EventArgs e)
        {

        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            m_Client.Register();
        }

        private void subProduct(string product)
        {
            subProduct(product, "BID1_PRICE");
            subProduct(product, "BID1_SIZE");
            subProduct(product, "OFFER1_PRICE");
            subProduct(product, "OFFER1_SIZE");
        }
        private void btnSubscribeMany_Click(object sender, EventArgs e)
        {
            subProduct("KTSIM:ZZ:DELL:ESXXXX");
            subProduct("KTSIM:ZZ:APPL:ESXXXX");
            subProduct("KTSIM:ZZ:HPQ:ESXXXX");
            subProduct("KTSIM:ZZ:CY:ESXXXX");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_Client = K2BaseSvrClient.K2SVRRTDSvrClient.Instance();
            m_Client.UpdateNotify += new K2BaseSvrClient.OnUpdateNotify(OnUpdateNotify);
            m_Client.UpdateStatus += new K2BaseSvrClient.OnUpdateStatus(OnStatusNotify);
            m_Client.StartServer("RTDTEST");
            subProduct(txtTopicID.Text, txtHeader.Text);
            subProduct(txtTopicID.Text, txtHeader.Text);
            subProduct(txtTopicID.Text, txtHeader.Text);
            subProduct(txtTopicID.Text, txtHeader.Text);
            subProduct(txtTopicID.Text, txtHeader.Text);
            subProduct(txtTopicID.Text, txtHeader.Text);
        }

        private void btnSubOrder_Click(object sender, EventArgs e)
        {
            try
            {
                subDummyOrder("KTSIM:ZZ:DELL:ESXXXX");
            }
            catch (Exception myE)
            {
            }
        }
    }
}
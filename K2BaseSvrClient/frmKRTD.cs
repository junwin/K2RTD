/***************************************************************************
 *    
 *      Copyright (c) 2009,2010,2011 KaiTrade LLC (registered in Delaware)
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace K2BaseSvrClient
{
    public partial class frmKRTD : Form
    {
        //Need to swap update threads - for price updates
        public delegate void StatusChanged();
        private StatusChanged m_StatusChanged;

        private ContextMenu m_menu;
        private bool m_Stale = false;
        private RTDConnectionState m_State = RTDConnectionState.none;
        private List<string> m_StatusValues;
        private K2BaseSvrClient.K2SVRRTDSvrClient m_Client;
        private string m_LastRequest = "";
        private KRTDConfig m_Config=null;

        public frmKRTD()
        {
            InitializeComponent();
            m_StatusChanged = new StatusChanged(this.applyStatusChanged);

            m_menu = new ContextMenu();

            m_menu.MenuItems.Add(0, new MenuItem("Show", new System.EventHandler(Show_Click)));
            m_menu.MenuItems.Add(1, new MenuItem("Hide", new System.EventHandler(Hide_Click)));
            m_menu.MenuItems.Add(2, new MenuItem("ReSubscribe", new System.EventHandler(Resub_Click)));
            m_menu.MenuItems.Add(3, new MenuItem("ReStart", new System.EventHandler(ReStart_Click)));
            m_menu.MenuItems.Add(2, new MenuItem("Start", new System.EventHandler(Start_Click)));
            m_menu.MenuItems.Add(2, new MenuItem("Stop", new System.EventHandler(Stop_Click)));
            notifyIcon1.ContextMenu = m_menu;
            m_StatusValues = new List<string>();
            txtWrkDir.Text = Directory.GetCurrentDirectory();
            txtMachine.Text = System.Environment.MachineName;
        }

        public KRTDConfig Config
        {
            get { return m_Config; }
            set
            {
                m_Config = value;
                config2Dialog();
            }
        }

        public K2BaseSvrClient.K2SVRRTDSvrClient Client
        {
            get { return m_Client; }
            set { m_Client = value; }
        }

        public string LastRequest
        {
            get { return m_LastRequest; }
            set { m_LastRequest = value; }
        }

        public string IP
        {
            get { return txtIP.Text; }
            set { txtIP.Text = value; }
        }

        public string Port
        {
            get { return txtPort.Text; }
            set { txtPort.Text = value; }
        }

        public string ConfigPath
        {
            get { return txtConfigDir.Text; }
            set { txtConfigDir.Text = value; }
        }

        private void applyStatusChanged()
        {
            try
            {

                switch (m_State)
                {
                    case RTDConnectionState.connecting:
                        
                        toolStripStatusConnectedState.Text = "Connecting";
                        toolStripStatusConnectedState.BackColor = Color.Yellow;
                        break;
                    case RTDConnectionState.connected:
                        
                        toolStripStatusConnectedState.Text = "Connected";
                        toolStripStatusConnectedState.BackColor = Color.Green;
                        break;
                    case RTDConnectionState.disconnected:
                        
                        toolStripStatusConnectedState.Text = "Not Connected";
                        toolStripStatusConnectedState.BackColor = Color.Red;
                        break;
                    case RTDConnectionState.stale:
                        toolStripStatusConnectedState.Text = "Stale Data";
                        toolStripStatusConnectedState.BackColor = Color.Yellow;
                        break;

                    default:
                        break;
                }
            }
            catch (Exception myE)
            {
                ////m_Log.Error("applyStatus", myE);
            }

        }

        public string LastStatus()
        {
            if (m_StatusValues.Count > 0)
            {
                return m_StatusValues[m_StatusValues.Count - 1];
            }
            else
            {
                return "";
            }
        }
        public string ConnectionStatus
        {
            get
            {
                return LastStatus();
            }
            set
            {
                try
                {
                    m_StatusValues.Add(value);
                }
                catch (Exception myE)
                {
                }
            }
        }
        public void SetConnectionState(RTDConnectionState newState)
        {
            try
            {
                m_State = newState;
                //this.Invoke(m_StatusChanged, new object[] { });

            }
            catch (Exception myE)
            {
            }
        }

        public void SetStale(bool newStale)
        {
            try
            {
                m_Stale = newStale;
                this.Invoke(m_StatusChanged, new object[] { });
            }
            catch (Exception myE)
            {
            }
        }

        protected void ReStart_Click(Object sender, System.EventArgs e)
        {
            try
            {
                // resubscribe all
                //KRTDSupport.Instance().RestartRequest();
            }
            catch (Exception myE)
            {
            }
        }
        protected void Resub_Click(Object sender, System.EventArgs e)
        {
            try
            {
                // resubscribe all
                //KRTDSupport.Instance().ReSubscribe("*", 0);
            }
            catch (Exception myE)
            {
            }
        }
        protected void Start_Click(Object sender, System.EventArgs e)
        {
            try
            {
                m_Client.Start();
            }
            catch (Exception myE)
            {
            }
        }

        protected void Stop_Click(Object sender, System.EventArgs e)
        {
            try
            {
                m_Client.Stop();
            }
            catch (Exception myE)
            {
            }
        }

        protected void Hide_Click(Object sender, System.EventArgs e)
        {
            Hide();
        }
        protected void Show_Click(Object sender, System.EventArgs e)
        {
            Show();
        }

        private void frmKRTD_Load(object sender, EventArgs e)
        {

        }

        private void frmKRTD_DoubleClick(object sender, EventArgs e)
        {
            // refresh on a double click
            try
            {
                update();
            }
            catch (Exception myE)
            {

            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // refresh on a double click
            try
            {
                update();
            }
            catch (Exception myE)
            {

            }
        }

        private void update()
        {
            // refresh on a double click
            try
            {
                applyStatusChanged();
                listBox1.Items.Clear();
                foreach (string report in m_StatusValues)
                {
                    listBox1.Items.Add(report);
                }
                txtTripTime.Text = (m_Client.TripTime).ToString();
                txtLastReq.Text = this.LastRequest;
            }
            catch (Exception myE)
            {

            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dialog2Config()
        {
            try
            {
                if (m_Config != null)
                {
                    m_Config.IPEndpoint.Port = uint.Parse(txtPort.Text);
                    m_Config.IPEndpoint.Server = txtIP.Text;
                    m_Config.LogConfigPath = txtLogPath.Text;
                    //string temp = m_Config.ToXml();
                }
            }
            catch (Exception myE)
            {

            }
        }

        private void config2Dialog()
        {
            try
            {
                if (m_Config != null)
                {
                    txtPort.Text=m_Config.IPEndpoint.Port.ToString();
                    txtIP.Text = m_Config.IPEndpoint.Server;
                    txtLogPath.Text=m_Config.LogConfigPath;
                    
                }
            }
            catch (Exception myE)
            {

            }
        }

        public void SaveConfig(string path)
        {
            try
            {
                TextWriter writer = new StreamWriter(path, false);
                XmlSerializer serializer = new XmlSerializer(typeof(KRTDConfig));
                serializer.Serialize(writer, m_Config);
                writer.Close();


            }
            catch (Exception myE)
            {
            }
        }
        private void btnCreateCfg_Click(object sender, EventArgs e)
        {
            try
            {
                m_Config = new KRTDConfig();
                dialog2Config();
                string savePath = m_Client.ConfigPath + "newRtdconf.xml";
                SaveConfig(savePath);
                
            }
            catch (Exception myE)
            {
                 
            }
        }

        private void btnLoadCfg_Click(object sender, EventArgs e)
        {
            try
            {
                m_Client.LoadConfig(txtConfigDir.Text);
                this.config2Dialog();

                
            }
            catch (Exception myE)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtConfigDir.Text = openFileDialog1.FileName;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                m_Client.Stop();
                


            }
            catch (Exception myE)
            {

            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                m_Client.Start();



            }
            catch (Exception myE)
            {

            }
        }

        private void btnSaveCfg_Click(object sender, EventArgs e)
        {
            try
            {
                dialog2Config();
                string savePath = m_Client.ConfigPath + "savRtdconf.xml";
                m_Client.SaveConfig(savePath);



            }
            catch (Exception myE)
            {

            }
        }


    }
}

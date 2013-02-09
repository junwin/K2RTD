/***************************************************************************
 *    
 *      Copyright (c) 2009,2010 KaiTrade LLC (registered in Delaware)
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;

namespace KTRTestApp
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class Form2 : Form
    {
        K2BaseSvrClient.K2RTDClientHelper m_ClientHelper;
        //private ServiceReference2.K2RTDSupportClient m_Client = null;
        //KResposnseHandler m_Listner;
        public Form2()
        {
            InitializeComponent();
            m_ClientHelper = new K2BaseSvrClient.K2RTDClientHelper();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            ServiceReference2.K2RTDSupportClient
            ServiceReference1.KTAMessage myMsg = new ServiceReference1.KTAMessage();
            myMsg.m_Label = "zoot";
            myMsg.m_Data = txtData.Text;
            myMsg.m_UserID = txtUserID.Text;
            myMsg.m_VenueCode = "";
            m_Client.Send(myMsg);
             * */
             
        }

        
       

        

        private void btnSubscribe_Click(object sender, EventArgs e)
        {
            m_ClientHelper.Register();
            //m_Listner.Subscribe(txtUserID.Text);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtUserID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtData_TextChanged(object sender, EventArgs e)
        {

        }

        

        public void OnTopicUpdate(int[] id, string[] value)
        {
           // throw new NotImplementedException();
        }

        

        private void btnGetUpdates_Click(object sender, EventArgs e)
        {
            try
            {
                int topicCount=0;
                Array myData = m_ClientHelper.RefreshData(ref topicCount);
                lstResponse.Items.Clear();
                for (int i = 0; i < topicCount; i++)
                {
                    lstResponse.Items.Add(myData.GetValue(i, 0).ToString() + ":" + myData.GetValue(i, 1).ToString());
                }
            }
            catch (Exception myE)
            {
            }
        }
       
    }

   

        
    

    
}

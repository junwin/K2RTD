namespace KTRTestApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnInitAndSend = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.txtStorePath = new System.Windows.Forms.TextBox();
            this.btnStore = new System.Windows.Forms.Button();
            this.lstStatus = new System.Windows.Forms.ListBox();
            this.btnFinalizeSupport = new System.Windows.Forms.Button();
            this.btnInitSupport = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnSubscribeMany = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.tvTopicInfo = new System.Windows.Forms.TreeView();
            this.txtRate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtHeader = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.btnUnSub = new System.Windows.Forms.Button();
            this.txtTopicID = new System.Windows.Forms.TextBox();
            this.btnSubscribe = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnSubOrder = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(1, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(487, 272);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnInitAndSend);
            this.tabPage1.Controls.Add(this.btnRegister);
            this.tabPage1.Controls.Add(this.txtStorePath);
            this.tabPage1.Controls.Add(this.btnStore);
            this.tabPage1.Controls.Add(this.lstStatus);
            this.tabPage1.Controls.Add(this.btnFinalizeSupport);
            this.tabPage1.Controls.Add(this.btnInitSupport);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(479, 246);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Connect";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnInitAndSend
            // 
            this.btnInitAndSend.Location = new System.Drawing.Point(301, 27);
            this.btnInitAndSend.Name = "btnInitAndSend";
            this.btnInitAndSend.Size = new System.Drawing.Size(75, 23);
            this.btnInitAndSend.TabIndex = 6;
            this.btnInitAndSend.Text = "InitSend";
            this.btnInitAndSend.UseVisualStyleBackColor = true;
            this.btnInitAndSend.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnRegister
            // 
            this.btnRegister.Enabled = false;
            this.btnRegister.Location = new System.Drawing.Point(113, 27);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(75, 23);
            this.btnRegister.TabIndex = 5;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // txtStorePath
            // 
            this.txtStorePath.Location = new System.Drawing.Point(104, 56);
            this.txtStorePath.Name = "txtStorePath";
            this.txtStorePath.Size = new System.Drawing.Size(165, 20);
            this.txtStorePath.TabIndex = 4;
            // 
            // btnStore
            // 
            this.btnStore.Location = new System.Drawing.Point(23, 53);
            this.btnStore.Name = "btnStore";
            this.btnStore.Size = new System.Drawing.Size(75, 23);
            this.btnStore.TabIndex = 3;
            this.btnStore.Text = "StoreData";
            this.btnStore.UseVisualStyleBackColor = true;
            this.btnStore.Click += new System.EventHandler(this.btnStore_Click);
            // 
            // lstStatus
            // 
            this.lstStatus.FormattingEnabled = true;
            this.lstStatus.Location = new System.Drawing.Point(23, 102);
            this.lstStatus.Name = "lstStatus";
            this.lstStatus.Size = new System.Drawing.Size(246, 95);
            this.lstStatus.TabIndex = 2;
            // 
            // btnFinalizeSupport
            // 
            this.btnFinalizeSupport.Location = new System.Drawing.Point(194, 27);
            this.btnFinalizeSupport.Name = "btnFinalizeSupport";
            this.btnFinalizeSupport.Size = new System.Drawing.Size(75, 23);
            this.btnFinalizeSupport.TabIndex = 1;
            this.btnFinalizeSupport.Text = "Stop";
            this.btnFinalizeSupport.UseVisualStyleBackColor = true;
            this.btnFinalizeSupport.Click += new System.EventHandler(this.btnFinalizeSupport_Click);
            // 
            // btnInitSupport
            // 
            this.btnInitSupport.Location = new System.Drawing.Point(23, 24);
            this.btnInitSupport.Name = "btnInitSupport";
            this.btnInitSupport.Size = new System.Drawing.Size(75, 23);
            this.btnInitSupport.TabIndex = 0;
            this.btnInitSupport.Text = "Init";
            this.btnInitSupport.UseVisualStyleBackColor = true;
            this.btnInitSupport.Click += new System.EventHandler(this.btnInitSupport_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnSubscribeMany);
            this.tabPage2.Controls.Add(this.btnTest);
            this.tabPage2.Controls.Add(this.tvTopicInfo);
            this.tabPage2.Controls.Add(this.txtRate);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.txtHeader);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.txtID);
            this.tabPage2.Controls.Add(this.btnUnSub);
            this.tabPage2.Controls.Add(this.txtTopicID);
            this.tabPage2.Controls.Add(this.btnSubscribe);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(479, 246);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Subscribe";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // btnSubscribeMany
            // 
            this.btnSubscribeMany.Location = new System.Drawing.Point(23, 114);
            this.btnSubscribeMany.Name = "btnSubscribeMany";
            this.btnSubscribeMany.Size = new System.Drawing.Size(75, 23);
            this.btnSubscribeMany.TabIndex = 12;
            this.btnSubscribeMany.Text = "SubscribeMany";
            this.btnSubscribeMany.UseVisualStyleBackColor = true;
            this.btnSubscribeMany.Click += new System.EventHandler(this.btnSubscribeMany_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(186, 72);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 11;
            this.btnTest.Text = "test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // tvTopicInfo
            // 
            this.tvTopicInfo.Location = new System.Drawing.Point(276, 114);
            this.tvTopicInfo.Name = "tvTopicInfo";
            this.tvTopicInfo.Size = new System.Drawing.Size(199, 113);
            this.tvTopicInfo.TabIndex = 10;
            // 
            // txtRate
            // 
            this.txtRate.Location = new System.Drawing.Point(331, 70);
            this.txtRate.Name = "txtRate";
            this.txtRate.Size = new System.Drawing.Size(87, 20);
            this.txtRate.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(273, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Rate/Sec";
            // 
            // txtHeader
            // 
            this.txtHeader.Location = new System.Drawing.Point(331, 29);
            this.txtHeader.Name = "txtHeader";
            this.txtHeader.Size = new System.Drawing.Size(87, 20);
            this.txtHeader.TabIndex = 7;
            this.txtHeader.Text = "BID1_PRICE";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(414, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(273, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Header";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "TopicID";
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(438, 29);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(37, 20);
            this.txtID.TabIndex = 3;
            this.txtID.Text = "0";
            // 
            // btnUnSub
            // 
            this.btnUnSub.Location = new System.Drawing.Point(104, 73);
            this.btnUnSub.Name = "btnUnSub";
            this.btnUnSub.Size = new System.Drawing.Size(75, 23);
            this.btnUnSub.TabIndex = 2;
            this.btnUnSub.Text = "UnSubscribe";
            this.btnUnSub.UseVisualStyleBackColor = true;
            // 
            // txtTopicID
            // 
            this.txtTopicID.Location = new System.Drawing.Point(71, 29);
            this.txtTopicID.Name = "txtTopicID";
            this.txtTopicID.Size = new System.Drawing.Size(196, 20);
            this.txtTopicID.TabIndex = 1;
            this.txtTopicID.Text = "IB:OSE.JPN:N225:FXXXXX:20081211";
            // 
            // btnSubscribe
            // 
            this.btnSubscribe.Location = new System.Drawing.Point(23, 73);
            this.btnSubscribe.Name = "btnSubscribe";
            this.btnSubscribe.Size = new System.Drawing.Size(75, 23);
            this.btnSubscribe.TabIndex = 0;
            this.btnSubscribe.Text = "Subscribe";
            this.btnSubscribe.UseVisualStyleBackColor = true;
            this.btnSubscribe.Click += new System.EventHandler(this.btnSubscribe_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(479, 246);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Data Updates";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btnSubOrder);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(479, 246);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Order";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnSubOrder
            // 
            this.btnSubOrder.Location = new System.Drawing.Point(36, 43);
            this.btnSubOrder.Name = "btnSubOrder";
            this.btnSubOrder.Size = new System.Drawing.Size(93, 23);
            this.btnSubOrder.TabIndex = 0;
            this.btnSubOrder.Text = "DummyOrder";
            this.btnSubOrder.UseVisualStyleBackColor = true;
            this.btnSubOrder.Click += new System.EventHandler(this.btnSubOrder_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 273);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "KRTDTestApp";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnFinalizeSupport;
        private System.Windows.Forms.Button btnInitSupport;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox lstStatus;
        private System.Windows.Forms.TextBox txtStorePath;
        private System.Windows.Forms.Button btnStore;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.Button btnUnSub;
        private System.Windows.Forms.TextBox txtTopicID;
        private System.Windows.Forms.Button btnSubscribe;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtHeader;
        private System.Windows.Forms.TextBox txtRate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TreeView tvTopicInfo;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnSubscribeMany;
        private System.Windows.Forms.Button btnInitAndSend;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnSubOrder;
    }
}


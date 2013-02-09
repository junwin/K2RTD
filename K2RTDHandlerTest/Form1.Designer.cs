namespace K2RTDHandlerTest
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
            this.btnStartHandler = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnStopHandler = new System.Windows.Forms.Button();
            this.btnTestPub = new System.Windows.Forms.Button();
            this.btnPubDoubleArray = new System.Windows.Forms.Button();
            this.btnPubIntArray = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboSubject = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtHeader = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.btnGetSubjects = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartHandler
            // 
            this.btnStartHandler.Location = new System.Drawing.Point(38, 39);
            this.btnStartHandler.Name = "btnStartHandler";
            this.btnStartHandler.Size = new System.Drawing.Size(90, 23);
            this.btnStartHandler.TabIndex = 0;
            this.btnStartHandler.Text = "Start Handler";
            this.btnStartHandler.UseVisualStyleBackColor = true;
            this.btnStartHandler.Click += new System.EventHandler(this.btnStartHandler_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(20, 41);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(379, 134);
            this.listBox1.TabIndex = 3;
            // 
            // btnStopHandler
            // 
            this.btnStopHandler.Enabled = false;
            this.btnStopHandler.Location = new System.Drawing.Point(149, 39);
            this.btnStopHandler.Name = "btnStopHandler";
            this.btnStopHandler.Size = new System.Drawing.Size(91, 23);
            this.btnStopHandler.TabIndex = 4;
            this.btnStopHandler.Text = "Stop Handler";
            this.btnStopHandler.UseVisualStyleBackColor = true;
            this.btnStopHandler.Click += new System.EventHandler(this.btnStopHandler_Click);
            // 
            // btnTestPub
            // 
            this.btnTestPub.Location = new System.Drawing.Point(6, 39);
            this.btnTestPub.Name = "btnTestPub";
            this.btnTestPub.Size = new System.Drawing.Size(90, 23);
            this.btnTestPub.TabIndex = 5;
            this.btnTestPub.Text = "TestPub";
            this.btnTestPub.UseVisualStyleBackColor = true;
            this.btnTestPub.Click += new System.EventHandler(this.btnTestPub_Click);
            // 
            // btnPubDoubleArray
            // 
            this.btnPubDoubleArray.Location = new System.Drawing.Point(6, 19);
            this.btnPubDoubleArray.Name = "btnPubDoubleArray";
            this.btnPubDoubleArray.Size = new System.Drawing.Size(90, 23);
            this.btnPubDoubleArray.TabIndex = 6;
            this.btnPubDoubleArray.Text = "Publish Double";
            this.btnPubDoubleArray.UseVisualStyleBackColor = true;
            // 
            // btnPubIntArray
            // 
            this.btnPubIntArray.Location = new System.Drawing.Point(6, 48);
            this.btnPubIntArray.Name = "btnPubIntArray";
            this.btnPubIntArray.Size = new System.Drawing.Size(90, 23);
            this.btnPubIntArray.TabIndex = 7;
            this.btnPubIntArray.Text = "Publish int";
            this.btnPubIntArray.UseVisualStyleBackColor = true;
            this.btnPubIntArray.Click += new System.EventHandler(this.btnPubIntArray_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(562, 319);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnStartHandler);
            this.tabPage1.Controls.Add(this.btnStopHandler);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(554, 293);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Start/Stop";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(554, 293);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "InboundRequest";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Controls.Add(this.btnGetSubjects);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(554, 293);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "PublishData";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnPubDoubleArray);
            this.groupBox2.Controls.Add(this.btnPubIntArray);
            this.groupBox2.Location = new System.Drawing.Point(27, 167);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(478, 100);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Array Publish";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnTestPub);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cboSubject);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtHeader);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtValue);
            this.groupBox1.Location = new System.Drawing.Point(27, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(478, 100);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SingleItem";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(345, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Value";
            // 
            // cboSubject
            // 
            this.cboSubject.FormattingEnabled = true;
            this.cboSubject.Location = new System.Drawing.Point(115, 40);
            this.cboSubject.Name = "cboSubject";
            this.cboSubject.Size = new System.Drawing.Size(121, 21);
            this.cboSubject.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(239, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Header";
            // 
            // txtHeader
            // 
            this.txtHeader.Location = new System.Drawing.Point(242, 41);
            this.txtHeader.Name = "txtHeader";
            this.txtHeader.Size = new System.Drawing.Size(100, 20);
            this.txtHeader.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(112, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Subject";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(348, 41);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(100, 20);
            this.txtValue.TabIndex = 11;
            // 
            // btnGetSubjects
            // 
            this.btnGetSubjects.Location = new System.Drawing.Point(27, 18);
            this.btnGetSubjects.Name = "btnGetSubjects";
            this.btnGetSubjects.Size = new System.Drawing.Size(90, 23);
            this.btnGetSubjects.TabIndex = 8;
            this.btnGetSubjects.Text = "GetSubjects";
            this.btnGetSubjects.UseVisualStyleBackColor = true;
            this.btnGetSubjects.Click += new System.EventHandler(this.btnGetSubjects_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 297);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(562, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(43, 17);
            this.toolStripStatusLabel1.Text = "Closed";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 319);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "K2RTDHandlerSample";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartHandler;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnStopHandler;
        private System.Windows.Forms.Button btnTestPub;
        private System.Windows.Forms.Button btnPubDoubleArray;
        private System.Windows.Forms.Button btnPubIntArray;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ComboBox cboSubject;
        private System.Windows.Forms.Button btnGetSubjects;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtHeader;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}


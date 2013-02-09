namespace KTRTestApp
{
    partial class Form2
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
            this.button1 = new System.Windows.Forms.Button();
            this.lstResponse = new System.Windows.Forms.ListBox();
            this.txtData = new System.Windows.Forms.TextBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSubscribe = new System.Windows.Forms.Button();
            this.btnGetUpdates = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(259, 140);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lstResponse
            // 
            this.lstResponse.FormattingEnabled = true;
            this.lstResponse.Location = new System.Drawing.Point(52, 166);
            this.lstResponse.Name = "lstResponse";
            this.lstResponse.Size = new System.Drawing.Size(409, 95);
            this.lstResponse.TabIndex = 1;
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(340, 140);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(100, 20);
            this.txtData.TabIndex = 2;
            this.txtData.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(179, 16);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(100, 20);
            this.txtUserID.TabIndex = 3;
            this.txtUserID.TextChanged += new System.EventHandler(this.txtUserID_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(133, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "UserID";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // btnSubscribe
            // 
            this.btnSubscribe.Location = new System.Drawing.Point(52, 13);
            this.btnSubscribe.Name = "btnSubscribe";
            this.btnSubscribe.Size = new System.Drawing.Size(75, 23);
            this.btnSubscribe.TabIndex = 5;
            this.btnSubscribe.Text = "Subscribe";
            this.btnSubscribe.UseVisualStyleBackColor = true;
            this.btnSubscribe.Click += new System.EventHandler(this.btnSubscribe_Click);
            // 
            // btnGetUpdates
            // 
            this.btnGetUpdates.Location = new System.Drawing.Point(52, 51);
            this.btnGetUpdates.Name = "btnGetUpdates";
            this.btnGetUpdates.Size = new System.Drawing.Size(75, 23);
            this.btnGetUpdates.TabIndex = 6;
            this.btnGetUpdates.Text = "GetUpdates";
            this.btnGetUpdates.UseVisualStyleBackColor = true;
            this.btnGetUpdates.Click += new System.EventHandler(this.btnGetUpdates_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 273);
            this.Controls.Add(this.btnGetUpdates);
            this.Controls.Add(this.btnSubscribe);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.lstResponse);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox lstResponse;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSubscribe;
        private System.Windows.Forms.Button btnGetUpdates;
    }
}


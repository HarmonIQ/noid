namespace NoID.Biometrics
{
    partial class Form_Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false</param>
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
            this.txtReaderSelected = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.btnEnroll = new System.Windows.Forms.Button();
            this.btnIdentify = new System.Windows.Forms.Button();
            this.btnVerify = new System.Windows.Forms.Button();
            this.btnStreaming = new System.Windows.Forms.Button();
            this.btnCapture = new System.Windows.Forms.Button();
            this.btnReaderSelect = new System.Windows.Forms.Button();
            this.btnEnrollmentControl = new System.Windows.Forms.Button();
            this.btnIdentificationControl = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtReaderSelected
            // 
            this.txtReaderSelected.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.txtReaderSelected.Location = new System.Drawing.Point(15, 27);
            this.txtReaderSelected.Name = "txtReaderSelected";
            this.txtReaderSelected.Size = new System.Drawing.Size(233, 19);
            this.txtReaderSelected.TabIndex = 7;
            this.txtReaderSelected.ReadOnly = true;
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(12, 9);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(236, 15);
            this.Label1.Text = "Selected Reader:";
            // 
            // btnEnroll
            // 
            this.btnEnroll.Enabled = false;
            this.btnEnroll.Location = new System.Drawing.Point(12, 111);
            this.btnEnroll.Name = "btnEnroll";
            this.btnEnroll.Size = new System.Drawing.Size(115, 23);
            this.btnEnroll.TabIndex = 13;
            this.btnEnroll.Text = "Enrollment";
            this.btnEnroll.Click += new System.EventHandler(this.btnEnroll_Click);
            // 
            // btnIdentify
            // 
            this.btnIdentify.Enabled = false;
            this.btnIdentify.Location = new System.Drawing.Point(133, 82);
            this.btnIdentify.Name = "btnIdentify";
            this.btnIdentify.Size = new System.Drawing.Size(115, 23);
            this.btnIdentify.TabIndex = 12;
            this.btnIdentify.Text = "Identification";
            this.btnIdentify.Click += new System.EventHandler(this.btnIdentify_Click);
            // 
            // btnVerify
            // 
            this.btnVerify.Enabled = false;
            this.btnVerify.Location = new System.Drawing.Point(12, 82);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(115, 23);
            this.btnVerify.TabIndex = 11;
            this.btnVerify.Text = "Verification";
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // btnStreaming
            // 
            this.btnStreaming.Enabled = false;
            this.btnStreaming.Location = new System.Drawing.Point(133, 111);
            this.btnStreaming.Name = "btnStreaming";
            this.btnStreaming.Size = new System.Drawing.Size(115, 23);
            this.btnStreaming.TabIndex = 15;
            this.btnStreaming.Text = "Streaming";
            this.btnStreaming.Click += new System.EventHandler(this.btnStreaming_Click);
            // 
            // btnCapture
            // 
            this.btnCapture.Enabled = false;
            this.btnCapture.Location = new System.Drawing.Point(133, 53);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(115, 23);
            this.btnCapture.TabIndex = 9;
            this.btnCapture.Text = "Capture";
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // btnReaderSelect
            // 
            this.btnReaderSelect.Location = new System.Drawing.Point(12, 53);
            this.btnReaderSelect.Name = "btnReaderSelect";
            this.btnReaderSelect.Size = new System.Drawing.Size(115, 23);
            this.btnReaderSelect.TabIndex = 8;
            this.btnReaderSelect.Text = "Reader Selection";
            this.btnReaderSelect.Click += new System.EventHandler(this.btnReaderSelect_Click);
            // 
            // btnEnrollmentControl
            // 
            this.btnEnrollmentControl.Enabled = false;
            this.btnEnrollmentControl.Location = new System.Drawing.Point(12, 150);
            this.btnEnrollmentControl.Name = "btnEnrollmentControl";
            this.btnEnrollmentControl.Size = new System.Drawing.Size(115, 23);
            this.btnEnrollmentControl.TabIndex = 16;
            this.btnEnrollmentControl.Text = "Enrollment GUI";
            this.btnEnrollmentControl.Click += new System.EventHandler(this.btnEnrollmentControl_Click);
            // 
            // btnIdentificationControl
            // 
            this.btnIdentificationControl.Enabled = false;
            this.btnIdentificationControl.Location = new System.Drawing.Point(133, 150);
            this.btnIdentificationControl.Name = "btnIdentificationControl";
            this.btnIdentificationControl.Size = new System.Drawing.Size(115, 23);
            this.btnIdentificationControl.TabIndex = 17;
            this.btnIdentificationControl.Text = "Identification GUI";
            this.btnIdentificationControl.Click += new System.EventHandler(this.btnIdentificationControl_Click);
            // 
            // Form_Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(257, 177);
            this.Controls.Add(this.btnIdentificationControl);
            this.Controls.Add(this.btnEnrollmentControl);
            this.Controls.Add(this.txtReaderSelected);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.btnEnroll);
            this.Controls.Add(this.btnIdentify);
            this.Controls.Add(this.btnVerify);
            this.Controls.Add(this.btnStreaming);
            this.Controls.Add(this.btnCapture);
            this.Controls.Add(this.btnReaderSelect);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
#if !WindowsCE
            this.MaximumSize = new System.Drawing.Size(277, 227);
            this.MinimumSize = new System.Drawing.Size(277, 227);
            this.ClientSize = new System.Drawing.Size(277, 227);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
#endif
            this.Name = "Form_Main";
            this.Text = "U.are.U Sample C#";
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.TextBox txtReaderSelected;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Button btnEnroll;
        internal System.Windows.Forms.Button btnIdentify;
        internal System.Windows.Forms.Button btnVerify;
        internal System.Windows.Forms.Button btnStreaming;
        internal System.Windows.Forms.Button btnCapture;
        internal System.Windows.Forms.Button btnReaderSelect;
        internal System.Windows.Forms.Button btnEnrollmentControl;
        public System.Windows.Forms.Button btnIdentificationControl;
    }
}
using DPCtlUruNet;

//! @cond
namespace NoID.Biometrics
{
    partial class EnrollmentControl
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
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pbFingerprint = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.txtMessage.Location = new System.Drawing.Point(491, 3);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(276, 173);
            this.txtMessage.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(499, 207);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 20);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(499, 233);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 20);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pbFingerprint
            // 
            this.pbFingerprint.Location = new System.Drawing.Point(583, 182);
            this.pbFingerprint.Name = "pbFingerprint";
            this.pbFingerprint.Size = new System.Drawing.Size(184, 167);
            this.pbFingerprint.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbFingerprint.TabIndex = 0;
            this.pbFingerprint.TabStop = false;
            // 
            // EnrollmentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(770, 354);
            this.Controls.Add(this.pbFingerprint);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtMessage);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
#if !WindowsCE
            this.MaximumSize = new System.Drawing.Size(790, 404);
            this.MinimumSize = new System.Drawing.Size(790, 404);
            this.ClientSize = new System.Drawing.Size(790, 404);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
#endif
            this.Name = "EnrollmentControl";
            this.Text = "Enrollment";
            this.Closed += new System.EventHandler(this.frmEnrollment_Closed);
            this.Load += new System.EventHandler(this.EnrollmentControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.PictureBox pbFingerprint;
    }
}
//! @endcond
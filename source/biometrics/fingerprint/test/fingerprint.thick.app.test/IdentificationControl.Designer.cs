using DPCtlUruNet;

//! @cond
namespace NoID.Biometrics
{
    partial class IdentificationControl
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
            this.btnClose = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(328, 111);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 20);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.txtMessage.Location = new System.Drawing.Point(406, 3);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(208, 128);
            this.txtMessage.TabIndex = 2;
            // 
            // IdentificationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(617, 135);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtMessage);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
#if !WindowsCE
        this.MaximumSize = new System.Drawing.Size(637, 185);
        this.MinimumSize = new System.Drawing.Size(637, 185);
        this.ClientSize = new System.Drawing.Size(637, 185);
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
#endif
            this.Name = "IdentificationControl";
            this.Text = "Identification";
            this.Closed += new System.EventHandler(this.IdentificationControl_Closed);
            this.Load += new System.EventHandler(this.IdentificationControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtMessage;
    }
}
//! @endcond
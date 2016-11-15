namespace NoID.Biometrics
{
    partial class Enrollment
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
            this.btnBack = new System.Windows.Forms.Button();
            this.txtEnroll = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(276, 229);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 6;
            this.btnBack.Text = "Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // txtEnroll
            // 
            this.txtEnroll.Location = new System.Drawing.Point(12, 10);
            this.txtEnroll.Multiline = true;
            this.txtEnroll.Name = "txtEnroll";
            this.txtEnroll.Size = new System.Drawing.Size(339, 213);
            this.txtEnroll.TabIndex = 5;
            // 
            // Enrollment
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(354, 262);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.txtEnroll);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
#if !WindowsCE
            this.MaximumSize = new System.Drawing.Size(374, 312);
            this.MinimumSize = new System.Drawing.Size(374, 312);
            this.ClientSize = new System.Drawing.Size(374, 312);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
#endif
            this.Name = "Enrollment";
            this.Text = "Enrollment";
            this.Load += new System.EventHandler(this.Enrollment_Load);
            this.Closed += new System.EventHandler(this.Enrollment_Closed);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnBack;
        internal System.Windows.Forms.TextBox txtEnroll;
    }
}
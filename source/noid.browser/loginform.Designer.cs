namespace NoID.Browser
{
    partial class LoginForm
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
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.labelUserName = new System.Windows.Forms.Label();
            this.maskedTextBoxPassword = new System.Windows.Forms.MaskedTextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.buttonTestAuthentication = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(104, 83);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(150, 38);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save To Keystore";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(29, 45);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(148, 20);
            this.textBoxUserName.TabIndex = 1;
            this.textBoxUserName.Text = "FHIRWebServer";
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Location = new System.Drawing.Point(26, 28);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(60, 13);
            this.labelUserName.TabIndex = 2;
            this.labelUserName.Text = "User Name";
            // 
            // maskedTextBoxPassword
            // 
            this.maskedTextBoxPassword.Location = new System.Drawing.Point(183, 45);
            this.maskedTextBoxPassword.Name = "maskedTextBoxPassword";
            this.maskedTextBoxPassword.PasswordChar = '*';
            this.maskedTextBoxPassword.Size = new System.Drawing.Size(147, 20);
            this.maskedTextBoxPassword.TabIndex = 3;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(180, 28);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(53, 13);
            this.labelPassword.TabIndex = 4;
            this.labelPassword.Text = "Password";
            // 
            // buttonTestAuthentication
            // 
            this.buttonTestAuthentication.Location = new System.Drawing.Point(104, 147);
            this.buttonTestAuthentication.Name = "buttonTestAuthentication";
            this.buttonTestAuthentication.Size = new System.Drawing.Size(150, 38);
            this.buttonTestAuthentication.TabIndex = 5;
            this.buttonTestAuthentication.Text = "Test Authentication";
            this.buttonTestAuthentication.UseVisualStyleBackColor = true;
            this.buttonTestAuthentication.Click += new System.EventHandler(this.buttonTestAuthentication_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(3, 204);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(65, 13);
            this.labelStatus.TabIndex = 6;
            this.labelStatus.Text = "Status: New";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 226);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonTestAuthentication);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.maskedTextBoxPassword);
            this.Controls.Add(this.labelUserName);
            this.Controls.Add(this.textBoxUserName);
            this.Controls.Add(this.buttonSave);
            this.Name = "LoginForm";
            this.Text = "NoID FHIR Server Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxPassword;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Button buttonTestAuthentication;
        private System.Windows.Forms.Label labelStatus;
    }
}
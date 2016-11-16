namespace NoID.Biometrics
{
    partial class Capabilities
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
            this.btnSelect = new System.Windows.Forms.Button();
            this.lstCaps = new System.Windows.Forms.ListBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtReaderSelected = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(157, 256);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(115, 23);
            this.btnSelect.TabIndex = 14;
            this.btnSelect.Text = "Close";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // lstCaps
            // 
            this.lstCaps.Location = new System.Drawing.Point(12, 88);
            this.lstCaps.Name = "lstCaps";
            this.lstCaps.Size = new System.Drawing.Size(260, 162);
            this.lstCaps.TabIndex = 13;
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.txtName.Location = new System.Drawing.Point(13, 20);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(260, 19);
            this.txtName.TabIndex = 16;
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(11, 2);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(236, 15);
            this.Label1.Text = "Name:";
            // 
            // txtReaderSelected
            // 
            this.txtReaderSelected.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.txtReaderSelected.Location = new System.Drawing.Point(12, 62);
            this.txtReaderSelected.Name = "txtReaderSelected";
            this.txtReaderSelected.Size = new System.Drawing.Size(260, 19);
            this.txtReaderSelected.TabIndex = 19;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(236, 15);
            this.label2.Text = "Selected Reader:";
            // 
            // Capabilities
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(284, 283);
            this.Controls.Add(this.txtReaderSelected);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.lstCaps);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
#if !WindowsCE
                        this.MaximumSize = new System.Drawing.Size(304, 333);
                        this.MinimumSize = new System.Drawing.Size(304, 333);
                    this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
#endif
            this.Name = "Capabilities";
            this.Text = "Capabilities";
            this.Load += new System.EventHandler(this.Capabilities_Load);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnSelect;
        internal System.Windows.Forms.ListBox lstCaps;
        internal System.Windows.Forms.TextBox txtName;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox txtReaderSelected;
        internal System.Windows.Forms.Label label2;
    }
}


//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
//            this.ClientSize = new System.Drawing.Size(284, 283);
//            this.Controls.Add(this.btnSelect);
//            this.Controls.Add(this.lstCaps);
//            this.MaximizeBox = false;
//            this.MinimizeBox = false;
//#if !WindowsCE
//            this.MaximumSize = new System.Drawing.Size(304, 333);
//            this.MinimumSize = new System.Drawing.Size(304, 333);
//        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
//#endif
//            this.Name = "Capabilities";
//            this.Text = "Capabilities";
//            this.Load += new System.EventHandler(this.Capabilities_Load);
//            this.ResumeLayout(false);
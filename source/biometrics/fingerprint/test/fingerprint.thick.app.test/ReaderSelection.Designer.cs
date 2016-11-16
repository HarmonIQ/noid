namespace NoID.Biometrics
{
    partial class ReaderSelection
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
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCaps = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cboReaders = new System.Windows.Forms.ComboBox();
            this.lblSelectReader = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(133, 81);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(115, 23);
            this.btnBack.TabIndex = 18;
            this.btnBack.Text = "Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Enabled = false;
            this.btnSelect.Location = new System.Drawing.Point(12, 81);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(115, 23);
            this.btnSelect.TabIndex = 17;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnReaderSelect_Click);
            // 
            // btnCaps
            // 
            this.btnCaps.Enabled = false;
            this.btnCaps.Location = new System.Drawing.Point(133, 52);
            this.btnCaps.Name = "btnCaps";
            this.btnCaps.Size = new System.Drawing.Size(115, 23);
            this.btnCaps.TabIndex = 16;
            this.btnCaps.Text = "Capabilities";
            this.btnCaps.Click += new System.EventHandler(this.btnCaps_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 52);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(115, 23);
            this.btnRefresh.TabIndex = 15;
            this.btnRefresh.Text = "Refresh List";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cboReaders
            // 
            this.cboReaders.Font = new System.Drawing.Font("Tahoma", 8.0F, System.Drawing.FontStyle.Regular);
            this.cboReaders.Location = new System.Drawing.Point(12, 25);
            this.cboReaders.Name = "cboReaders";
            this.cboReaders.Size = new System.Drawing.Size(256, 20);
            this.cboReaders.TabIndex = 14;
            // 
            // lblSelectReader
            // 
            this.lblSelectReader.Location = new System.Drawing.Point(12, 9);
            this.lblSelectReader.Name = "lblSelectReader";
            this.lblSelectReader.Size = new System.Drawing.Size(296, 13);
            this.lblSelectReader.TabIndex = 13;
            this.lblSelectReader.Text = "Select Reader:";
            // 
            // ReaderSelection
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(292, 109);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnCaps);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.cboReaders);
            this.Controls.Add(this.lblSelectReader);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
#if !WindowsCE
        this.MaximumSize = new System.Drawing.Size(312, 159);
        this.MinimumSize = new System.Drawing.Size(312, 159);
        this.ClientSize = new System.Drawing.Size(312, 159);
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
#endif
            this.Name = "Select Reader";
            this.Text = "Select Reader";
            this.Load += new System.EventHandler(this.ReaderSelection_Load);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnBack;
        internal System.Windows.Forms.Button btnSelect;
        internal System.Windows.Forms.Button btnCaps;
        internal System.Windows.Forms.Button btnRefresh;
        internal System.Windows.Forms.ComboBox cboReaders;
        internal System.Windows.Forms.Label lblSelectReader;
    }
}
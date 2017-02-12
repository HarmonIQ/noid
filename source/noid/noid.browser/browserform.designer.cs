namespace NoID.Browser
{
    partial class BrowserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserForm));
#if NAVIGATE
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.statusLabel = new System.Windows.Forms.Label();
            this.outputLabel = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.statusLabel);
            this.toolStripContainer.ContentPanel.Controls.Add(this.outputLabel);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(730, 465);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.LeftToolStripPanelVisible = false;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.RightToolStripPanelVisible = false;
            this.toolStripContainer.Size = new System.Drawing.Size(730, 490);
            this.toolStripContainer.TabIndex = 0;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip1);

            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusLabel.Location = new System.Drawing.Point(0, 439);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 13);
            this.statusLabel.TabIndex = 1;
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.outputLabel.Location = new System.Drawing.Point(0, 452);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(0, 13);
            this.outputLabel.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.Size = new System.Drawing.Size(730, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
#endif
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 490);        
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BrowserForm";
            this.Text = "NoID Main Window";
#if NAVIGATE
            this.Controls.Add(this.toolStripContainer);
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.ContentPanel.PerformLayout();
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.ResumeLayout(false);
#endif
        }

#endregion

        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.Label statusLabel;
#if NAVIGATE
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton backButton;
        private System.Windows.Forms.ToolStripButton forwardButton;
        private System.Windows.Forms.ToolStripTextBox urlTextBox;
        private System.Windows.Forms.ToolStripButton goButton;
#endif
    }
}
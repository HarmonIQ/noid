namespace NoID.Match.Database.Tests
{
    partial class TestMatchEngineForm
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
            this.imageCurrentFinger = new System.Windows.Forms.PictureBox();
            this.imageMatchedFinger = new System.Windows.Forms.PictureBox();
            this.labelCurrentFinger = new System.Windows.Forms.Label();
            this.labelBestFinger = new System.Windows.Forms.Label();
            this.labelBestScore = new System.Windows.Forms.Label();
            this.labelScannerStatus = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.labelBestFinger2 = new System.Windows.Forms.Label();
            this.imageBestFinger = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageCurrentFinger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageMatchedFinger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBestFinger)).BeginInit();
            this.SuspendLayout();
            // 
            // imageCurrentFinger
            // 
            this.imageCurrentFinger.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageCurrentFinger.Location = new System.Drawing.Point(46, 77);
            this.imageCurrentFinger.Name = "imageCurrentFinger";
            this.imageCurrentFinger.Size = new System.Drawing.Size(242, 264);
            this.imageCurrentFinger.TabIndex = 0;
            this.imageCurrentFinger.TabStop = false;
            // 
            // imageMatchedFinger
            // 
            this.imageMatchedFinger.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageMatchedFinger.Location = new System.Drawing.Point(457, 77);
            this.imageMatchedFinger.Name = "imageMatchedFinger";
            this.imageMatchedFinger.Size = new System.Drawing.Size(242, 264);
            this.imageMatchedFinger.TabIndex = 1;
            this.imageMatchedFinger.TabStop = false;
            // 
            // labelCurrentFinger
            // 
            this.labelCurrentFinger.AutoSize = true;
            this.labelCurrentFinger.Location = new System.Drawing.Point(43, 61);
            this.labelCurrentFinger.Name = "labelCurrentFinger";
            this.labelCurrentFinger.Size = new System.Drawing.Size(73, 13);
            this.labelCurrentFinger.TabIndex = 2;
            this.labelCurrentFinger.Text = "Current Finger";
            // 
            // labelBestFinger
            // 
            this.labelBestFinger.AutoSize = true;
            this.labelBestFinger.Location = new System.Drawing.Point(454, 61);
            this.labelBestFinger.Name = "labelBestFinger";
            this.labelBestFinger.Size = new System.Drawing.Size(81, 13);
            this.labelBestFinger.TabIndex = 3;
            this.labelBestFinger.Text = "Matched Finger";
            // 
            // labelBestScore
            // 
            this.labelBestScore.AutoSize = true;
            this.labelBestScore.Location = new System.Drawing.Point(454, 9);
            this.labelBestScore.Name = "labelBestScore";
            this.labelBestScore.Size = new System.Drawing.Size(59, 13);
            this.labelBestScore.TabIndex = 4;
            this.labelBestScore.Text = "Best Score";
            // 
            // labelScannerStatus
            // 
            this.labelScannerStatus.AutoSize = true;
            this.labelScannerStatus.Location = new System.Drawing.Point(43, 9);
            this.labelScannerStatus.Name = "labelScannerStatus";
            this.labelScannerStatus.Size = new System.Drawing.Size(112, 13);
            this.labelScannerStatus.TabIndex = 5;
            this.labelScannerStatus.Text = "Scanner Status: None";
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(457, 355);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 6;
            this.buttonReset.Text = "Reset Best";
            this.buttonReset.UseVisualStyleBackColor = true;
            // 
            // labelBestFinger2
            // 
            this.labelBestFinger2.AutoSize = true;
            this.labelBestFinger2.Location = new System.Drawing.Point(724, 61);
            this.labelBestFinger2.Name = "labelBestFinger2";
            this.labelBestFinger2.Size = new System.Drawing.Size(60, 13);
            this.labelBestFinger2.TabIndex = 8;
            this.labelBestFinger2.Text = "Best Finger";
            // 
            // imageBestFinger
            // 
            this.imageBestFinger.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBestFinger.Location = new System.Drawing.Point(727, 77);
            this.imageBestFinger.Name = "imageBestFinger";
            this.imageBestFinger.Size = new System.Drawing.Size(242, 264);
            this.imageBestFinger.TabIndex = 7;
            this.imageBestFinger.TabStop = false;
            // 
            // TestMatchEngineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 449);
            this.Controls.Add(this.labelBestFinger2);
            this.Controls.Add(this.imageBestFinger);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.labelScannerStatus);
            this.Controls.Add(this.labelBestScore);
            this.Controls.Add(this.labelBestFinger);
            this.Controls.Add(this.labelCurrentFinger);
            this.Controls.Add(this.imageMatchedFinger);
            this.Controls.Add(this.imageCurrentFinger);
            this.Name = "TestMatchEngineForm";
            this.Text = "Match Engine Test Application";
            ((System.ComponentModel.ISupportInitialize)(this.imageCurrentFinger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageMatchedFinger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBestFinger)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imageCurrentFinger;
        private System.Windows.Forms.PictureBox imageMatchedFinger;
        private System.Windows.Forms.Label labelCurrentFinger;
        private System.Windows.Forms.Label labelBestFinger;
        private System.Windows.Forms.Label labelBestScore;
        private System.Windows.Forms.Label labelScannerStatus;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Label labelBestFinger2;
        private System.Windows.Forms.PictureBox imageBestFinger;
    }
}


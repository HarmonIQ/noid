namespace noid.test.webclient
{
    partial class windowTestFHIR
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
            this.btnFind = new System.Windows.Forms.Button();
            this.btnAddBob = new System.Windows.Forms.Button();
            this.textFirstName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textMRN = new System.Windows.Forms.TextBox();
            this.textLastName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelRecordsFound = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(253, 189);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(128, 43);
            this.btnFind.TabIndex = 0;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnAddBob
            // 
            this.btnAddBob.Location = new System.Drawing.Point(57, 189);
            this.btnAddBob.Name = "btnAddBob";
            this.btnAddBob.Size = new System.Drawing.Size(128, 43);
            this.btnAddBob.TabIndex = 1;
            this.btnAddBob.Text = "Add";
            this.btnAddBob.UseVisualStyleBackColor = true;
            this.btnAddBob.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // textFirstName
            // 
            this.textFirstName.Location = new System.Drawing.Point(19, 95);
            this.textFirstName.Name = "textFirstName";
            this.textFirstName.Size = new System.Drawing.Size(128, 20);
            this.textFirstName.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textMRN);
            this.groupBox1.Controls.Add(this.textLastName);
            this.groupBox1.Controls.Add(this.textFirstName);
            this.groupBox1.Location = new System.Drawing.Point(57, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 157);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Patient Information:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "MRN";
            // 
            // textMRN
            // 
            this.textMRN.Location = new System.Drawing.Point(19, 51);
            this.textMRN.Name = "textMRN";
            this.textMRN.Size = new System.Drawing.Size(128, 20);
            this.textMRN.TabIndex = 4;
            // 
            // textLastName
            // 
            this.textLastName.Location = new System.Drawing.Point(162, 95);
            this.textLastName.Name = "textLastName";
            this.textLastName.Size = new System.Drawing.Size(128, 20);
            this.textLastName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "First Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(159, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Last Name";
            // 
            // labelRecordsFound
            // 
            this.labelRecordsFound.AutoSize = true;
            this.labelRecordsFound.Location = new System.Drawing.Point(250, 245);
            this.labelRecordsFound.Name = "labelRecordsFound";
            this.labelRecordsFound.Size = new System.Drawing.Size(86, 13);
            this.labelRecordsFound.TabIndex = 7;
            this.labelRecordsFound.Text = "Records Found: ";
            // 
            // windowTestFHIR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 261);
            this.Controls.Add(this.labelRecordsFound);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAddBob);
            this.Controls.Add(this.btnFind);
            this.Name = "windowTestFHIR";
            this.Text = "Test NoID Spark FHIR";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnAddBob;
        private System.Windows.Forms.TextBox textFirstName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textLastName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textMRN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelRecordsFound;
    }
}


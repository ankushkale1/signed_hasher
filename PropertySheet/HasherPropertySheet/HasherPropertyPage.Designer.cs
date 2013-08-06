namespace signed_hasher
{
    partial class HasherPropertyPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.datagrid = new System.Windows.Forms.DataGridView();
            this.file = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ok = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hashprogress = new System.Windows.Forms.ProgressBar();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btncheck = new System.Windows.Forms.Button();
            this.lblmatched = new System.Windows.Forms.Label();
            this.lblnotmatched = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.datagrid)).BeginInit();
            this.SuspendLayout();
            // 
            // datagrid
            // 
            this.datagrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.datagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.file,
            this.ok});
            this.datagrid.Location = new System.Drawing.Point(3, 3);
            this.datagrid.Name = "datagrid";
            this.datagrid.Size = new System.Drawing.Size(333, 292);
            this.datagrid.TabIndex = 4;
            // 
            // file
            // 
            this.file.HeaderText = "file";
            this.file.Name = "file";
            this.file.ReadOnly = true;
            this.file.Width = 45;
            // 
            // ok
            // 
            this.ok.HeaderText = "Status";
            this.ok.Name = "ok";
            this.ok.ReadOnly = true;
            this.ok.Width = 62;
            // 
            // hashprogress
            // 
            this.hashprogress.Location = new System.Drawing.Point(3, 301);
            this.hashprogress.Name = "hashprogress";
            this.hashprogress.Size = new System.Drawing.Size(333, 17);
            this.hashprogress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.hashprogress.TabIndex = 16;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(50, 324);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 17;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btncheck
            // 
            this.btncheck.Location = new System.Drawing.Point(200, 324);
            this.btncheck.Name = "btncheck";
            this.btncheck.Size = new System.Drawing.Size(75, 23);
            this.btncheck.TabIndex = 18;
            this.btncheck.Text = "Check";
            this.btncheck.UseVisualStyleBackColor = true;
            this.btncheck.Click += new System.EventHandler(this.btncheck_Click);
            // 
            // lblmatched
            // 
            this.lblmatched.AutoSize = true;
            this.lblmatched.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblmatched.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblmatched.Location = new System.Drawing.Point(46, 365);
            this.lblmatched.Name = "lblmatched";
            this.lblmatched.Size = new System.Drawing.Size(79, 20);
            this.lblmatched.TabIndex = 19;
            this.lblmatched.Text = "Matched: ";
            // 
            // lblnotmatched
            // 
            this.lblnotmatched.AutoSize = true;
            this.lblnotmatched.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblnotmatched.ForeColor = System.Drawing.Color.Red;
            this.lblnotmatched.Location = new System.Drawing.Point(196, 365);
            this.lblnotmatched.Name = "lblnotmatched";
            this.lblnotmatched.Size = new System.Drawing.Size(104, 20);
            this.lblnotmatched.TabIndex = 20;
            this.lblnotmatched.Text = "Not Matched:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(282, 409);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Ankyasoft";
            // 
            // HasherPropertyPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblnotmatched);
            this.Controls.Add(this.lblmatched);
            this.Controls.Add(this.btncheck);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.hashprogress);
            this.Controls.Add(this.datagrid);
            this.Name = "HasherPropertyPage";
            this.Size = new System.Drawing.Size(339, 422);
            this.Load += new System.EventHandler(this.FileTimesPropertyPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.datagrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView datagrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn file;
        private System.Windows.Forms.DataGridViewTextBoxColumn ok;
        private System.Windows.Forms.ProgressBar hashprogress;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btncheck;
        private System.Windows.Forms.Label lblmatched;
        private System.Windows.Forms.Label lblnotmatched;
        private System.Windows.Forms.Label label1;


    }
}

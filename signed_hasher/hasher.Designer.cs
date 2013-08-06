namespace signed_hasher
{
    partial class hasher
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(hasher));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtbrowse = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnbrowse = new System.Windows.Forms.Button();
            this.datagrid = new System.Windows.Forms.DataGridView();
            this.file = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ok = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Hash = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblmatched = new System.Windows.Forms.Label();
            this.lblnotmatched = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btncheck = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtcerpath = new System.Windows.Forms.TextBox();
            this.btncert = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkfromstore = new System.Windows.Forms.CheckBox();
            this.btnnewcert = new System.Windows.Forms.Button();
            this.txtpass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.hashprogress = new System.Windows.Forms.ProgressBar();
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.label5 = new System.Windows.Forms.Label();
            this.txtwatchfol = new System.Windows.Forms.TextBox();
            this.btnwatchfolder = new System.Windows.Forms.Button();
            this.trayicon = new System.Windows.Forms.NotifyIcon(this.components);
            this.rightclickmenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemshow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemstopwatch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemexit = new System.Windows.Forms.ToolStripMenuItem();
            this.chkshow = new System.Windows.Forms.CheckBox();
            this.chkiconoverlay = new System.Windows.Forms.CheckBox();
            this.chkboxpropertysheet = new System.Windows.Forms.CheckBox();
            this.btnhelp = new System.Windows.Forms.Button();
            this.chkfromserver = new System.Windows.Forms.CheckBox();
            this.txtfilter = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtclientid = new System.Windows.Forms.TextBox();
            this.txtclisecret = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnsync = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.datagrid)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
            this.rightclickmenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtbrowse
            // 
            this.txtbrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbrowse.Location = new System.Drawing.Point(60, 12);
            this.txtbrowse.Name = "txtbrowse";
            this.txtbrowse.Size = new System.Drawing.Size(313, 20);
            this.txtbrowse.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Browse";
            // 
            // btnbrowse
            // 
            this.btnbrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnbrowse.Location = new System.Drawing.Point(379, 10);
            this.btnbrowse.Name = "btnbrowse";
            this.btnbrowse.Size = new System.Drawing.Size(93, 23);
            this.btnbrowse.TabIndex = 2;
            this.btnbrowse.Text = "Browse";
            this.btnbrowse.UseVisualStyleBackColor = true;
            this.btnbrowse.Click += new System.EventHandler(this.btnbrowse_Click);
            // 
            // datagrid
            // 
            this.datagrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datagrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.datagrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.datagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.file,
            this.ok,
            this.Hash});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.datagrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.datagrid.Location = new System.Drawing.Point(12, 95);
            this.datagrid.Name = "datagrid";
            this.datagrid.Size = new System.Drawing.Size(457, 289);
            this.datagrid.TabIndex = 3;
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
            // Hash
            // 
            this.Hash.HeaderText = "Hash";
            this.Hash.Name = "Hash";
            this.Hash.Width = 57;
            // 
            // lblmatched
            // 
            this.lblmatched.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblmatched.AutoSize = true;
            this.lblmatched.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblmatched.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblmatched.Location = new System.Drawing.Point(76, 439);
            this.lblmatched.Name = "lblmatched";
            this.lblmatched.Size = new System.Drawing.Size(79, 20);
            this.lblmatched.TabIndex = 4;
            this.lblmatched.Text = "Matched: ";
            // 
            // lblnotmatched
            // 
            this.lblnotmatched.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblnotmatched.AutoSize = true;
            this.lblnotmatched.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblnotmatched.ForeColor = System.Drawing.Color.Red;
            this.lblnotmatched.Location = new System.Drawing.Point(348, 439);
            this.lblnotmatched.Name = "lblnotmatched";
            this.lblnotmatched.Size = new System.Drawing.Size(104, 20);
            this.lblnotmatched.TabIndex = 5;
            this.lblnotmatched.Text = "Not Matched:";
            this.lblnotmatched.Click += new System.EventHandler(this.lblnotmatched_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(80, 413);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 6;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btncheck
            // 
            this.btncheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btncheck.Location = new System.Drawing.Point(352, 413);
            this.btncheck.Name = "btncheck";
            this.btncheck.Size = new System.Drawing.Size(75, 23);
            this.btncheck.TabIndex = 7;
            this.btncheck.Text = "Check";
            this.btncheck.UseVisualStyleBackColor = true;
            this.btncheck.Click += new System.EventHandler(this.btncheck_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Select Certificate";
            // 
            // txtcerpath
            // 
            this.txtcerpath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtcerpath.Location = new System.Drawing.Point(102, 56);
            this.txtcerpath.Name = "txtcerpath";
            this.txtcerpath.Size = new System.Drawing.Size(250, 20);
            this.txtcerpath.TabIndex = 9;
            // 
            // btncert
            // 
            this.btncert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btncert.Location = new System.Drawing.Point(358, 56);
            this.btncert.Name = "btncert";
            this.btncert.Size = new System.Drawing.Size(93, 23);
            this.btncert.TabIndex = 10;
            this.btncert.Text = "Browse";
            this.btncert.UseVisualStyleBackColor = true;
            this.btncert.Click += new System.EventHandler(this.btncert_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkfromstore);
            this.groupBox1.Controls.Add(this.btnnewcert);
            this.groupBox1.Controls.Add(this.txtpass);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btncert);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtcerpath);
            this.groupBox1.Location = new System.Drawing.Point(15, 566);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(457, 119);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Certificate";
            // 
            // chkfromstore
            // 
            this.chkfromstore.AutoSize = true;
            this.chkfromstore.Location = new System.Drawing.Point(102, 23);
            this.chkfromstore.Name = "chkfromstore";
            this.chkfromstore.Size = new System.Drawing.Size(127, 17);
            this.chkfromstore.TabIndex = 17;
            this.chkfromstore.Text = "From Certificate Store";
            this.chkfromstore.UseVisualStyleBackColor = true;
            this.chkfromstore.CheckedChanged += new System.EventHandler(this.chkfromstore_CheckedChanged);
            // 
            // btnnewcert
            // 
            this.btnnewcert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnnewcert.Location = new System.Drawing.Point(265, 19);
            this.btnnewcert.Name = "btnnewcert";
            this.btnnewcert.Size = new System.Drawing.Size(87, 23);
            this.btnnewcert.TabIndex = 15;
            this.btnnewcert.Text = "Create New";
            this.btnnewcert.UseVisualStyleBackColor = true;
            this.btnnewcert.Click += new System.EventHandler(this.btnnewcert_Click);
            // 
            // txtpass
            // 
            this.txtpass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtpass.Location = new System.Drawing.Point(102, 89);
            this.txtpass.Name = "txtpass";
            this.txtpass.PasswordChar = '*';
            this.txtpass.Size = new System.Drawing.Size(250, 20);
            this.txtpass.TabIndex = 13;
            this.txtpass.TextChanged += new System.EventHandler(this.txtpass_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Password";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(409, 537);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Ankyasoft©";
            // 
            // hashprogress
            // 
            this.hashprogress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hashprogress.Location = new System.Drawing.Point(9, 390);
            this.hashprogress.Name = "hashprogress";
            this.hashprogress.Size = new System.Drawing.Size(457, 17);
            this.hashprogress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.hashprogress.TabIndex = 15;
            this.hashprogress.Click += new System.EventHandler(this.hashprogress_Click);
            // 
            // fileSystemWatcher
            // 
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.IncludeSubdirectories = true;
            this.fileSystemWatcher.NotifyFilter = ((System.IO.NotifyFilters)((((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName) 
            | System.IO.NotifyFilters.Attributes) 
            | System.IO.NotifyFilters.CreationTime)));
            this.fileSystemWatcher.SynchronizingObject = this;
            this.fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.onchange);
            this.fileSystemWatcher.Created += new System.IO.FileSystemEventHandler(this.onfilecreate);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 479);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Watch Folder";
            // 
            // txtwatchfol
            // 
            this.txtwatchfol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtwatchfol.Location = new System.Drawing.Point(117, 476);
            this.txtwatchfol.Name = "txtwatchfol";
            this.txtwatchfol.Size = new System.Drawing.Size(250, 20);
            this.txtwatchfol.TabIndex = 19;
            // 
            // btnwatchfolder
            // 
            this.btnwatchfolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnwatchfolder.Location = new System.Drawing.Point(373, 474);
            this.btnwatchfolder.Name = "btnwatchfolder";
            this.btnwatchfolder.Size = new System.Drawing.Size(93, 23);
            this.btnwatchfolder.TabIndex = 20;
            this.btnwatchfolder.Text = "Browse";
            this.btnwatchfolder.UseVisualStyleBackColor = true;
            this.btnwatchfolder.Click += new System.EventHandler(this.btnwatchfolder_Click);
            // 
            // trayicon
            // 
            this.trayicon.BalloonTipText = "File Hasher";
            this.trayicon.ContextMenuStrip = this.rightclickmenu;
            this.trayicon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayicon.Icon")));
            this.trayicon.Text = "Hasher";
            this.trayicon.Visible = true;
            this.trayicon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayicon_MouseDoubleClick);
            // 
            // rightclickmenu
            // 
            this.rightclickmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemshow,
            this.toolStripMenuItemstopwatch,
            this.toolStripMenuItemexit});
            this.rightclickmenu.Name = "rightclickmenu";
            this.rightclickmenu.Size = new System.Drawing.Size(153, 70);
            // 
            // toolStripMenuItemshow
            // 
            this.toolStripMenuItemshow.Name = "toolStripMenuItemshow";
            this.toolStripMenuItemshow.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemshow.Text = "Show Me";
            this.toolStripMenuItemshow.Click += new System.EventHandler(this.toolStripMenuItemshow_Click);
            // 
            // toolStripMenuItemstopwatch
            // 
            this.toolStripMenuItemstopwatch.Name = "toolStripMenuItemstopwatch";
            this.toolStripMenuItemstopwatch.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemstopwatch.Text = "Stop Watching";
            this.toolStripMenuItemstopwatch.Click += new System.EventHandler(this.toolStripMenuItemstopwatch_Click);
            // 
            // toolStripMenuItemexit
            // 
            this.toolStripMenuItemexit.Name = "toolStripMenuItemexit";
            this.toolStripMenuItemexit.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemexit.Text = "Exit";
            this.toolStripMenuItemexit.Click += new System.EventHandler(this.toolStripMenuItemexit_Click);
            // 
            // chkshow
            // 
            this.chkshow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkshow.AutoSize = true;
            this.chkshow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkshow.Location = new System.Drawing.Point(334, 514);
            this.chkshow.Name = "chkshow";
            this.chkshow.Size = new System.Drawing.Size(111, 20);
            this.chkshow.TabIndex = 21;
            this.chkshow.Text = "Show Settings";
            this.chkshow.UseVisualStyleBackColor = true;
            this.chkshow.CheckedChanged += new System.EventHandler(this.chkshow_CheckedChanged);
            // 
            // chkiconoverlay
            // 
            this.chkiconoverlay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkiconoverlay.AutoSize = true;
            this.chkiconoverlay.Location = new System.Drawing.Point(12, 516);
            this.chkiconoverlay.Name = "chkiconoverlay";
            this.chkiconoverlay.Size = new System.Drawing.Size(159, 17);
            this.chkiconoverlay.TabIndex = 22;
            this.chkiconoverlay.Text = "Enable Icon OverlayHandler";
            this.chkiconoverlay.UseVisualStyleBackColor = true;
            this.chkiconoverlay.CheckedChanged += new System.EventHandler(this.chkiconoverlay_CheckedChanged);
            // 
            // chkboxpropertysheet
            // 
            this.chkboxpropertysheet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkboxpropertysheet.AutoSize = true;
            this.chkboxpropertysheet.Location = new System.Drawing.Point(187, 516);
            this.chkboxpropertysheet.Name = "chkboxpropertysheet";
            this.chkboxpropertysheet.Size = new System.Drawing.Size(132, 17);
            this.chkboxpropertysheet.TabIndex = 23;
            this.chkboxpropertysheet.Text = "Enable Property Sheet";
            this.chkboxpropertysheet.UseVisualStyleBackColor = true;
            this.chkboxpropertysheet.CheckedChanged += new System.EventHandler(this.chkboxpropertysheet_CheckedChanged);
            // 
            // btnhelp
            // 
            this.btnhelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnhelp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnhelp.BackgroundImage")));
            this.btnhelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnhelp.Location = new System.Drawing.Point(451, 505);
            this.btnhelp.Name = "btnhelp";
            this.btnhelp.Size = new System.Drawing.Size(30, 29);
            this.btnhelp.TabIndex = 24;
            this.btnhelp.UseVisualStyleBackColor = true;
            this.btnhelp.Click += new System.EventHandler(this.btnhelp_Click);
            // 
            // chkfromserver
            // 
            this.chkfromserver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkfromserver.AutoSize = true;
            this.chkfromserver.Location = new System.Drawing.Point(12, 536);
            this.chkfromserver.Name = "chkfromserver";
            this.chkfromserver.Size = new System.Drawing.Size(78, 17);
            this.chkfromserver.TabIndex = 25;
            this.chkfromserver.Text = "SignServer";
            this.chkfromserver.UseVisualStyleBackColor = true;
            this.chkfromserver.Visible = false;
            this.chkfromserver.CheckedChanged += new System.EventHandler(this.chkfromserver_CheckedChanged);
            // 
            // txtfilter
            // 
            this.txtfilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtfilter.Location = new System.Drawing.Point(60, 38);
            this.txtfilter.Name = "txtfilter";
            this.txtfilter.Size = new System.Drawing.Size(313, 24);
            this.txtfilter.TabIndex = 26;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "Filter";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 72);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "Client ID";
            // 
            // txtclientid
            // 
            this.txtclientid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtclientid.Location = new System.Drawing.Point(60, 69);
            this.txtclientid.Name = "txtclientid";
            this.txtclientid.PasswordChar = '*';
            this.txtclientid.Size = new System.Drawing.Size(100, 20);
            this.txtclientid.TabIndex = 29;
            // 
            // txtclisecret
            // 
            this.txtclisecret.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtclisecret.Location = new System.Drawing.Point(273, 69);
            this.txtclisecret.Name = "txtclisecret";
            this.txtclisecret.PasswordChar = '*';
            this.txtclisecret.Size = new System.Drawing.Size(100, 20);
            this.txtclisecret.TabIndex = 30;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(200, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 31;
            this.label8.Text = "Client Secret";
            // 
            // btnsync
            // 
            this.btnsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnsync.BackgroundImage = global::signed_hasher.Properties.Resources.Google_Drive_256;
            this.btnsync.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnsync.Location = new System.Drawing.Point(409, 36);
            this.btnsync.Name = "btnsync";
            this.btnsync.Size = new System.Drawing.Size(60, 49);
            this.btnsync.TabIndex = 32;
            this.btnsync.UseVisualStyleBackColor = true;
            this.btnsync.Click += new System.EventHandler(this.btnsync_Click);
            // 
            // hasher
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 562);
            this.Controls.Add(this.btnsync);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtclisecret);
            this.Controls.Add(this.txtclientid);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtfilter);
            this.Controls.Add(this.chkfromserver);
            this.Controls.Add(this.btnhelp);
            this.Controls.Add(this.chkboxpropertysheet);
            this.Controls.Add(this.chkiconoverlay);
            this.Controls.Add(this.chkshow);
            this.Controls.Add(this.btnwatchfolder);
            this.Controls.Add(this.txtwatchfol);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.hashprogress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btncheck);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.lblnotmatched);
            this.Controls.Add(this.lblmatched);
            this.Controls.Add(this.datagrid);
            this.Controls.Add(this.btnbrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtbrowse);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "hasher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File HashGen";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.hasher_closing);
            this.Load += new System.EventHandler(this.hasher_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.hasher_dragdrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.hasher_dragenter);
            this.Resize += new System.EventHandler(this.hasher_resize);
            ((System.ComponentModel.ISupportInitialize)(this.datagrid)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
            this.rightclickmenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtbrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnbrowse;
        private System.Windows.Forms.Label lblmatched;
        private System.Windows.Forms.Label lblnotmatched;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btncheck;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtcerpath;
        private System.Windows.Forms.Button btncert;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        public System.Windows.Forms.DataGridView datagrid;
        private System.Windows.Forms.TextBox txtpass;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar hashprogress;
        private System.IO.FileSystemWatcher fileSystemWatcher;
        private System.Windows.Forms.Button btnwatchfolder;
        private System.Windows.Forms.TextBox txtwatchfol;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnnewcert;
        private System.Windows.Forms.CheckBox chkfromstore;
        private System.Windows.Forms.NotifyIcon trayicon;
        private System.Windows.Forms.ContextMenuStrip rightclickmenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemshow;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemstopwatch;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemexit;
        private System.Windows.Forms.CheckBox chkshow;
        private System.Windows.Forms.CheckBox chkboxpropertysheet;
        private System.Windows.Forms.CheckBox chkiconoverlay;
        private System.Windows.Forms.Button btnhelp;
        private System.Windows.Forms.CheckBox chkfromserver;
        private System.Windows.Forms.DataGridViewTextBoxColumn file;
        private System.Windows.Forms.DataGridViewTextBoxColumn ok;
        private System.Windows.Forms.DataGridViewTextBoxColumn Hash;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtfilter;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtclisecret;
        private System.Windows.Forms.TextBox txtclientid;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnsync;
    }
}


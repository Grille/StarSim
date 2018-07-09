namespace StarSim
{
    partial class MainWindow
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.TimerDraw = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.selectetStarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.followToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchStarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullscrennToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.showMarkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showStarInfosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSimInfosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyBindingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.jghfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highQualityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TimerDraw
            // 
            this.TimerDraw.Interval = 15;
            this.TimerDraw.Tick += new System.EventHandler(this.TimerDraw_Tick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.simulationToolStripMenuItem,
            this.windowToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem1,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // newToolStripMenuItem1
            // 
            this.newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            resources.ApplyResources(this.newToolStripMenuItem1, "newToolStripMenuItem1");
            this.newToolStripMenuItem1.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            resources.ApplyResources(this.loadToolStripMenuItem, "loadToolStripMenuItem");
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // simulationToolStripMenuItem
            // 
            this.simulationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator2,
            this.selectetStarToolStripMenuItem,
            this.searchStarToolStripMenuItem});
            this.simulationToolStripMenuItem.Name = "simulationToolStripMenuItem";
            resources.ApplyResources(this.simulationToolStripMenuItem, "simulationToolStripMenuItem");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // selectetStarToolStripMenuItem
            // 
            this.selectetStarToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.followToolStripMenuItem,
            this.toolStripSeparator4,
            this.editToolStripMenuItem});
            this.selectetStarToolStripMenuItem.Name = "selectetStarToolStripMenuItem";
            resources.ApplyResources(this.selectetStarToolStripMenuItem, "selectetStarToolStripMenuItem");
            // 
            // followToolStripMenuItem
            // 
            this.followToolStripMenuItem.Name = "followToolStripMenuItem";
            resources.ApplyResources(this.followToolStripMenuItem, "followToolStripMenuItem");
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            // 
            // searchStarToolStripMenuItem
            // 
            this.searchStarToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.searchStarToolStripMenuItem.Name = "searchStarToolStripMenuItem";
            resources.ApplyResources(this.searchStarToolStripMenuItem, "searchStarToolStripMenuItem");
            this.searchStarToolStripMenuItem.Click += new System.EventHandler(this.searchStarToolStripMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullscrennToolStripMenuItem,
            this.toolStripSeparator3,
            this.showMarkerToolStripMenuItem,
            this.showStarInfosToolStripMenuItem,
            this.showSimInfosToolStripMenuItem,
            this.highQualityToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            resources.ApplyResources(this.windowToolStripMenuItem, "windowToolStripMenuItem");
            // 
            // fullscrennToolStripMenuItem
            // 
            this.fullscrennToolStripMenuItem.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.fullscrennToolStripMenuItem.CheckOnClick = true;
            this.fullscrennToolStripMenuItem.Name = "fullscrennToolStripMenuItem";
            resources.ApplyResources(this.fullscrennToolStripMenuItem, "fullscrennToolStripMenuItem");
            this.fullscrennToolStripMenuItem.Click += new System.EventHandler(this.fullscrennToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // showMarkerToolStripMenuItem
            // 
            this.showMarkerToolStripMenuItem.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.showMarkerToolStripMenuItem.Checked = true;
            this.showMarkerToolStripMenuItem.CheckOnClick = true;
            this.showMarkerToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showMarkerToolStripMenuItem.Name = "showMarkerToolStripMenuItem";
            resources.ApplyResources(this.showMarkerToolStripMenuItem, "showMarkerToolStripMenuItem");
            this.showMarkerToolStripMenuItem.Click += new System.EventHandler(this.showMarkerToolStripMenuItem_Click);
            // 
            // showStarInfosToolStripMenuItem
            // 
            this.showStarInfosToolStripMenuItem.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.showStarInfosToolStripMenuItem.Checked = true;
            this.showStarInfosToolStripMenuItem.CheckOnClick = true;
            this.showStarInfosToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showStarInfosToolStripMenuItem.Name = "showStarInfosToolStripMenuItem";
            resources.ApplyResources(this.showStarInfosToolStripMenuItem, "showStarInfosToolStripMenuItem");
            this.showStarInfosToolStripMenuItem.Click += new System.EventHandler(this.showStarInfosToolStripMenuItem_Click);
            // 
            // showSimInfosToolStripMenuItem
            // 
            this.showSimInfosToolStripMenuItem.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.showSimInfosToolStripMenuItem.Checked = true;
            this.showSimInfosToolStripMenuItem.CheckOnClick = true;
            this.showSimInfosToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showSimInfosToolStripMenuItem.Name = "showSimInfosToolStripMenuItem";
            resources.ApplyResources(this.showSimInfosToolStripMenuItem, "showSimInfosToolStripMenuItem");
            this.showSimInfosToolStripMenuItem.Click += new System.EventHandler(this.showSimInfosToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keyBindingsToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // keyBindingsToolStripMenuItem
            // 
            this.keyBindingsToolStripMenuItem.Name = "keyBindingsToolStripMenuItem";
            resources.ApplyResources(this.keyBindingsToolStripMenuItem, "keyBindingsToolStripMenuItem");
            this.keyBindingsToolStripMenuItem.Click += new System.EventHandler(this.keyBindingsToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jghfToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // jghfToolStripMenuItem
            // 
            this.jghfToolStripMenuItem.Name = "jghfToolStripMenuItem";
            resources.ApplyResources(this.jghfToolStripMenuItem, "jghfToolStripMenuItem");
            // 
            // highQualityToolStripMenuItem
            // 
            this.highQualityToolStripMenuItem.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.highQualityToolStripMenuItem.Checked = true;
            this.highQualityToolStripMenuItem.CheckOnClick = true;
            this.highQualityToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.highQualityToolStripMenuItem.Name = "highQualityToolStripMenuItem";
            resources.ApplyResources(this.highQualityToolStripMenuItem, "highQualityToolStripMenuItem");
            // 
            // MainWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.this_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainWindow_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Window_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer TimerDraw;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullscrennToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem jghfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keyBindingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectetStarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchStarToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem showMarkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showStarInfosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSimInfosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem followToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem highQualityToolStripMenuItem;
    }
}


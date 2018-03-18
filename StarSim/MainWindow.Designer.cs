namespace AtomSim
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
            this.buttonStart = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxSpeed = new System.Windows.Forms.TextBox();
            this.textBoxMass = new System.Windows.Forms.TextBox();
            this.textBoxNumber = new System.Windows.Forms.TextBox();
            this.TimerLogik = new System.Windows.Forms.Timer(this.components);
            this.TimerDraw = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonTimeSub = new System.Windows.Forms.Button();
            this.buttonTimeAdd = new System.Windows.Forms.Button();
            this.labelTime = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStart.BackColor = System.Drawing.Color.Black;
            this.buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStart.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStart.ForeColor = System.Drawing.Color.LightGray;
            this.buttonStart.Location = new System.Drawing.Point(145, 452);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(79, 30);
            this.buttonStart.TabIndex = 74;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = false;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.label3.ForeColor = System.Drawing.Color.LightGray;
            this.label3.Location = new System.Drawing.Point(14, 467);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 15);
            this.label3.TabIndex = 81;
            this.label3.Text = "Speed";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.label2.ForeColor = System.Drawing.Color.LightGray;
            this.label2.Location = new System.Drawing.Point(14, 441);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 15);
            this.label2.TabIndex = 80;
            this.label2.Text = "Mass";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Location = new System.Drawing.Point(14, 415);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 79;
            this.label1.Text = "Number";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.BackColor = System.Drawing.Color.Black;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.ForeColor = System.Drawing.Color.LightGray;
            this.buttonClose.Location = new System.Drawing.Point(677, 452);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(79, 30);
            this.buttonClose.TabIndex = 78;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoad.BackColor = System.Drawing.Color.Black;
            this.buttonLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLoad.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLoad.ForeColor = System.Drawing.Color.LightGray;
            this.buttonLoad.Location = new System.Drawing.Point(556, 452);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(79, 30);
            this.buttonLoad.TabIndex = 77;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = false;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.AutoEllipsis = true;
            this.buttonSave.BackColor = System.Drawing.Color.Black;
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSave.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.ForeColor = System.Drawing.Color.LightGray;
            this.buttonSave.Location = new System.Drawing.Point(471, 452);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(79, 30);
            this.buttonSave.TabIndex = 76;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = false;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxSpeed
            // 
            this.textBoxSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxSpeed.BackColor = System.Drawing.Color.Black;
            this.textBoxSpeed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxSpeed.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSpeed.ForeColor = System.Drawing.Color.LightGray;
            this.textBoxSpeed.Location = new System.Drawing.Point(69, 462);
            this.textBoxSpeed.Multiline = true;
            this.textBoxSpeed.Name = "textBoxSpeed";
            this.textBoxSpeed.Size = new System.Drawing.Size(61, 20);
            this.textBoxSpeed.TabIndex = 2;
            this.textBoxSpeed.Text = "4";
            this.textBoxSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxMass
            // 
            this.textBoxMass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxMass.BackColor = System.Drawing.Color.Black;
            this.textBoxMass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxMass.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMass.ForeColor = System.Drawing.Color.LightGray;
            this.textBoxMass.Location = new System.Drawing.Point(69, 436);
            this.textBoxMass.Multiline = true;
            this.textBoxMass.Name = "textBoxMass";
            this.textBoxMass.Size = new System.Drawing.Size(61, 20);
            this.textBoxMass.TabIndex = 1;
            this.textBoxMass.Text = "1";
            this.textBoxMass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxMass.TextChanged += new System.EventHandler(this.textBoxMass_TextChanged);
            // 
            // textBoxNumber
            // 
            this.textBoxNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxNumber.BackColor = System.Drawing.Color.Black;
            this.textBoxNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxNumber.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNumber.ForeColor = System.Drawing.Color.LightGray;
            this.textBoxNumber.Location = new System.Drawing.Point(69, 410);
            this.textBoxNumber.Multiline = true;
            this.textBoxNumber.Name = "textBoxNumber";
            this.textBoxNumber.Size = new System.Drawing.Size(61, 20);
            this.textBoxNumber.TabIndex = 0;
            this.textBoxNumber.Text = "500";
            this.textBoxNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxNumber.TextChanged += new System.EventHandler(this.textBoxNumber_TextChanged);
            // 
            // TimerLogik
            // 
            this.TimerLogik.Interval = 25;
            this.TimerLogik.Tick += new System.EventHandler(this.timerLogik_Tick);
            // 
            // TimerDraw
            // 
            this.TimerDraw.Interval = 5;
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
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStop.BackColor = System.Drawing.Color.Black;
            this.buttonStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStop.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStop.ForeColor = System.Drawing.Color.LightGray;
            this.buttonStop.Location = new System.Drawing.Point(280, 452);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(30, 30);
            this.buttonStop.TabIndex = 82;
            this.buttonStop.Text = ">";
            this.buttonStop.UseVisualStyleBackColor = false;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonTimeSub
            // 
            this.buttonTimeSub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTimeSub.BackColor = System.Drawing.Color.Black;
            this.buttonTimeSub.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTimeSub.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTimeSub.ForeColor = System.Drawing.Color.LightGray;
            this.buttonTimeSub.Location = new System.Drawing.Point(316, 452);
            this.buttonTimeSub.Name = "buttonTimeSub";
            this.buttonTimeSub.Size = new System.Drawing.Size(30, 30);
            this.buttonTimeSub.TabIndex = 83;
            this.buttonTimeSub.Text = "-";
            this.buttonTimeSub.UseVisualStyleBackColor = false;
            this.buttonTimeSub.Click += new System.EventHandler(this.buttonTimeSub_Click);
            // 
            // buttonTimeAdd
            // 
            this.buttonTimeAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTimeAdd.BackColor = System.Drawing.Color.Black;
            this.buttonTimeAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTimeAdd.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTimeAdd.ForeColor = System.Drawing.Color.LightGray;
            this.buttonTimeAdd.Location = new System.Drawing.Point(388, 452);
            this.buttonTimeAdd.Name = "buttonTimeAdd";
            this.buttonTimeAdd.Size = new System.Drawing.Size(30, 30);
            this.buttonTimeAdd.TabIndex = 84;
            this.buttonTimeAdd.Text = "+";
            this.buttonTimeAdd.UseVisualStyleBackColor = false;
            this.buttonTimeAdd.Click += new System.EventHandler(this.buttonTimeAdd_Click);
            // 
            // labelTime
            // 
            this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTime.BackColor = System.Drawing.Color.Black;
            this.labelTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelTime.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.labelTime.ForeColor = System.Drawing.Color.LightGray;
            this.labelTime.Location = new System.Drawing.Point(352, 458);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(30, 19);
            this.labelTime.TabIndex = 85;
            this.labelTime.Text = "1";
            this.labelTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.label5.ForeColor = System.Drawing.Color.LightGray;
            this.label5.Location = new System.Drawing.Point(239, 467);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 15);
            this.label5.TabIndex = 86;
            this.label5.Text = "Time";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(768, 498);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.buttonTimeAdd);
            this.Controls.Add(this.buttonTimeSub);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxNumber);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxMass);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.textBoxSpeed);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainWindow";
            this.Text = "StarSim";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.this_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainWindow_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseDoubleClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Window_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxSpeed;
        private System.Windows.Forms.TextBox textBoxMass;
        private System.Windows.Forms.TextBox textBoxNumber;
        private System.Windows.Forms.Timer TimerLogik;
        private System.Windows.Forms.Timer TimerDraw;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonTimeSub;
        private System.Windows.Forms.Button buttonTimeAdd;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label label5;
    }
}


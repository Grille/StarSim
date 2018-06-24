namespace StarSim
{
    partial class EditStarDialog
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
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxPosX = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxMass = new System.Windows.Forms.TextBox();
            this.textBoxPosY = new System.Windows.Forms.TextBox();
            this.textBoxSpeedX = new System.Windows.Forms.TextBox();
            this.textBoxSpeedY = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(12, 12);
            this.label11.Margin = new System.Windows.Forms.Padding(3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(175, 20);
            this.label11.TabIndex = 22;
            this.label11.Text = "Absolute Star Data";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 119);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = "Used Reference:";
            // 
            // textBoxPosX
            // 
            this.textBoxPosX.Location = new System.Drawing.Point(98, 143);
            this.textBoxPosX.Name = "textBoxPosX";
            this.textBoxPosX.Size = new System.Drawing.Size(89, 20);
            this.textBoxPosX.TabIndex = 25;
            this.textBoxPosX.Text = "1000";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(98, 38);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(89, 20);
            this.textBoxName.TabIndex = 27;
            this.textBoxName.Text = "1000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Name:";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Absolute Point",
            "Center of Mass",
            "Relative Star"});
            this.comboBox1.Location = new System.Drawing.Point(98, 116);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(89, 21);
            this.comboBox1.TabIndex = 28;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(12, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(175, 20);
            this.label2.TabIndex = 29;
            this.label2.Text = "Relative Star Data";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Mass:";
            // 
            // textBoxMass
            // 
            this.textBoxMass.Location = new System.Drawing.Point(98, 64);
            this.textBoxMass.Name = "textBoxMass";
            this.textBoxMass.Size = new System.Drawing.Size(89, 20);
            this.textBoxMass.TabIndex = 31;
            this.textBoxMass.Text = "1000";
            // 
            // textBoxPosY
            // 
            this.textBoxPosY.Location = new System.Drawing.Point(98, 169);
            this.textBoxPosY.Name = "textBoxPosY";
            this.textBoxPosY.Size = new System.Drawing.Size(89, 20);
            this.textBoxPosY.TabIndex = 32;
            this.textBoxPosY.Text = "1000";
            // 
            // textBoxSpeedX
            // 
            this.textBoxSpeedX.Location = new System.Drawing.Point(98, 195);
            this.textBoxSpeedX.Name = "textBoxSpeedX";
            this.textBoxSpeedX.Size = new System.Drawing.Size(89, 20);
            this.textBoxSpeedX.TabIndex = 33;
            this.textBoxSpeedX.Text = "1000";
            // 
            // textBoxSpeedY
            // 
            this.textBoxSpeedY.Location = new System.Drawing.Point(98, 221);
            this.textBoxSpeedY.Name = "textBoxSpeedY";
            this.textBoxSpeedY.Size = new System.Drawing.Size(89, 20);
            this.textBoxSpeedY.TabIndex = 34;
            this.textBoxSpeedY.Text = "1000";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Star Position :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(78, 146);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "X";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(78, 172);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 37;
            this.label6.Text = "Y";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(78, 198);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 13);
            this.label7.TabIndex = 38;
            this.label7.Text = "X";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(78, 224);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(14, 13);
            this.label9.TabIndex = 39;
            this.label9.Text = "Y";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 198);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 13);
            this.label10.TabIndex = 40;
            this.label10.Text = "Star Speed:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(112, 247);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 42;
            this.button2.Text = "cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(17, 247);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 41;
            this.button1.Text = "ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // EditStarDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(199, 278);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxSpeedY);
            this.Controls.Add(this.textBoxSpeedX);
            this.Controls.Add(this.textBoxPosY);
            this.Controls.Add(this.textBoxMass);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPosX);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label11);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditStarDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Edit Star";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditStarDialog_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxPosX;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxMass;
        private System.Windows.Forms.TextBox textBoxPosY;
        private System.Windows.Forms.TextBox textBoxSpeedX;
        private System.Windows.Forms.TextBox textBoxSpeedY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}
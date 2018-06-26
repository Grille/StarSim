namespace StarSim
{
    partial class SearchStarDialog
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
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ColumnStar = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnMass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSpeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonGoTo = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.starBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.starBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(279, 281);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 43;
            this.button2.Text = "cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnStar,
            this.ColumnName,
            this.ColumnMass,
            this.ColumnSpeed});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(343, 263);
            this.dataGridView1.TabIndex = 45;
            this.dataGridView1.Click += new System.EventHandler(this.dataGridView1_Click);
            // 
            // ColumnStar
            // 
            this.ColumnStar.Frozen = true;
            this.ColumnStar.HeaderText = "star";
            this.ColumnStar.Name = "ColumnStar";
            this.ColumnStar.ReadOnly = true;
            this.ColumnStar.Visible = false;
            // 
            // ColumnName
            // 
            this.ColumnName.Frozen = true;
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            this.ColumnName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnMass
            // 
            this.ColumnMass.Frozen = true;
            this.ColumnMass.HeaderText = "Mass";
            this.ColumnMass.Name = "ColumnMass";
            this.ColumnMass.ReadOnly = true;
            this.ColumnMass.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnSpeed
            // 
            this.ColumnSpeed.Frozen = true;
            this.ColumnSpeed.HeaderText = "Speed";
            this.ColumnSpeed.Name = "ColumnSpeed";
            this.ColumnSpeed.ReadOnly = true;
            this.ColumnSpeed.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // buttonGoTo
            // 
            this.buttonGoTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonGoTo.Location = new System.Drawing.Point(12, 281);
            this.buttonGoTo.Name = "buttonGoTo";
            this.buttonGoTo.Size = new System.Drawing.Size(75, 23);
            this.buttonGoTo.TabIndex = 46;
            this.buttonGoTo.Text = "go to";
            this.buttonGoTo.UseVisualStyleBackColor = true;
            this.buttonGoTo.Click += new System.EventHandler(this.buttonGoTo_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEdit.Location = new System.Drawing.Point(93, 281);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(75, 23);
            this.buttonEdit.TabIndex = 47;
            this.buttonEdit.Text = "edit";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // SearchStarDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 312);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonGoTo);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(382, 3051);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(382, 351);
            this.Name = "SearchStarDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Search Star";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.starBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource starBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnStar;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMass;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSpeed;
        private System.Windows.Forms.Button buttonGoTo;
        private System.Windows.Forms.Button buttonEdit;
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StarSim
{
    public partial class NewWorldDialog : Form
    {
        private MainWindow window;
        public NewWorldDialog()
        {
            InitializeComponent();
            comboBoxMode.SelectedIndex = 0;
        }
        public void Show(MainWindow window)
        {
            base.Show(window);
            this.window = window;
            window.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            window.Init(
                comboBoxMode.SelectedIndex, Convert.ToInt32(textBoxSize.Text), Convert.ToInt32(textBoxStars.Text),
                Convert.ToSingle(textBoxMass1.Text), Convert.ToSingle(textBoxMass2.Text), Convert.ToSingle(textBoxSpeed1.Text), Convert.ToSingle(textBoxSpeed2.Text)
                );
            this.Close();
        }

        private void NewWorldDialog_FormClosed(object sender, FormClosedEventArgs e)
        {

            window.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StarSim
{
    public partial class EditStarDialog : Form
    {
        private Star editStar;
        private MainWindow window;

        bool oldTime;
        public EditStarDialog()
        {
            InitializeComponent();
        }
        public void Show(MainWindow window,Star star)
        {

            base.Show(window);
            window.ChildNumber++;
            this.window = window;
            oldTime = Program.Simulation.Running;
            Program.Simulation.Stop();
            Program.Simulation.Wait();
            UpdateStar(star);
            comboBox1.SelectedIndex = 1;
        }
        public void UpdateStar(Star star)
        {
            if (editStar != null) editStar.Editing = false;
            if (star == null) return;
            editStar = star;
            editStar.Editing = true;
            textBoxName.Text = star.Name;
            textBoxMass.Text = "" + star.Mass;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    textBoxPosX.Text = "" + editStar.PosX;
                    textBoxPosY.Text = "" + editStar.PosY;
                    textBoxSpeedX.Text = "" + editStar.SpeedX;
                    textBoxSpeedY.Text = "" + editStar.SpeedY;
                    break;
                case 1:
                    textBoxPosX.Text = "" + (editStar.PosX - Program.Simulation.MassCenterX);
                    textBoxPosY.Text = "" + (editStar.PosY - Program.Simulation.MassCenterY);
                    textBoxSpeedX.Text = "" + (editStar.SpeedX - Program.Simulation.SpeedCenterX);
                    textBoxSpeedY.Text = "" + (editStar.SpeedY - Program.Simulation.SpeedCenterY);
                    break;
                case 2:
                    textBoxPosX.Text = "" + (editStar.PosX - Program.Simulation.RefStar.PosX);
                    textBoxPosY.Text = "" + (editStar.PosY - Program.Simulation.RefStar.PosY);
                    textBoxSpeedX.Text = "" + (editStar.SpeedX - Program.Simulation.RefStar.SpeedX);
                    textBoxSpeedY.Text = "" + (editStar.SpeedY - Program.Simulation.RefStar.SpeedY);
                    break;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            editStar.Name = textBoxName.Text;
            editStar.UpdateMass(Convert.ToSingle(textBoxMass.Text));

            editStar.PosX = (Convert.ToSingle(textBoxPosX.Text));
            editStar.PosY = (Convert.ToSingle(textBoxPosY.Text));
            editStar.SpeedX = (Convert.ToSingle(textBoxSpeedX.Text));
            editStar.SpeedY = (Convert.ToSingle(textBoxSpeedY.Text));
            //this.Close();
        }

        private void EditStarDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (editStar != null) editStar.Editing = false;
            Program.Simulation.Running = oldTime;
            window.ChildNumber--;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStar(editStar);
        }
    }
}

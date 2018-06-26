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
            this.window = window;
            oldTime = window.Simulation.Running;
            window.Simulation.Stop();
            window.Simulation.Wait();
            UpdateStar(star);
        }
        public void UpdateStar(Star star)
        {
            if (editStar != null) editStar.Editing = false;
            if (star == null) return;
            editStar = star;
            editStar.Editing = true;
            textBoxName.Text = star.Name;
            textBoxMass.Text = "" + star.Mass;

            textBoxPosX.Text = "" + editStar.PosX;
            textBoxPosY.Text = "" + editStar.PosY;
            textBoxSpeedX.Text = "" + editStar.SpeedX;
            textBoxSpeedY.Text = "" + editStar.SpeedY;
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
            window.Simulation.Running = oldTime;
        }
    }
}

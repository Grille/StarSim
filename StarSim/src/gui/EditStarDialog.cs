using System;
using System.Windows.Forms;

namespace StarSim
{
    public partial class EditStarDialog : Form
    {
        int oldIndex2 = 0;
        public int SelectetIndex { get; private set; } = 0;

        private StarSim simulation;
        private Star editStar;
        private MainWindow window;
        private bool readEnabled;
        private double posX = 0, posY = 0, speedX = 0, speedY = 0;
        bool oldTime;
        public EditStarDialog()
        {
            InitializeComponent();
        }
        public void Show(MainWindow window, StarSim sim, Star star)
        {

            base.Show(window);
            window.ChildNumber++;
            simulation = sim;
            this.window = window;
            oldTime = simulation.Running;
            simulation.Stop();
            simulation.Wait();

            editStar = star;
            editStar.Editor = this;
            textBoxName.Text = star.Name;
            textBoxMass.Text = "" + star.Mass;
            textBoxDensity.Text = "" + star.Density;

            updateComboBox();
            if (simulation.RefStar != null && editStar != simulation.RefStar) comboBox1.SelectedIndex = 3;
            else comboBox1.SelectedIndex = 2;

            readEnabled = true;
        }
        public void UpdateGui()
        {
            int round = 6;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    textBoxPosX.Text = "" + Math.Round(posX, round);
                    textBoxPosY.Text = "" + Math.Round(posY, round);
                    textBoxSpeedX.Text = "" + Math.Round(speedX, round);
                    textBoxSpeedY.Text = "" + Math.Round(speedY, round);
                    break;
                case 1:
                    textBoxPosX.Text = "" + Math.Round(editStar.PosX + posX, round);
                    textBoxPosY.Text = "" + Math.Round(editStar.PosY + posY, round);
                    textBoxSpeedX.Text = "" + Math.Round(editStar.SpeedX + speedX, round);
                    textBoxSpeedY.Text = "" + Math.Round(editStar.SpeedY + speedY, round);
                    break;
                case 2:
                    textBoxPosX.Text = "" + Math.Round(editStar.PosX - simulation.MassCenterX + posX, round);
                    textBoxPosY.Text = "" + Math.Round(editStar.PosY - simulation.MassCenterY + posY, round);
                    textBoxSpeedX.Text = "" + Math.Round(editStar.SpeedX - simulation.SpeedCenterX + speedX, round);
                    textBoxSpeedY.Text = "" + Math.Round(editStar.SpeedY - simulation.SpeedCenterY + speedY, round);
                    break;
                case 3:
                    textBoxPosX.Text = "" + Math.Round(editStar.PosX - simulation.RefStar.PosX + posX, round);
                    textBoxPosY.Text = "" + Math.Round(editStar.PosY - simulation.RefStar.PosY + posY, round);
                    textBoxSpeedX.Text = "" + Math.Round(editStar.SpeedX - simulation.RefStar.SpeedX + speedX, round);
                    textBoxSpeedY.Text = "" + Math.Round(editStar.SpeedY - simulation.RefStar.SpeedY + speedY, round);
                    break;
            }
            Refresh();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            updateComboBox();

            editStar.Name = textBoxName.Text;
            editStar.Density = (Convert.ToSingle(textBoxDensity.Text));
            editStar.UpdateMass(Convert.ToSingle(textBoxMass.Text));

            readTextBox();

            editStar.PosX += posX;
            editStar.PosY += posY;
            editStar.SpeedX += speedX;
            editStar.SpeedY += speedY;

            posX = 0; posY = 0; speedX = 0; speedY = 0;
            window.ViewChange = true;

            UpdateGui();
        }

        private void EditStarDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (editStar != null) editStar.Editor = null;
            if (editStar.Mass == 0) editStar.Enabled = false;
            simulation.Running = oldTime;
            window.ChildNumber--;
            window.ViewChange = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            oldIndex2 = SelectetIndex;
            SelectetIndex = comboBox1.SelectedIndex;
            readTextBox();
            UpdateGui();
            window.ViewChange = true;
        }

        private unsafe void readTextBox()
        {
            Console.WriteLine("\n------------------------------------- index:" + oldIndex2);

            Console.WriteLine("posX 1--> " + posX);
            Console.WriteLine("posY 1--> " + posY);
            if (!readEnabled) return;
            //double tbposX = double.NaN, tbposY = double.NaN, tbspeedX = double.NaN, tbspeedY = double.NaN;

            bool eposX = double.TryParse(textBoxPosX.Text, out double tbposX);
            bool eposY = double.TryParse(textBoxPosY.Text, out double tbposY);
            bool espeedX = double.TryParse(textBoxSpeedX.Text, out double tbspeedX);
            bool espeedY = double.TryParse(textBoxSpeedY.Text, out double tbspeedY);

            Console.WriteLine("tbposX--> " + tbposX);
            Console.WriteLine("tbposY--> " + tbposY);
            switch (oldIndex2)
            {
                case 0:
                    if (eposX) posX = tbposX;
                    if (eposY) posY = tbposY;
                    if (espeedX) speedX = tbspeedX;
                    if (!double.IsNaN(tbspeedY)) speedY = tbspeedY;
                    break;
                case 1:
                    if (eposX) posX = tbposX - editStar.PosX;
                    if (eposY) posY = tbposY - editStar.PosY;
                    if (espeedX) speedX = tbspeedX - editStar.SpeedX;
                    if (!double.IsNaN(tbspeedY)) speedY = tbspeedY - editStar.SpeedY;
                    break;
                case 2:
                    if (eposX) posX = tbposX - (editStar.PosX - simulation.MassCenterX);
                    if (eposY) posY = tbposY - (editStar.PosY - simulation.MassCenterY);
                    if (espeedX) speedX = tbspeedX - (editStar.SpeedX - simulation.SpeedCenterX);
                    if (!double.IsNaN(tbspeedY)) speedY = tbspeedY - (editStar.SpeedY - simulation.SpeedCenterY);
                    break;
                case 3:
                    if (eposX) posX = tbposX - (editStar.PosX - simulation.RefStar.PosX);
                    if (eposY) posY = tbposY - (editStar.PosY - simulation.RefStar.PosY);
                    if (espeedX) speedX = tbspeedX - (editStar.SpeedX - simulation.RefStar.SpeedX);
                    if (espeedY) speedY = tbspeedY - (editStar.SpeedY - simulation.RefStar.SpeedY);
                    break;
            }
            Console.WriteLine("posX 2--> " + posX);
            Console.WriteLine("posY 2--> " + posY);
        }

        private void updateComboBox()
        {

            int index = comboBox1.SelectedIndex;
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Null");
            comboBox1.Items.Add("Absolute");
            comboBox1.Items.Add("Center");
            if (simulation.RefStar != null && editStar != simulation.RefStar) comboBox1.Items.Add("Relative");
            else if (index == 3) index = 2;
            comboBox1.SelectedIndex = index;
            comboBox1.Refresh();

        }

        private void comboBox1_MouseEnter(object sender, EventArgs e)
        {
            updateComboBox();
        }
    }
}

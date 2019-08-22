using System;
using System.Windows.Forms;

namespace StarSim
{
    public partial class EditStarDialog : Form
    {
        int selectetIndex = 0, oldIndex2 = 0;
        public int SelectetIndex
        {
            get {
                return selectetIndex;
            }
        }
        
        private Star editStar;
        private MainWindow window;
        private bool readEnabled;
        private double posX = 0, posY = 0, speedX = 0, speedY = 0;
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

            editStar = star;
            editStar.Editor = this;
            textBoxName.Text = star.Name;
            textBoxMass.Text = "" + star.Mass;
            textBoxDensity.Text = "" + star.Density;

            updateComboBox();
            if (Program.Simulation.RefStar != null && editStar != Program.Simulation.RefStar) comboBox1.SelectedIndex = 3;
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
                    textBoxPosX.Text = "" + Math.Round(editStar.PosX - Program.Simulation.MassCenterX + posX, round);
                    textBoxPosY.Text = "" + Math.Round(editStar.PosY - Program.Simulation.MassCenterY + posY, round);
                    textBoxSpeedX.Text = "" + Math.Round(editStar.SpeedX - Program.Simulation.SpeedCenterX + speedX, round);
                    textBoxSpeedY.Text = "" + Math.Round(editStar.SpeedY - Program.Simulation.SpeedCenterY + speedY, round);
                    break;
                case 3:
                    textBoxPosX.Text = "" + Math.Round(editStar.PosX - Program.Simulation.RefStar.PosX + posX, round);
                    textBoxPosY.Text = "" + Math.Round(editStar.PosY - Program.Simulation.RefStar.PosY + posY, round);
                    textBoxSpeedX.Text = "" + Math.Round(editStar.SpeedX - Program.Simulation.RefStar.SpeedX + speedX, round);
                    textBoxSpeedY.Text = "" + Math.Round(editStar.SpeedY - Program.Simulation.RefStar.SpeedY + speedY, round);
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

            posX = 0;posY = 0;speedX = 0;speedY = 0;
            window.ViewChange = true;

            UpdateGui();
        }

        private void EditStarDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (editStar != null) editStar.Editor = null;
            if (editStar.Mass == 0) editStar.Enabled = false;
            Program.Simulation.Running = oldTime;
            window.ChildNumber--;
            window.ViewChange = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            oldIndex2 = selectetIndex;
            selectetIndex = comboBox1.SelectedIndex;
            readTextBox();
            UpdateGui();
            window.ViewChange = true;
        }

        unsafe private void readTextBox()
        {
            Console.WriteLine("\n------------------------------------- index:" + oldIndex2);

            Console.WriteLine("posX 1--> " + posX);
            Console.WriteLine("posY 1--> " + posY);
            if (!readEnabled) return;
            double tbposX = double.NaN, tbposY = double.NaN, tbspeedX = double.NaN, tbspeedY = double.NaN;

            try { tbposX = Convert.ToDouble(textBoxPosX.Text); }
            catch (Exception) { }
            try { tbposY = Convert.ToDouble(textBoxPosY.Text); }
            catch (Exception) { }
            try { tbspeedX = Convert.ToDouble(textBoxSpeedX.Text); }
            catch (Exception) { }
            try { tbspeedY = Convert.ToDouble(textBoxSpeedY.Text); }
            catch (Exception) { }

            Console.WriteLine("tbposX--> " + tbposX);
            Console.WriteLine("tbposY--> " + tbposY);
            switch (oldIndex2)
            {
                case 0:
                    if (!double.IsNaN(tbposX)) posX = tbposX;
                    if (!double.IsNaN(tbposY)) posY = tbposY;
                    if (!double.IsNaN(tbspeedX)) speedX = tbspeedX;
                    if (!double.IsNaN(tbspeedY)) speedY = tbspeedY;
                    break;
                case 1:
                    if (!double.IsNaN(tbposX)) posX = tbposX - editStar.PosX;
                    if (!double.IsNaN(tbposY)) posY = tbposY- editStar.PosY;
                    if (!double.IsNaN(tbspeedX)) speedX = tbspeedX- editStar.SpeedX;
                    if (!double.IsNaN(tbspeedY)) speedY = tbspeedY- editStar.SpeedY;
                    break;
                case 2:
                    if (!double.IsNaN(tbposX)) posX = tbposX - (editStar.PosX- Program.Simulation.MassCenterX);
                    if (!double.IsNaN(tbposY)) posY = tbposY - (editStar.PosY- Program.Simulation.MassCenterY);
                    if (!double.IsNaN(tbspeedX)) speedX = tbspeedX - (editStar.SpeedX- Program.Simulation.SpeedCenterX);
                    if (!double.IsNaN(tbspeedY)) speedY = tbspeedY - (editStar.SpeedY- Program.Simulation.SpeedCenterY);
                    break;
                case 3:
                    if (!double.IsNaN(tbposX)) posX = tbposX - (editStar.PosX- Program.Simulation.RefStar.PosX);
                    if (!double.IsNaN(tbposY)) posY = tbposY - (editStar.PosY- Program.Simulation.RefStar.PosY);
                    if (!double.IsNaN(tbspeedX)) speedX = tbspeedX - (editStar.SpeedX- Program.Simulation.RefStar.SpeedX);
                    if (!double.IsNaN(tbspeedY)) speedY = tbspeedY - (editStar.SpeedY- Program.Simulation.RefStar.SpeedY);
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
            if (Program.Simulation.RefStar != null && editStar != Program.Simulation.RefStar) comboBox1.Items.Add("Relative");
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

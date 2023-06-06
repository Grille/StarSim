using System;
using System.Windows.Forms;

namespace StarSim
{
    public partial class EditStarDialog : Form
    {
        enum Tab
        {
            Null,
            Absolute,
            Center,
            Relative,
        }

        int oldIndex2 = 0;
        public int SelectetIndex { get; private set; } = 0;

        private Game game;
        private SimulationData data;
        private Star editStar;
        private MainWindow window;
        private bool readEnabled;
        private double posX = 0, posY = 0, speedX = 0, speedY = 0;
        bool oldTime;
        public EditStarDialog()
        {
            InitializeComponent();
        }
        public void Show(MainWindow window, Game game, Star star)
        {

            base.Show(window);
            this.game = game;
            window.ChildNumber++;
            data = game.Data;
            this.window = window;
            oldTime = game.Running;

            game.Timer.Lock();

            editStar = star;
            editStar.Editor = this;
            textBoxName.Text = star.Name;
            textBoxMass.Text = "" + star.Mass;
            textBoxDensity.Text = "" + star.Density;

            updateComboBox();
            if (data.RefStar != null && editStar != data.RefStar) comboBox1.SelectedIndex = 3;
            else comboBox1.SelectedIndex = 2;

            readEnabled = true;
        }
        public void UpdateGui()
        {
            switch (comboBox1.SelectedIndex)
            {
                case (int)Tab.Null:
                    setTextBoxValue(textBoxPosX, posX);
                    setTextBoxValue(textBoxPosY, posY);
                    setTextBoxValue(textBoxSpeedX, speedX);
                    setTextBoxValue(textBoxSpeedY, speedY);
                    break;
                case (int)Tab.Absolute:
                    setTextBoxValue(textBoxPosX, editStar.PosX + posX);
                    setTextBoxValue(textBoxPosY, editStar.PosY + posY);
                    setTextBoxValue(textBoxSpeedX, editStar.SpeedX + speedX);
                    setTextBoxValue(textBoxSpeedY, editStar.SpeedY + speedY);
                    break;
                case (int)Tab.Center:
                    setTextBoxValue(textBoxPosX, editStar.PosX - data.MassCenterX + posX);
                    setTextBoxValue(textBoxPosY, editStar.PosY - data.MassCenterY + posY);
                    setTextBoxValue(textBoxSpeedX, editStar.SpeedX - data.SpeedCenterX + speedX);
                    setTextBoxValue(textBoxSpeedY, editStar.SpeedY - data.SpeedCenterY + speedY);
                    break;
                case (int)Tab.Relative:
                    setTextBoxValue(textBoxPosX, editStar.PosX - data.RefStar.PosX + posX);
                    setTextBoxValue(textBoxPosY, editStar.PosY - data.RefStar.PosY + posY);
                    setTextBoxValue(textBoxSpeedX, editStar.SpeedX - data.RefStar.SpeedX + speedX);
                    setTextBoxValue(textBoxSpeedY, editStar.SpeedY - data.RefStar.SpeedY + speedY);
                    break;
            }
            Refresh();
        }

        private void setTextBoxValue(TextBox control, double value)
        {
            textBoxPosX.Text = Math.Round(editStar.PosX - data.RefStar.PosX + posX, 6).ToString();
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
            game.ViewChanged = true;

            UpdateGui();
        }

        private void EditStarDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (editStar != null) editStar.Editor = null;
            if (editStar.Mass == 0) editStar.Enabled = false;
            game.Running = oldTime;
            window.ChildNumber--;
            game.ViewChanged = true;

            game.Timer.Free();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            oldIndex2 = SelectetIndex;
            SelectetIndex = comboBox1.SelectedIndex;
            readTextBox();
            UpdateGui();
            game.ViewChanged = true;
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
                case (int)Tab.Null:
                    if (eposX) posX = tbposX;
                    if (eposY) posY = tbposY;
                    if (espeedX) speedX = tbspeedX;
                    if (espeedY) speedY = tbspeedY;
                    break;
                case (int)Tab.Absolute:
                    if (eposX) posX = tbposX - editStar.PosX;
                    if (eposY) posY = tbposY - editStar.PosY;
                    if (espeedX) speedX = tbspeedX - editStar.SpeedX;
                    if (espeedY) speedY = tbspeedY - editStar.SpeedY;
                    break;
                case (int)Tab.Center:
                    if (eposX) posX = tbposX - (editStar.PosX - data.MassCenterX);
                    if (eposY) posY = tbposY - (editStar.PosY - data.MassCenterY);
                    if (espeedX) speedX = tbspeedX - (editStar.SpeedX - data.SpeedCenterX);
                    if (espeedY) speedY = tbspeedY - (editStar.SpeedY - data.SpeedCenterY);
                    break;
                case (int)Tab.Relative:
                    if (eposX) posX = tbposX - (editStar.PosX - data.RefStar.PosX);
                    if (eposY) posY = tbposY - (editStar.PosY - data.RefStar.PosY);
                    if (espeedX) speedX = tbspeedX - (editStar.SpeedX - data.RefStar.SpeedX);
                    if (espeedY) speedY = tbspeedY - (editStar.SpeedY - data.RefStar.SpeedY);
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
            if (data.RefStar != null && editStar != data.RefStar) comboBox1.Items.Add("Relative");
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

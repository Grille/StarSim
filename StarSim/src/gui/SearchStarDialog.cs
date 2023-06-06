using System;
using System.Windows.Forms;

namespace StarSim
{
    public partial class SearchStarDialog : Form
    {
        MainWindow window;
        Game game;
        SimulationData stars;
        bool oldTime;
        public SearchStarDialog()
        {
            InitializeComponent();
        }

        public void Show(MainWindow window, Game game)
        {
            base.Show(window);
            this.game = game;
            window.ChildNumber++;
            this.window = window;
            this.stars = game.Data;
            oldTime = game.Running;
            game.Timer.Lock();
            SetStars(stars);
        }

        public void SetStars(SimulationData stars)
        {
            if (stars == null)
            {
                buttonGoTo.Enabled = false;
                buttonEdit.Enabled = false;
            }
            else
            {
                dataGridView1.ShowEditingIcon = false;
                this.dataGridView1.Rows.Clear();
                for (int i = 0; i < stars.Count; i++)
                {
                    if (stars[i].Enabled) this.dataGridView1.Rows.Add(i, stars[i].Name, Math.Round(stars[i].Mass, 2), Math.Round(Math.Abs(stars[i].SpeedX) + Math.Abs(stars[i].SpeedY), 2));
                }
            }
            dataGridView1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            game.Running = oldTime;
        }

        private void buttonGoTo_Click(object sender, EventArgs e)
        {
            game.Camera.PosX = stars[(int)dataGridView1.CurrentRow.Cells[0].Value].PosX;
            game.Camera.PosY = stars[(int)dataGridView1.CurrentRow.Cells[0].Value].PosY;
            game.ViewChanged = true;
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            new EditStarDialog().Show(window, game, stars[(int)dataGridView1.CurrentRow.Cells[0].Value]);
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            //game.SelectetStar = stars[(int)dataGridView1.CurrentRow.Cells[0].Value];
            window.Focus();
        }

        private void SearchStarDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            window.ChildNumber--;
            game.Timer.Free();
        }
    }
}

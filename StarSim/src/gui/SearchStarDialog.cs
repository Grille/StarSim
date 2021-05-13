using System;
using System.Windows.Forms;

namespace StarSim
{
    public partial class SearchStarDialog : Form
    {
        MainWindow window;
        StarSim simulation;
        Star[] stars;
        bool oldTime;
        public SearchStarDialog()
        {
            InitializeComponent();
        }

        public void Show(MainWindow window, StarSim sim)
        {
            base.Show(window);
            window.ChildNumber++;
            this.window = window;
            this.simulation = sim;
            this.stars = sim.Stars;
            oldTime = simulation.Running;
            simulation.Stop();
            simulation.Wait();
            SetStars(stars);
        }
        public void SetStars(Star[] stars)
        {
            this.stars = stars;
            if (stars == null)
            {
                buttonGoTo.Enabled = false;
                buttonEdit.Enabled = false;
            }
            else
            {
                dataGridView1.ShowEditingIcon = false;
                this.dataGridView1.Rows.Clear();
                for (int i = 0; i < stars.Length; i++)
                {
                    if (stars[i].Enabled) this.dataGridView1.Rows.Add(i, stars[i].Name, Math.Round(stars[i].Mass, 2), Math.Round(Math.Abs(stars[i].SpeedX) + Math.Abs(stars[i].SpeedY), 2));
                }
            }
            dataGridView1.Refresh();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            simulation.Running = oldTime;
        }

        private void buttonGoTo_Click(object sender, EventArgs e)
        {
            window.Camera.PosX = -stars[(int)dataGridView1.CurrentRow.Cells[0].Value].PosX;
            window.Camera.PosY = -stars[(int)dataGridView1.CurrentRow.Cells[0].Value].PosY;
            window.ViewChange = true;
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            new EditStarDialog().Show(window, simulation, stars[(int)dataGridView1.CurrentRow.Cells[0].Value]);
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            simulation.SelectetStar = stars[(int)dataGridView1.CurrentRow.Cells[0].Value];
            window.Focus();
        }

        private void SearchStarDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            window.ChildNumber--;
        }
    }
}

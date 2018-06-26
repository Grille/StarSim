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
    public partial class SearchStarDialog : Form
    {
        MainWindow window;
        Star[] stars;
        bool oldTime;
        public SearchStarDialog()
        {
            InitializeComponent();
            //dataGridView1.CurrentCell.Value = "xfdgch";
        }
        public void Show(MainWindow window, Star[] stars)
        {
            base.Show(window);
            this.window = window;
            oldTime = window.Simulation.Running;
            window.Simulation.Stop();
            window.Simulation.Wait();
            SetStars(stars);
        }
        public void SetStars(Star[] stars)
        {
            this.stars = stars;
            //window.Wait();
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
            window.Simulation.Running = oldTime;
        }

        private void buttonGoTo_Click(object sender, EventArgs e)
        {
            window.camPosX = -stars[(int)dataGridView1.CurrentRow.Cells[0].Value].PosX;
            window.camPosY = -stars[(int)dataGridView1.CurrentRow.Cells[0].Value].PosY;
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            new EditStarDialog().Show(window, stars[(int)dataGridView1.CurrentRow.Cells[0].Value]);
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            window.Simulation.SelectetStar = (int)dataGridView1.CurrentRow.Cells[0].Value;
            window.Focus();
        }
    }
}

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
            oldTime = window.Running;
            window.TimeRun(false);
            Update(stars);
        }
        public void Update(Star[] stars)
        {
            this.stars = stars;

            if (stars == null)
            {
                buttonSelect.Enabled = false;
                buttonGoTo.Enabled = false;
                buttonEdit.Enabled = false;
            }
            else
            {
                dataGridView1.ShowEditingIcon = false;

                for (int iSrc = 0; iSrc < stars.Length; iSrc++)
                {
                    if (stars[iSrc].Enabled) this.dataGridView1.Rows.Add(iSrc, stars[iSrc].Name, Math.Round(stars[iSrc].Mass, 2), Math.Round(Math.Abs(stars[iSrc].SpeedX) + Math.Abs(stars[iSrc].SpeedY), 2));
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            window.TimeRun(oldTime);
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            window.curStar = (int)dataGridView1.CurrentRow.Cells[0].Value;
            window.Focus();
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
    }
}

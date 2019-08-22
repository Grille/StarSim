using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GGL.IO;
using System.Globalization;
using GGL.Graphic;

namespace StarSim
{
    public unsafe partial class MainWindow : Form
    {
        private MouseEventArgs lastRightClick = new MouseEventArgs(MouseButtons.None,0,0,0,0);
        public bool ViewChange = true;
        StarSim simulation;
        private Renderer renderer;
        public Renderer Renderer
        {
            get { return renderer; }
        }
        public int ChildNumber = 0;

        public SearchStarDialog searchStarDialog = new SearchStarDialog();

        Point lastMousePos = new Point(0,0);

        int usedTime;

        public MainWindow()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(Window_MouseWheel);

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer,true);

            DoubleBuffered = true;
            simulation = Program.Simulation;
            renderer = new Renderer(simulation);
            simulation.FrameCalculatet += new EventHandler(folow);
            simulation.Start();
            TimerDraw.Start();
        }
        ~MainWindow()
        {
            //Application.Exit();
        }

        public void Init(int mode, int size, int stars, float minMass, float maxMass, float disSpeed)
        {
            simulation.SelectetStar = null; simulation.FocusStar = null; simulation.RefStar = null;
            renderer.CamPosX = renderer.CamPosY = 0;
            renderer.scaling = Math.Min(this.Width, this.Height) / (float)((size+ disSpeed*32) * 1.2f);
            simulation.Init(mode, size, stars, minMass, maxMass, disSpeed);
            ViewChange = true;
        }
        void folow(object sender,EventArgs e)
        {
            if (simulation.FocusStar != null && simulation.FocusStar.Enabled == true)
            {
                renderer.CamPosX -= simulation.FocusStar.SpeedX;
                renderer.CamPosY -= simulation.FocusStar.SpeedY;
            }
            else
            {
                renderer.CamPosX -= simulation.SpeedCenterX;
                renderer.CamPosY -= simulation.SpeedCenterY;
            }
        }

        private void TimerDraw_Tick(object sender, EventArgs e)
        {
            if (ViewChange | simulation.NewFrameCalculatet)
            {
                this.Invalidate();
            }
            ViewChange = false;
        }
        private void this_Paint(object sender, PaintEventArgs e)
        {
            renderer.HighRenderQuality = highQualityToolStripMenuItem.Checked;
            renderer.Render(sender, e.Graphics);
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                renderer.CamPosX += ((e.X - lastMousePos.X)/ renderer.scaling);
                renderer.CamPosY += ((e.Y - lastMousePos.Y) / renderer.scaling);
                ViewChange = true;
            }
            lastMousePos = e.Location;
        }
        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
  
            //get worldPos
            double posX = -renderer.CamPosX + (e.X - Width / 2d) / renderer.scaling;
            double posY = -renderer.CamPosY + (e.Y - Height / 2d) / renderer.scaling;

            renderer.scaling += (e.Delta * renderer.scaling) / 500d;
            if (renderer.scaling < 0.00001) renderer.scaling = 0.00001f;
            else if (renderer.scaling > 10) renderer.scaling = 10;

            renderer.CamPosX = -posX + (Width / 2d * (e.X / (double)Width * 2d - 1)) / renderer.scaling;
            renderer.CamPosY = -posY + (Height / 2d * (e.Y / (double)Height * 2d - 1)) / renderer.scaling;
            ViewChange = true;
        }

        private void MainWindow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            selectStar(e);
        }
        private void MainWindow_MouseClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine(e.Button);
            if (e.Button == MouseButtons.Left) return;
            selectStar(e);
        }
        private void selectStar(MouseEventArgs e)
        {
            float posX = (float)(-renderer.CamPosX + (e.X - Width / 2)/ renderer.scaling);
            float posY = (float)(-renderer.CamPosY + (e.Y - Height / 2)/ renderer.scaling);
            Star nearestStar = null;

            bool firstStar = true;
            double maxdist = 0;

            for (int iS = 0; iS < simulation.Stars.Length; iS++)
            {
                if (simulation.Stars[iS].Enabled == true)
                { //Vergleicher jedes object mit jedem anderen
                    double distX = posX - (simulation.Stars[iS].PosX + 0);
                    double distY = posY - (simulation.Stars[iS].PosY + 0);
                    if (firstStar)
                    {
                        maxdist = (float)Math.Sqrt((distX * distX) + (distY * distY));
                        firstStar = false;
                        nearestStar = simulation.Stars[iS];
                    }
                    else
                    {
                        float dist = (float)Math.Sqrt((distX * distX) + (distY * distY));
                        if (dist < maxdist)
                        {
                            maxdist = dist;
                            nearestStar = simulation.Stars[iS];
                        }
                    }

                }
            }
            simulation.SelectetStar = nearestStar;
            ViewChange = true;
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            simulation.SelectetStar = null; simulation.FocusStar = null; simulation.RefStar = null;
            simulation.Wait();

            ByteStream bs = new ByteStream(openFileDialog.FileName);
            bs.ResetIndex();

            bs.ReadByte();
            int starArrayLenght = bs.ReadInt();
            renderer.CamPosX = bs.ReadFloat();
            renderer.CamPosY = bs.ReadFloat();
            renderer.scaling = bs.ReadFloat();
            Star[] stars = new Star[starArrayLenght];
            for (int i = 0; i < starArrayLenght; i++)
            {
                stars[i] = new Star(bs.ReadInt(),bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat());
                stars[i].Name = bs.ReadString();
            }
            int starIdx;
            if ((starIdx = bs.ReadInt()) != -1) simulation.SelectetStar = stars[starIdx];
            if ((starIdx = bs.ReadInt()) != -1) simulation.FocusStar = stars[starIdx];
            if ((starIdx = bs.ReadInt()) != -1) simulation.RefStar = stars[starIdx];
            simulation.Stars = stars;
        }
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            
            simulation.Wait();

            simulation.CollapseStarArray();
            ByteStream bs = new ByteStream();
            bs.WriteByte(0);

            bs.WriteInt(simulation.Stars.Length);
            bs.WriteFloat((float)renderer.CamPosX);
            bs.WriteFloat((float)renderer.CamPosY);
            bs.WriteFloat((float)renderer.scaling);
            Star[] stars = simulation.Stars;
            for (int i = 0; i < simulation.Stars.Length; i++)
            {
                bs.WriteInt(stars[i].Idx);
                bs.WriteFloat(stars[i].Mass);
                bs.WriteFloat((float)stars[i].PosX);
                bs.WriteFloat((float)stars[i].PosY);
                bs.WriteFloat((float)stars[i].SpeedX);
                bs.WriteFloat((float)stars[i].SpeedY);
                bs.WriteString(stars[i].Name);
            }
            bs.WriteInt(simulation.SelectetStar != null ? simulation.SelectetStar.Idx : -1);
            bs.WriteInt(simulation.FocusStar != null ? simulation.FocusStar.Idx : -1);
            bs.WriteInt(simulation.RefStar != null ? simulation.RefStar.Idx : -1);
            bs.Save(saveFileDialog.FileName);
            
        }
        private void MainWindow_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            
            switch (e.KeyData)
            {
                case Keys.Space:
                    if (simulation.Running) simulation.Stop();
                    else if (ChildNumber <= 0) simulation.Start();
                    break;
                case Keys.F:
                    if (simulation.FocusStar == simulation.SelectetStar) simulation.FocusStar = null;
                    else simulation.FocusStar = simulation.SelectetStar;
                    break;
                case Keys.R:
                    if (simulation.RefStar == simulation.SelectetStar) simulation.RefStar = null;
                    else simulation.RefStar = simulation.SelectetStar;
                    break;
                case Keys.E:
                    if (simulation.SelectetStar != null)
                    {
                        if (simulation.SelectetStar.Editor == null)
                            new EditStarDialog().Show(this, simulation.SelectetStar);
                        else simulation.SelectetStar.Editor.Focus();
                    }
                    break;
                case Keys.M:
                    if (simulation.SelectetStar != null) simulation.SelectetStar.Marked = !simulation.SelectetStar.Marked;
                    break;
                case Keys.T:
                    if (simulation.SelectetStar != null) simulation.SelectetStar.Tracked = !simulation.SelectetStar.Tracked;
                    break;
                case Keys.OemPeriod:
                    if (simulation.SimSpeed < 512) simulation.SimSpeed *= 2;
                    break;
                case Keys.Oemcomma:
                    if (simulation.SimSpeed > 1) simulation.SimSpeed /= 2;
                    break;
            }
            ViewChange = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            simulation.Wait();
            saveFileDialog.DefaultExt = "sm";
            saveFileDialog.AddExtension = true;
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog.ShowDialog();
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "sm";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.ShowDialog();
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new NewWorldDialog().Show(this);
        }

        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            lastMousePos.X = e.X;
            lastMousePos.Y = e.Y;
        }
        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)lastRightClick = e;
        }

        private void searchStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchStarDialog = new SearchStarDialog();
            searchStarDialog.Show(this,simulation.Stars);
        }

        private void showMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.showMarker = ((ToolStripMenuItem)sender).Checked;
        }
        private void showStarInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.showStarInfo = ((ToolStripMenuItem)sender).Checked;
        }
        private void showSimInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.showSimInfo = ((ToolStripMenuItem)sender).Checked;
        }
        private void keyBindingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"
Space:  Pause/Run
Dot(.):  Increase speed
Comma(,):  lower speed
F:  Camera focus to selcetet star
R:  Set selcetet star as physics reference
", "Key Bindings");
        }
        private void fullscrennToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormBorderStyle != FormBorderStyle.None)
            {
                FormBorderStyle = FormBorderStyle.None;
                if (WindowState == FormWindowState.Maximized)
                    WindowState = FormWindowState.Normal;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void jghfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float posX = (float)(-renderer.CamPosX + (lastRightClick.X - Width / 2) / renderer.scaling);
            float posY = (float)(-renderer.CamPosY + (lastRightClick.Y - Height / 2) / renderer.scaling);
            Program.Simulation.AddStar(1, posX, posY, 0, 0);

            ViewChange = true;
        }

        private void followToolStripMenuItem_Click(object sender, EventArgs e)
        {

                    if (simulation.FocusStar == simulation.SelectetStar) simulation.FocusStar = null;
            else simulation.FocusStar = simulation.SelectetStar;

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

                    if (simulation.SelectetStar != null)
            {
                if (simulation.SelectetStar.Editor == null)
                    new EditStarDialog().Show(this, simulation.SelectetStar);
                else simulation.SelectetStar.Editor.Focus();
            }
        }
    }
}

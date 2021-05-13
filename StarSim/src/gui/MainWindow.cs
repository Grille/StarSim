using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GGL;

namespace StarSim
{
    public unsafe partial class MainWindow : Form
    {
        private MouseEventArgs lastRightClick = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
        public bool ViewChange = true;
        private StarSim simulation;

        public Camera Camera;
        public Renderer Renderer { get; }
        public int ChildNumber = 0;

        public MainWindow()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(Window_MouseWheel);

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            DoubleBuffered = true;

            simulation = new StarSim();
            Camera = new Camera();
            Renderer = new Renderer(Camera,simulation);

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
            Camera.Scale = Math.Min(this.Width, this.Height) / (float)((size + disSpeed * 32) * 1.2f);
            simulation.Init(mode, size, stars, minMass, maxMass, disSpeed);
            ViewChange = true;
        }
        void folow(object sender, EventArgs e)
        {
            if (simulation.FocusStar != null && simulation.FocusStar.Enabled == true)
            {
                Camera.PosX += simulation.FocusStar.SpeedX;
                Camera.PosY += simulation.FocusStar.SpeedY;
            }
            else
            {
                Camera.PosX += simulation.SpeedCenterX;
                Camera.PosY += simulation.SpeedCenterY;
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
            Camera.SetScreenSize(ClientSize.Width, ClientSize.Height);
            Renderer.HighRenderQuality = highQualityToolStripMenuItem.Checked;
            Renderer.Render(sender, e.Graphics);
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Camera.MouseMove(e, e.Button == MouseButtons.Left);
            ViewChange = true;
        }
        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
            Camera.MouseWheel(e, 1.5);
            //0.00001f; 10;
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
            Camera.ScreenToWorldSpace(e.X, e.Y, out double posX, out double posY);
            Star nearestStar = null;

            bool firstStar = true;
            double maxdist = 0;

            for (int iS = 0; iS < simulation.Stars.Length; iS++)
            {
                if (simulation.Stars[iS].Enabled == true)
                {
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

            using (var bs = new BinaryView(openFileDialog.FileName))
            {
                bs.Decompress();

                bs.ReadByte();
                int starArrayLenght = bs.ReadInt32();
                Camera.PosX = bs.ReadSingle();
                Camera.PosY = bs.ReadSingle();
                Camera.Scale = bs.ReadSingle();
                Star[] stars = new Star[starArrayLenght];
                for (int i = 0; i < starArrayLenght; i++)
                {
                    stars[i] = new Star(bs.ReadInt32(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
                    stars[i].Name = bs.ReadString();
                }
                int starIdx;
                if ((starIdx = bs.ReadInt32()) != -1) simulation.SelectetStar = stars[starIdx];
                if ((starIdx = bs.ReadInt32()) != -1) simulation.FocusStar = stars[starIdx];
                if ((starIdx = bs.ReadInt32()) != -1) simulation.RefStar = stars[starIdx];
                simulation.Stars = stars;
            }
        }
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {

            simulation.Wait();

            simulation.CollapseStarArray();

            using (var bs = new BinaryView(saveFileDialog.FileName, false))
            {
                bs.WriteByte(0);

                bs.WriteInt32(simulation.Stars.Length);
                bs.WriteDouble(Camera.PosX);
                bs.WriteDouble(Camera.PosY);
                bs.WriteDouble(Camera.Scale);
                Star[] stars = simulation.Stars;
                for (int i = 0; i < simulation.Stars.Length; i++)
                {
                    bs.WriteInt32(stars[i].Idx);
                    bs.WriteDouble(stars[i].Mass);
                    bs.WriteDouble(stars[i].PosX);
                    bs.WriteDouble(stars[i].PosY);
                    bs.WriteDouble(stars[i].SpeedX);
                    bs.WriteDouble(stars[i].SpeedY);
                    bs.WriteString(stars[i].Name);
                }
                bs.WriteInt32(simulation.SelectetStar != null ? simulation.SelectetStar.Idx : -1);
                bs.WriteInt32(simulation.FocusStar != null ? simulation.FocusStar.Idx : -1);
                bs.WriteInt32(simulation.RefStar != null ? simulation.RefStar.Idx : -1);

                bs.Compress();
            }

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
                            new EditStarDialog().Show(this, simulation, simulation.SelectetStar);
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
                    if (simulation.SimSpeed > 0.001) simulation.SimSpeed /= 2;
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
            var dialog = new NewWorldDialog();
            dialog.Show(this);
        }

        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            Camera.MouseMove(e, false);
        }
        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) lastRightClick = e;
        }

        private void searchStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new SearchStarDialog();
            dialog.Show(this, simulation);
        }

        private void showMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Renderer.ShowMarker = ((ToolStripMenuItem)sender).Checked;
        }
        private void showStarInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Renderer.ShowStarInfo = ((ToolStripMenuItem)sender).Checked;
        }
        private void showSimInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Renderer.ShowSimInfo = ((ToolStripMenuItem)sender).Checked;
        }
        private void keyBindingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Space:  Pause/Run\n" +
                "Dot(.):  Increase speed\n" +
                "Comma(,):  lower speed\n" +
                "F:  Camera focus to selcetet star\n" +
                "R:  Set selcetet star as physics reference\n"
                , 
                "Key Bindings"
            );
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

        private void jghfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Camera.ScreenToWorldSpace(lastRightClick.X, lastRightClick.Y, out double posX, out double posY);
            simulation.AddStar(1, posX, posY, 0, 0);

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
                    new EditStarDialog().Show(this, simulation, simulation.SelectetStar);
                else simulation.SelectetStar.Editor.Focus();
            }
        }
    }
}

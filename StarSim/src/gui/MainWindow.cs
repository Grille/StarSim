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

        public Renderer Renderer { get; }
        public int ChildNumber = 0;

        Point lastMousePos = new Point(0, 0);

        public MainWindow()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(Window_MouseWheel);

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            DoubleBuffered = true;
            simulation = Program.Simulation;
            Renderer = new Renderer(simulation);
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
            Renderer.CamPosX = Renderer.CamPosY = 0;
            Renderer.scaling = Math.Min(this.Width, this.Height) / (float)((size + disSpeed * 32) * 1.2f);
            simulation.Init(mode, size, stars, minMass, maxMass, disSpeed);
            ViewChange = true;
        }
        void folow(object sender, EventArgs e)
        {
            if (simulation.FocusStar != null && simulation.FocusStar.Enabled == true)
            {
                Renderer.CamPosX -= simulation.FocusStar.SpeedX;
                Renderer.CamPosY -= simulation.FocusStar.SpeedY;
            }
            else
            {
                Renderer.CamPosX -= simulation.SpeedCenterX;
                Renderer.CamPosY -= simulation.SpeedCenterY;
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
            Renderer.HighRenderQuality = highQualityToolStripMenuItem.Checked;
            Renderer.Render(sender, e.Graphics);
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Renderer.CamPosX += ((e.X - lastMousePos.X) / Renderer.scaling);
                Renderer.CamPosY += ((e.Y - lastMousePos.Y) / Renderer.scaling);
                ViewChange = true;
            }
            lastMousePos = e.Location;
        }
        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
            double posX = -Renderer.CamPosX + (e.X - Width / 2d) / Renderer.scaling;
            double posY = -Renderer.CamPosY + (e.Y - Height / 2d) / Renderer.scaling;

            Renderer.scaling += (e.Delta * Renderer.scaling) / 500d;
            if (Renderer.scaling < 0.00001) Renderer.scaling = 0.00001f;
            else if (Renderer.scaling > 10) Renderer.scaling = 10;

            Renderer.CamPosX = -posX + (Width / 2d * (e.X / (double)Width * 2d - 1)) / Renderer.scaling;
            Renderer.CamPosY = -posY + (Height / 2d * (e.Y / (double)Height * 2d - 1)) / Renderer.scaling;
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
            float posX = (float)(-Renderer.CamPosX + (e.X - Width / 2) / Renderer.scaling);
            float posY = (float)(-Renderer.CamPosY + (e.Y - Height / 2) / Renderer.scaling);
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
                bs.ReadByte();
                int starArrayLenght = bs.ReadInt32();
                Renderer.CamPosX = bs.ReadSingle();
                Renderer.CamPosY = bs.ReadSingle();
                Renderer.scaling = bs.ReadSingle();
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
            using (var bs = new BinaryView())
            {
                bs.WriteByte(0);

                bs.WriteInt32(simulation.Stars.Length);
                bs.WriteSingle((float)Renderer.CamPosX);
                bs.WriteSingle((float)Renderer.CamPosY);
                bs.WriteSingle((float)Renderer.scaling);
                Star[] stars = simulation.Stars;
                for (int i = 0; i < simulation.Stars.Length; i++)
                {
                    bs.WriteInt32(stars[i].Idx);
                    bs.WriteSingle(stars[i].Mass);
                    bs.WriteSingle((float)stars[i].PosX);
                    bs.WriteSingle((float)stars[i].PosY);
                    bs.WriteSingle((float)stars[i].SpeedX);
                    bs.WriteSingle((float)stars[i].SpeedY);
                    bs.WriteString(stars[i].Name);
                }
                bs.WriteInt32(simulation.SelectetStar != null ? simulation.SelectetStar.Idx : -1);
                bs.WriteInt32(simulation.FocusStar != null ? simulation.FocusStar.Idx : -1);
                bs.WriteInt32(simulation.RefStar != null ? simulation.RefStar.Idx : -1);
                bs.Save(saveFileDialog.FileName);
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
            var dialog = new NewWorldDialog();
            dialog.Show(this);
        }

        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            lastMousePos.X = e.X;
            lastMousePos.Y = e.Y;
        }
        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) lastRightClick = e;
        }

        private void searchStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new SearchStarDialog();
            dialog.Show(this, simulation.Stars);
        }

        private void showMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Renderer.showMarker = ((ToolStripMenuItem)sender).Checked;
        }
        private void showStarInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Renderer.showStarInfo = ((ToolStripMenuItem)sender).Checked;
        }
        private void showSimInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Renderer.showSimInfo = ((ToolStripMenuItem)sender).Checked;
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
            float posX = (float)(-Renderer.CamPosX + (lastRightClick.X - Width / 2) / Renderer.scaling);
            float posY = (float)(-Renderer.CamPosY + (lastRightClick.Y - Height / 2) / Renderer.scaling);
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

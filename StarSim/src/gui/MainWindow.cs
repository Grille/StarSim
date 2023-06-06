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

        private Game game;
        private SimulationData data;
        private Camera camera;
        private Renderer renderer;

        public int ChildNumber = 0;

        public MainWindow()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(Window_MouseWheel);

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            game = new Game();
            camera = game.Camera;
            data = game.Data;
            renderer = new Renderer(game);

            game.Start();

            TimerDraw.Start();
        }

        public void Init(int mode, int size, int stars, float minMass, float maxMass, float disSpeed)
        {
            game.Timer.Lock();

            data.SelectetStar = null; data.FocusStar = null; data.RefStar = null;
            camera.Scale = Math.Min(this.Width, this.Height) / (float)((size + disSpeed * 32) * 1.2f);
            game.Data.Init(mode, size, stars, minMass, maxMass, disSpeed);
            game.ViewChanged = true;

            game.Timer.Free();
        }

        private void TimerDraw_Tick(object sender, EventArgs e)
        {
            if (game.ViewChanged)
            {
                Invalidate();
                game.ViewChanged = false;
            }
        }
        private void this_Paint(object sender, PaintEventArgs e)
        {
            camera.SetScreenSize(ClientSize.Width, ClientSize.Height);
            renderer.HighRenderQuality = highQualityToolStripMenuItem.Checked;
            renderer.Render(sender, e.Graphics);
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            camera.MouseMove(e, e.Button == MouseButtons.Left);
        }
        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
            camera.MouseWheel(e, 1.5);
            //0.00001f; 10;
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
            camera.ScreenToWorldSpace(e.X, e.Y, out double posX, out double posY);
            Star nearestStar = null;

            bool firstStar = true;
            double maxdist = 0;

            for (int iS = 0; iS < game.Data.Count; iS++)
            {
                if (game.Data[iS].Enabled == true)
                {
                    double distX = posX - (game.Data[iS].PosX + 0);
                    double distY = posY - (game.Data[iS].PosY + 0);
                    if (firstStar)
                    {
                        maxdist = (float)Math.Sqrt((distX * distX) + (distY * distY));
                        firstStar = false;
                        nearestStar = game.Data[iS];
                    }
                    else
                    {
                        float dist = (float)Math.Sqrt((distX * distX) + (distY * distY));
                        if (dist < maxdist)
                        {
                            maxdist = dist;
                            nearestStar = game.Data[iS];
                        }
                    }

                }
            }
            data.SelectetStar = nearestStar;
            camera.Changed = true;
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            game.Load(saveFileDialog.FileName);
        }
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            game.Save(saveFileDialog.FileName);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyData)
            {
                case Keys.Space:
                    if (game.Running) game.Stop();
                    else if (ChildNumber <= 0) game.Start();
                    break;
                case Keys.F:
                    if (data.FocusStar == data.SelectetStar) data.FocusStar = null;
                    else data.FocusStar = data.SelectetStar;
                    break;
                case Keys.R:
                    if (data.RefStar == data.SelectetStar) data.RefStar = null;
                    else data.RefStar = data.SelectetStar;
                    break;
                case Keys.E:
                    if (data.SelectetStar != null)
                    {
                        if (data.SelectetStar.Editor == null)
                            new EditStarDialog().Show(this, game, data.SelectetStar);
                        else data.SelectetStar.Editor.Focus();
                    }
                    break;
                case Keys.M:
                    if (data.SelectetStar != null) data.SelectetStar.Marked = !data.SelectetStar.Marked;
                    break;
                case Keys.T:
                    if (data.SelectetStar != null) data.SelectetStar.Tracked = !data.SelectetStar.Tracked;
                    break;
                case Keys.OemPeriod:
                    if (game.Sim.SimSpeed < 512) game.Sim.SimSpeed *= 2;
                    break;
                case Keys.Oemcomma:
                    if (game.Sim.SimSpeed > 1) game.Sim.SimSpeed /= 2;
                    break;
            }
            camera.Changed = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.Timer.Lock();

            saveFileDialog.DefaultExt = "sm";
            saveFileDialog.AddExtension = true;
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog.ShowDialog();

            game.Timer.Free();
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
            camera.MouseMove(e, false);
        }
        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) lastRightClick = e;
        }

        private void searchStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new SearchStarDialog();
            dialog.Show(this, game);
        }

        private void showMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.ShowMarker = ((ToolStripMenuItem)sender).Checked;
        }
        private void showStarInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.ShowStarInfo = ((ToolStripMenuItem)sender).Checked;
        }
        private void showSimInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.ShowSimInfo = ((ToolStripMenuItem)sender).Checked;
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

        private void addStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            camera.ScreenToWorldSpace(lastRightClick.X, lastRightClick.Y, out double posX, out double posY);
            game.Data.AddStar(1, posX, posY, 0, 0);

            camera.Changed = true;
        }

        private void followToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (data.FocusStar == data.SelectetStar) data.FocusStar = null;
            else data.FocusStar = data.SelectetStar;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (data.SelectetStar != null)
            {
                if (data.SelectetStar.Editor == null)
                    new EditStarDialog().Show(this, game, data.SelectetStar);
                else data.SelectetStar.Editor.Focus();
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}

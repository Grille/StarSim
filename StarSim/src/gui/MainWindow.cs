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
        private StarSim simulation;
        public StarSim Simulation{
            get { return simulation; }
        }
        public double camPosX, camPosY;
        double scaling = 1;


        bool showMarker = true;
        bool showStarInfo = true;
        bool showSimInfo = true;

        public SearchStarDialog searchStarDialog = new SearchStarDialog();

        Graphics g;

        Point lastMousePos = new Point(0,0);

        int usedTime;

        public MainWindow()
        {
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            InitializeComponent();
            MouseWheel += new MouseEventHandler(Window_MouseWheel);

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer,true);

            DoubleBuffered = true;
            simulation = new StarSim();
            simulation.FrameCalculatet += new EventHandler(folow);
            simulation.Start();
            TimerDraw.Start();
            //Feld.Refresh();

        }
        ~MainWindow()
        {
            //Application.Exit();
        }

        public void Init(int mode, int size, int stars, float minMass, float maxMass, float disSpeed)
        {
            camPosX = camPosY = 0;
            scaling = Math.Min(this.Width, this.Height) / (float)(size * 1.2f);
            simulation.Init(mode, size, stars, minMass, maxMass, disSpeed);
        }
        void folow(object sender,EventArgs e)
        {
            if (simulation.FocusStar != -1 && simulation.Stars[simulation.FocusStar].Enabled == true)
            {
                camPosX -= (float)simulation.Stars[simulation.FocusStar].SpeedX;
                camPosY -= (float)simulation.Stars[simulation.FocusStar].SpeedY;
            }
            else
            {
                camPosX -= (float)simulation.SpeedCenterX;
                camPosY -= (float)simulation.SpeedCenterY;
            }
        }

        private void TimerDraw_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
            //Refresh();
        }
        private void transformPoint(float x, float y, out float outX, out float outY)
        {
            outX = (x + (float)camPosX) * (float)scaling + Width / 2f; 
            outY = (y + (float)camPosY) * (float)scaling + Height / 2f;
        }
        private void fixPoint(ref float posX,ref float posY,float refX,float refY)
        {

            float mX = (refX - posX) / (refY - posY);
            float nullPosX = posX - mX * posY;

            float mY = (refY - posY) / (refX - posX);
            float nullPosY = posY - mY * posX;

            if (posX < 0)
            {
                posY = nullPosY;
                posX = 0;
            }
            if (posY < 0)
            {
                posX = nullPosX;
                posY = 0;
            }
            if (posX > Width)
            {
                posY = mY * Width + nullPosY;
                posX = Width;
            }
            if (posY > Height)
            {
                posX = mX * Height + nullPosX;
                posY = Height;
            }
        }
        private void drawLine(Pen pen,float posX1,float posY1,float posX2,float posY2)
        {
            
            if (posX1 < 0 && posX2 < 0) return;
            if (posY1 < 0 && posY2 < 0) return;
            if (posX1 > Width && posX2 > Width) return;
            if (posY1 > Height && posY2 > Height) return;
            
            fixPoint(ref posX1,ref posY1, posX2, posY2);
            fixPoint(ref posX2, ref posY2, posX1, posY1);

            /*
            g.DrawEllipse(new Pen(Color.Red, 2), posX1-10, posY1-10, 20, 20);
            g.DrawEllipse(new Pen(Color.Red, 2), posX2 - 10, posY2 - 10, 20, 20);

            /*
            g.DrawString("X: " + posX1, new Font("Consolas", 9), new SolidBrush(Color.Red), new PointF(posX1, posY1));
            */

            g.DrawLine(pen, posX1,posY1, posX2, posY2);
        }
        private void this_Paint(object sender, PaintEventArgs e)
        {
            Stopwatch SWTotal = new Stopwatch();
            SWTotal.Start();
            g = e.Graphics;

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //g.TranslateTransform(Width / 2, Height / 2);
            //g.ScaleTransform(scaling, scaling);
            Pen backPen = new Pen(Color.FromArgb(30, 45, 45),1);
            backPen.DashPattern = new float[] { 8, 4};
            Pen backPen2 = new Pen(Color.FromArgb(30, 45, 45), 1);
            backPen2.DashPattern = new float[] { 8, 4,4,8 };
            Pen guiLine = new Pen(Color.FromArgb(40, 60, 80), 1);

            /*
            g.DrawLine(backPen, 0, Height / 2, Width, Height / 2);
            g.DrawLine(backPen, Width/2, 0 , Width / 2, Height);
            */

            Star[] starArray = simulation.Stars;
            int curStar = simulation.SelectetStar, refStar = simulation.RefStar, focusStar = simulation.FocusStar;
            if (showMarker)
            {
                float centerPosX, centerPosY;
                transformPoint((float)simulation.MassCenterX, (float)simulation.MassCenterY, out centerPosX, out centerPosY);

                g.DrawEllipse(backPen, centerPosX - 20f, centerPosY - 20f, 40, 40);

                /*
                g.DrawLine(backPen, 0, Height / 2, Width, Height / 2);
                g.DrawLine(backPen, Width/2, 0 , Width / 2, Height);
                */
                float refPosX = centerPosX, refPosY = centerPosY;
                if (refStar != -1)
                {
                    transformPoint((float)starArray[refStar].PosX, (float)starArray[refStar].PosY,out refPosX,out refPosY);
                    g.DrawEllipse(backPen, refPosX - 10, refPosY - 10, 20, 20);
                    drawLine(backPen, centerPosX, centerPosY, refPosX, refPosY);
                }
                float focusPosX = centerPosX, focusPosY = centerPosY;
                if (focusStar != -1)
                {
                    transformPoint((float)starArray[focusStar].PosX, (float)starArray[focusStar].PosY,out focusPosX,out focusPosY);
                }
                int dist1 = 10, dist2 = 40;
                drawLine(backPen, focusPosX + dist1, focusPosY + dist1, focusPosX + dist2, focusPosY + dist2);
                drawLine(backPen, focusPosX - dist1, focusPosY + dist1, focusPosX - dist2, focusPosY + dist2);
                drawLine(backPen, focusPosX + dist1, focusPosY - dist1, focusPosX + dist2, focusPosY - dist2);
                drawLine(backPen, focusPosX - dist1, focusPosY - dist1, focusPosX - dist2, focusPosY - dist2);

                if (curStar != -1)
                {
                    float curPosX, curPosY;
                    transformPoint((float)starArray[curStar].PosX, (float)starArray[curStar].PosY,out curPosX,out curPosY);
                    drawLine(backPen2, curPosX, curPosY, refPosX, refPosY);
                }
            }


            //try
            //{
            SolidBrush brush = new SolidBrush(Color.LightGray);

            for (int iS = 0; iS < simulation.Stars.Length; iS++)
            {
                if (!starArray[iS].Enabled) continue;

                float r = starArray[iS].SizeR;

                byte RColor = (byte)starArray[iS].Mass;
                float posX = (float)(starArray[iS].PosX - r + camPosX);
                float posY = (float)(starArray[iS].PosY - r + camPosY);

                posX *= (float)scaling; posY *= (float)scaling; r *= (float)scaling;
                posX += Width / 2f; posY += Height / 2f;

                if (r < 0.01) r = 0.01f;
                if (iS == curStar)
                {
                    if (starArray[iS].Mass > 0) g.DrawEllipse(new Pen(Color.FromArgb(170, 255, 255), 1), posX, posY, r * 2, r * 2);
                    else g.DrawEllipse(new Pen(Color.FromArgb(255, 170, 255), 1), posX, posY, r * 2, r * 2);
                }
                else
                {
                    if (starArray[iS].Mass > 0) g.DrawEllipse(new Pen(Color.FromArgb(100, 200, 255), 1), posX, posY, r * 2, r * 2);
                    else g.DrawEllipse(new Pen(Color.FromArgb(200, 100, 255), 1), posX, posY, r * 2, r * 2);
                }

                if ((curStar == iS || starArray[iS].Marked) && showStarInfo || starArray[iS].Editing)
                {
                    posX += r;
                    posY += r;
                    g.DrawLine(guiLine, new PointF(posX, posY), new PointF(posX + r * 1.5f, posY - r * 1.5f));
                    g.DrawLine(guiLine, new PointF(posX + r * 1.5f, posY - r * 1.5f), new PointF(posX + r * 1.5f + r * 2f + 4f, posY - r * 1.5f));
                    int txtPosX = (int)(posX + r * 1.5f + r * 2f + 9f);
                    int txtPosY = (int)(posY - r * 1.5f - 6f)-15;
                    if (starArray[iS].Name.Length > 0)g.DrawString("" + ((starArray[iS].Name.Length > 0) ? starArray[iS].Name : String.Format("{0:X}", starArray[iS].ID)), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY+=15));
                    g.DrawString(""+Math.Round(starArray[iS].Mass, 2), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                    if (starArray[iS].Editing)
                    {
                        g.DrawString(""+Math.Round(Math.Abs(starArray[iS].SpeedX)+Math.Abs(starArray[iS].SpeedY), 2), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                    }
                    txtPosY += 5;

                    /*
                    g.DrawString("Relative data to " +((refStar != -1) ? ((starArray[refStar].Name.Length > 0) ? starArray[refStar].Name : String.Format("{0:X}", starArray[refStar].ID)) : "Nothing"), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                    g.DrawString("Dist: " + Math.Abs((int)(massCenterX - starArray[iS].PosX)) + " posY: " +Math.Abs((int)(massCenterY - starArray[iS].PosY)), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                    g.DrawString("Speed: " + starArray[iS].SpeedX + " speedY: " + Math.Round(starArray[iS].SpeedY, 2), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                    //g.DrawString("Speed: " + (Math.Abs(starArray[ii].SpeedX) + Math.Abs(starArray[ii].SpeedY)), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
               */
                }

            }

            if (showSimInfo)
            {
                g.DrawString("Stars " + simulation.StarCount + " /" + simulation.Stars.Length, new Font("Consolas", 9), brush, new Point(10, 40));
                g.DrawString("Mass " + "-", new Font("Consolas", 9), brush, new Point(10, 50));
                g.DrawString("SimTime " + simulation.UsedTime, new Font("Consolas", 9), brush, new Point(10, 60));
                g.DrawString("SimSpeed " + simulation.SimSpeed, new Font("Consolas", 9), brush, new Point(10, 70));
                /*
                g.DrawString("L_Time " + usedTime + "ms", new Font("Consolas", 9), brush, new Point(10, 50));
                g.DrawString("R_Time " + SWTotal.ElapsedMilliseconds + "ms", new Font("Consolas", 9), brush, new Point(10, 60));
                */
            }
            SWTotal.Stop();
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                camPosX += ((e.X - lastMousePos.X)/scaling);
                camPosY += ((e.Y - lastMousePos.Y) / scaling);
            }
            lastMousePos.X = e.X;
            lastMousePos.Y = e.Y;
        }
        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
  
            //get worldPos
            double posX = -camPosX + (e.X - Width / 2d) / scaling;
            double posY = -camPosY + (e.Y - Height / 2d) / scaling;

            scaling += (e.Delta * scaling) / 500d;
            if (scaling < 0.00001) scaling = 0.00001f;
            else if (scaling > 10) scaling = 10;

            camPosX = -posX + (Width / 2d * (e.X / (double)Width * 2d - 1)) / scaling;
            camPosY = -posY + (Height / 2d * (e.Y / (double)Height * 2d - 1)) / scaling;
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
            float posX = (float)(-camPosX + (e.X - Width / 2)/ scaling);
            float posY = (float)(-camPosY + (e.Y - Height / 2)/scaling);
            int nearestStar = -1;

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
                        nearestStar = iS;
                    }
                    else
                    {
                        float dist = (float)Math.Sqrt((distX * distX) + (distY * distY));
                        if (dist < maxdist)
                        {
                            maxdist = dist;
                            nearestStar = iS;
                        }
                    }

                }
            }
            simulation.SelectetStar = nearestStar;
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            simulation.Wait();

            ByteStream bs = new ByteStream(openFileDialog.FileName);
            bs.ResetIndex();

            bs.ReadByte();
            int starArrayLenght = bs.ReadInt();
            camPosX = bs.ReadFloat();
            camPosY = bs.ReadFloat();
            scaling = bs.ReadFloat();
            Star[] stars = new Star[starArrayLenght];
            for (int i = 0; i < starArrayLenght; i++)
            {
                stars[i] = new Star(bs.ReadInt(),bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat());
                stars[i].Name = bs.ReadString();
            }
            simulation.SelectetStar = bs.ReadInt();
            simulation.FocusStar = bs.ReadInt();
            simulation.RefStar = bs.ReadInt();
            simulation.Stars = stars;

        }
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            
            simulation.Wait();

            simulation.CollapseStarArray();
            ByteStream bs = new ByteStream();
            bs.WriteByte(0);

            bs.WriteInt(simulation.Stars.Length);
            bs.WriteFloat((float)camPosX);
            bs.WriteFloat((float)camPosY);
            bs.WriteFloat((float)scaling);
            Star[] stars = simulation.Stars;
            for (int i = 0; i < simulation.Stars.Length; i++)
            {
                bs.WriteInt(stars[i].ID);
                bs.WriteFloat(stars[i].Mass);
                bs.WriteFloat((float)stars[i].PosX);
                bs.WriteFloat((float)stars[i].PosY);
                bs.WriteFloat((float)stars[i].SpeedX);
                bs.WriteFloat((float)stars[i].SpeedY);
                bs.WriteString(stars[i].Name);
            }
            bs.WriteInt(simulation.SelectetStar);
            bs.WriteInt(simulation.FocusStar);
            bs.WriteInt(simulation.RefStar);
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
                    else simulation.Start();
                    break;
                case Keys.F:
                    if (simulation.FocusStar == simulation.SelectetStar) simulation.FocusStar = -1;
                    else simulation.FocusStar = simulation.SelectetStar;
                    break;
                case Keys.R:
                    if (simulation.RefStar == simulation.SelectetStar) simulation.RefStar = -1;
                    else simulation.RefStar = simulation.SelectetStar;
                    break;
                case Keys.E:
                    if (simulation.SelectetStar != -1) new EditStarDialog().Show(this, simulation.Stars[simulation.SelectetStar]);
                    break;
                case Keys.M:
                    if (simulation.SelectetStar != -1) simulation.Stars[simulation.SelectetStar].Marked = !simulation.Stars[simulation.SelectetStar].Marked;
                    break;
                case Keys.T:
                    if (simulation.SelectetStar != -1) simulation.Stars[simulation.SelectetStar].Tracked = !simulation.Stars[simulation.SelectetStar].Tracked;
                    break;
                case Keys.OemPeriod:
                    if (simulation.SimSpeed < 512) simulation.SimSpeed *= 2;
                    break;
                case Keys.Oemcomma:
                    if (simulation.SimSpeed > 1) simulation.SimSpeed /= 2;
                    break;
            }
            
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
        }

        private void searchStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchStarDialog = new SearchStarDialog();
            searchStarDialog.Show(this,simulation.Stars);
        }

        private void showMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showMarker = ((ToolStripMenuItem)sender).Checked;
        }
        private void showStarInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showStarInfo = ((ToolStripMenuItem)sender).Checked;
        }
        private void showSimInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showSimInfo = ((ToolStripMenuItem)sender).Checked;
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
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
            }
        }
    }
}

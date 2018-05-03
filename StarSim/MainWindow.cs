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

namespace StarSim
{
    public unsafe partial class MainWindow : Form
    {
        Star[] starArray = new Star[1000];
        int starArrayLenght = 0;

        int curStar = -1;
        int focusStar = -1;
        int refStar = -1;

        float massCenterX = 0, massCenterY = 0;
        float speedCenterX = 0, speedCenterY = 0;
        int starsNumber = 0;

        int timeScale = 1;
        bool run = false;

        Task mainLogikTask;
        Task[] logikTasks;

        float[] camPos = new float[2];
        float scaling = 1;

        bool showMarker = true;
        bool showStarInfo = true;
        bool showSimInfo = true;

        Point lastMousePos = new Point(0,0);

        int usedTime;

        public MainWindow()
        {
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            InitializeComponent();
            MouseWheel += new MouseEventHandler(Window_MouseWheel);

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer,true);

            DoubleBuffered = true;
            TimerLogik.Enabled = true;
            TimerDraw.Enabled = true;
            //Feld.Refresh();

        }
        ~MainWindow()
        {
            //Application.Exit();
        }

        private void timerLogik_Tick(object sender, EventArgs e)
        {
            if (mainLogikTask == null || mainLogikTask.IsCompleted)
            {
                mainLogikTask = new Task(() => Simulate());
                mainLogikTask.Start();
            }
            //GameRun();
            float[] power = new float[2];
        }

        public void TimeRun(bool run)
        {
            this.run = run;
            if (run == true)
            {
                TimerLogik.Start();
            }
            else
            {
                TimerLogik.Stop();
            }
        }

        private void Simulate()
        {

            Stopwatch SWTotal = new Stopwatch();
            SWTotal.Start();

            for (int j = 0; j < timeScale; j++)
            {
                int newEnabeldStarNumber = 0;

                int tasks = 8;
                int step = starArrayLenght / tasks;


                logikTasks = new Task[tasks];
                for (int iT = 0; iT < tasks; iT++)
                {
                    int index = iT; logikTasks[index] = new Task(() => SimulateSelective(step * index, step * (index + 1)));
                }
                for (int iT = 0; iT < tasks; iT++)
                {
                    int index = iT; logikTasks[index].Start();
                }

                for (int iT = 0; iT < tasks; iT++)
                {
                    int index = iT; logikTasks[index].Wait();
                }

                float newMassCenterX = 0,newMassCenterY = 0;
                float newSpeedCenterX = 0, newSpeedCenterY = 0;
                float totalmass = 0;
                for (int iS1 = 0; iS1 < starArrayLenght; iS1++)
                {
                    if (starArray[iS1].Enabled == false) continue;
                    else if (starArray[iS1].Kill == true) starArray[iS1].Enabled = false;
                    else
                    {
                        newEnabeldStarNumber++;
                        starArray[iS1].UpdateMass();

                        
                        newMassCenterX += starArray[iS1].Pos[0] * starArray[iS1].AbsMass;
                        newMassCenterY += starArray[iS1].Pos[1] * starArray[iS1].AbsMass;
                        newSpeedCenterX += starArray[iS1].Speed[0] * starArray[iS1].AbsMass;
                        newSpeedCenterY += starArray[iS1].Speed[1] * starArray[iS1].AbsMass;
                        
                        totalmass += starArray[iS1].AbsMass;
                        //fix speed;
                        /*
                        float goalSpeed = 20;
                        float fullSpeed = Math.Abs(starArray[iS1].Speed[0]) + Math.Abs(starArray[iS1].Speed[1]);
                        float factor = (goalSpeed / fullSpeed);
                        starArray[iS1].Speed[0] *= factor;
                        starArray[iS1].Speed[1] *= factor;
                        */
                        starArray[iS1].Pos[0] += starArray[iS1].Speed[0] *= 1.000f;
                        starArray[iS1].Pos[1] += starArray[iS1].Speed[1] *= 1.000f;
                    }
                }
                massCenterX = newMassCenterX / totalmass; massCenterY = newMassCenterY / totalmass;
                speedCenterX = newSpeedCenterX / totalmass; speedCenterY = newSpeedCenterY / totalmass;

                starsNumber = newEnabeldStarNumber;

                if (focusStar != -1 && starArray[focusStar].Enabled == true)
                {
                    camPos[0] -= starArray[focusStar].Speed[0];
                    camPos[1] -= starArray[focusStar].Speed[1];
                }
                else
                {
                    camPos[0] -= speedCenterX;
                    camPos[1] -= speedCenterY;
                }
                if (refStar != -1 && starArray[refStar].Enabled == true)
                {
                }

                if (!run) break;
            }

            SWTotal.Stop();
            usedTime = (int)SWTotal.ElapsedMilliseconds;
        }
        private void SimulateSelective(int start, int end)
        {
            for (int iS1 = start; iS1 < end; iS1++)
            {
                if (starArray[iS1].Enabled == true)
                {
                    //--------------------------------------------------------------Atom_Kolision
                    for (int iS2 = iS1; iS2 < starArrayLenght; iS2++)
                    {
                        if (starArray[iS2].Enabled == true && iS1 != iS2)
                        { //Vergleicher jedes object mit jedem anderen
                            float distX = (starArray[iS1].Pos[0] + 0) - (starArray[iS2].Pos[0] + 0);
                            float distY = (starArray[iS1].Pos[1] + 0) - (starArray[iS2].Pos[1] + 0);
                            float dist = (float)Math.Sqrt((distX * distX) + (distY * distY));

                            float relativDistX = (distX > 0) ? distX : -distX;
                            float relativDistY = (distY > 0) ? distY : -distY;

                            float pX = relativDistX / (relativDistX + relativDistY);
                            float pY = relativDistY / (relativDistX + relativDistY);

                            float massPS1 = (float)starArray[iS1].AbsMass / (starArray[iS1].AbsMass + starArray[iS2].AbsMass);
                            float massPS2 = (float)starArray[iS2].AbsMass / (starArray[iS1].AbsMass + starArray[iS2].AbsMass);
                            float Fg = ((float)(starArray[iS1].Mass) * (starArray[iS2].Mass) / dist);


                            

                            //AtomArray[ii].speed[1] += pY * Fg /1000;
                            float kolision = 0;
                            if (dist < (starArray[iS1].SizeR) + (starArray[iS2].SizeR))
                            {
                                starArray[iS2].Kill = true;
                                if (iS2 == curStar) curStar = iS1;
                                if (iS2 == focusStar) focusStar = iS1;
                                if (iS2 == refStar) refStar = iS1;
                                starArray[iS1].NewMass = starArray[iS1].Mass + starArray[iS2].Mass;
                                if (starArray[iS1].NewMass == 0) starArray[iS1].Kill = true;
                                starArray[iS1].Pos[0] = (starArray[iS1].Pos[0] * massPS1 + starArray[iS2].Pos[0] * massPS2);
                                starArray[iS1].Pos[1] = (starArray[iS1].Pos[1] * massPS1 + starArray[iS2].Pos[1] * massPS2);

                                starArray[iS1].Speed[0] = (starArray[iS1].Speed[0] * massPS1 + starArray[iS2].Speed[0] * massPS2);
                                starArray[iS1].Speed[1] = (starArray[iS1].Speed[1] * massPS1 + starArray[iS2].Speed[1] * massPS2);

                                kolision = 0;
                            }
                            else
                            {
                                kolision = 1;
                            }

                            const int time = (int)100;
                            //Grafitation
                            if (starArray[iS1].Pos[0] > starArray[iS2].Pos[0])
                            {
                                starArray[iS1].Speed[0] -= (pX * Fg / time) * massPS2 * kolision;
                                starArray[iS2].Speed[0] += (pX * Fg / time) * massPS1 * kolision;
                            }
                            else if (starArray[iS1].Pos[0] < starArray[iS2].Pos[0])
                            {
                                starArray[iS1].Speed[0] += (pX * Fg / time) * massPS2 * kolision;
                                starArray[iS2].Speed[0] -= (pX * Fg / time) * massPS1 * kolision;
                            }

                            if (starArray[iS1].Pos[1] > starArray[iS2].Pos[1])
                            {
                                starArray[iS1].Speed[1] -= (pY * Fg / time) * massPS2 * kolision;
                                starArray[iS2].Speed[1] += (pY * Fg / time) * massPS1 * kolision;
                            }
                            else if (starArray[iS1].Pos[1] < starArray[iS2].Pos[1])
                            {
                                starArray[iS1].Speed[1] += (pY * Fg / time) * massPS2 * kolision;
                                starArray[iS2].Speed[1] -= (pY * Fg / time) * massPS1 * kolision;
                            }




                        }
                    }  //Vergleicher jedes object mit jedem anderen
                    //--------------------------------------------------------------End_Atom_Kolision

                }
            }
            return;
        }
        public void Init(int mode, int size, int stars, float minMass, float maxMass, float minSpeed, float maxSpeed)
        {
            mainLogikTask.Wait();
            for (int i = 0; i < logikTasks.Length; i++) logikTasks[i].Wait();
            try
            {
                camPos[0] = camPos[1] = 0;
                starArrayLenght = stars;
                starArray = new Star[stars];
                Random rnd = new Random(); // initialisiert die Zufallsklasse

                if (mode == 0)
                {
                    for (int ii = 0; ii < starArrayLenght; ii++)
                    {
                        starArray[ii].Init(ii, (float)((maxMass - minMass) * rnd.NextDouble() + minMass)//rnd.NextDouble() > 0.5f ? size : -size

                            , (float)(size * rnd.NextDouble() - size / 2)
                            , (float)(size * rnd.NextDouble() - size / 2)

                            , (float)((maxSpeed- minSpeed) * rnd.NextDouble() + minSpeed)
                            , (float)((maxSpeed - minSpeed) * rnd.NextDouble() + minSpeed)
                            );
                    }
                }

                TimeRun(true);
                //button1.BackColor = Color.White;
            }
            catch
            {
                TimerLogik.Enabled = false;
                //button1.BackColor = Color.Red;
            }
        }


        private void TimerDraw_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
            //Refresh();
        }
        private float[] transformPoint(float x,float y)
        {
            return new float[] {(x + camPos[0]) * scaling + Width / 2f, (y + camPos[1]) * scaling + Height / 2f};
        }
        private void this_Paint(object sender, PaintEventArgs e)
        {
            Stopwatch SWTotal = new Stopwatch();
            SWTotal.Start();
            Graphics g = e.Graphics;

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
            if (showMarker)
            {
                float[] centerPos = transformPoint(massCenterX, massCenterY);

                g.DrawEllipse(backPen, centerPos[0] - 20f, centerPos[1] - 20f, 40, 40);

                /*
                g.DrawLine(backPen, 0, Height / 2, Width, Height / 2);
                g.DrawLine(backPen, Width/2, 0 , Width / 2, Height);
                */
                float[] refPos = centerPos;
                if (refStar != -1)
                {
                    refPos = transformPoint(starArray[refStar].Pos[0], starArray[refStar].Pos[1]);
                    g.DrawEllipse(backPen, refPos[0] - 10, refPos[1] - 10, 20, 20);
                    g.DrawLine(backPen, centerPos[0], centerPos[1], refPos[0], refPos[1]);
                }
                float[] focusPos = centerPos;
                if (focusStar != -1)
                {
                    focusPos = transformPoint(starArray[focusStar].Pos[0], starArray[focusStar].Pos[1]);
                }
                int dist1 = 10, dist2 = 40;
                g.DrawLine(backPen, focusPos[0] + dist1, focusPos[1] + dist1, focusPos[0] + dist2, focusPos[1] + dist2);
                g.DrawLine(backPen, focusPos[0] - dist1, focusPos[1] + dist1, focusPos[0] - dist2, focusPos[1] + dist2);
                g.DrawLine(backPen, focusPos[0] + dist1, focusPos[1] - dist1, focusPos[0] + dist2, focusPos[1] - dist2);
                g.DrawLine(backPen, focusPos[0] - dist1, focusPos[1] - dist1, focusPos[0] - dist2, focusPos[1] - dist2);

                if (curStar != -1)
                {
                    float[] curPos = transformPoint(starArray[curStar].Pos[0], starArray[curStar].Pos[1]);
                    g.DrawLine(backPen2, curPos[0], curPos[1], refPos[0], refPos[1]);
                }
            }


            //try
            //{
            SolidBrush brush = new SolidBrush(Color.LightGray);

            for (int ii = 0; ii < starArrayLenght; ii++)
            {
                if (!starArray[ii].Enabled) continue;

                float r = starArray[ii].SizeR;

                byte RColor = (byte)starArray[ii].Mass;
                float posX = starArray[ii].Pos[0] - r + camPos[0];
                float posY = starArray[ii].Pos[1] - r + camPos[1];

                posX *= scaling; posY *= scaling; r *= scaling;
                posX += Width / 2f; posY += Height / 2f;

                if (r < 0.01) r = 0.01f;
                if (ii == curStar)
                {
                    if (starArray[ii].Mass > 0) g.DrawEllipse(new Pen(Color.FromArgb(170, 255, 255), 1), posX, posY, r * 2, r * 2);
                    else g.DrawEllipse(new Pen(Color.FromArgb(255, 170, 255), 1), posX, posY, r * 2, r * 2);

                    if (showStarInfo)
                    {
                        posX += r;
                        posY += r;
                        g.DrawLine(guiLine, new PointF(posX, posY), new PointF(posX + r * 1.5f, posY - r * 1.5f));
                        g.DrawLine(guiLine, new PointF(posX + r * 1.5f, posY - r * 1.5f), new PointF(posX + r * 1.5f + r * 2f + 4f, posY - r * 1.5f));
                        int txtPosX = (int)(posX + r * 1.5f + r * 2f + 9f);
                        int txtPosY = (int)(posY - r * 1.5f - 6f);
                        g.DrawString("Name: "+((starArray[ii].Name.Length > 0) ? starArray[ii].Name : String.Format("{0:X}", starArray[ii].ID)), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY));
                        g.DrawString("Mass: " + Math.Round(starArray[ii].Mass,2), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                        //g.DrawString("Speed: " + (Math.Abs(starArray[ii].Speed[0]) + Math.Abs(starArray[ii].Speed[1])), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                    }
                }
                else
                {
                    if (starArray[ii].Mass > 0) g.DrawEllipse(new Pen(Color.FromArgb(100, 200, 255), 1), posX, posY, r * 2, r * 2);
                    else g.DrawEllipse(new Pen(Color.FromArgb(200, 100, 255), 1), posX, posY, r * 2, r * 2);
                }
            }

            if (showSimInfo)
            {
                g.DrawString("Stars " + starsNumber, new Font("Consolas", 9), brush, new Point(10, 40));

                g.DrawString("SimSpeed " + timeScale, new Font("Consolas", 9), brush, new Point(10, 60));
                /*
                g.DrawString("L_Time " + usedTime + "ms", new Font("Consolas", 9), brush, new Point(10, 50));
                g.DrawString("R_Time " + SWTotal.ElapsedMilliseconds + "ms", new Font("Consolas", 9), brush, new Point(10, 60));
                */
            }
            SWTotal.Stop();
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            this.Focus();
            if (e.Button == MouseButtons.Left)
            {
                camPos[0] += ((e.X - lastMousePos.X)/scaling);
                camPos[1] += ((e.Y - lastMousePos.Y) / scaling);
            }
            lastMousePos.X = e.X;
            lastMousePos.Y = e.Y;
        }
        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
            float posX = -camPos[0] + (e.X - Width / 2) / scaling;
            float posY = -camPos[1] + (e.Y - Height / 2) / scaling;

            scaling += (e.Delta / 500f) * scaling;
            if (scaling < 0.00001) scaling = 0.00001f;
            else if (scaling > 10) scaling = 10;

            camPos[0] = -posX + (Width / 2 * (e.X / (float)Width * 2 - 1)) / scaling;
            camPos[1] = -posY + (Height / 2 * (e.Y / (float)Height * 2 - 1)) / scaling;
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
            float posX = -camPos[0] + (e.X - Width / 2)/ scaling;
            float posY = -camPos[1] + (e.Y - Height / 2)/scaling;
            int nearestStar = -1;

            bool firstStar = true;
            float maxdist = 0;

            for (int iS = 0; iS < starArrayLenght; iS++)
            {
                if (starArray[iS].Enabled == true)
                { //Vergleicher jedes object mit jedem anderen
                    float distX = posX - (starArray[iS].Pos[0] + 0);
                    float distY = posY - (starArray[iS].Pos[1] + 0);
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
            curStar = nearestStar;
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            mainLogikTask.Wait();
            for (int i = 0; i < logikTasks.Length; i++) logikTasks[i].Wait();

            ByteStream bs = new ByteStream(openFileDialog.FileName);
            bs.ResetIndex();

            bs.ReadByte();
            starArrayLenght = bs.ReadInt();
            camPos[0] = bs.ReadFloat();
            camPos[1] = bs.ReadFloat();
            scaling = bs.ReadFloat();
            starArray = new Star[starArrayLenght];
            for (int i = 0; i < starArrayLenght; i++)
            {
                starArray[i] = new Star();
                starArray[i].Init(bs.ReadInt(),bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat());
                bs.ReadString();
            }
            curStar = bs.ReadInt();
            focusStar = bs.ReadInt();
            refStar = bs.ReadInt();

            //TimerLogik.Start();
        }
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            mainLogikTask.Wait();
            for (int i = 0; i < logikTasks.Length; i++) logikTasks[i].Wait();

            ByteStream bs = new ByteStream();

            bs.WriteByte(0);

            int scurStar=-1, sfocusStar=-1, srefStar=-1;
            bs.WriteInt(starArrayLenght);
            bs.WriteFloat(camPos[0]);
            bs.WriteFloat(camPos[1]);
            bs.WriteFloat(scaling);
            int newMaxStars = 0;
            for (int i = 0; i < starArrayLenght; i++)
            {
                if (starArray[i].Enabled)
                {
                    bs.WriteInt(starArray[i].ID);
                    bs.WriteFloat(starArray[i].Mass);
                    bs.WriteFloat(starArray[i].Pos[0]);
                    bs.WriteFloat(starArray[i].Pos[1]);
                    bs.WriteFloat(starArray[i].Speed[0]);
                    bs.WriteFloat(starArray[i].Speed[1]);
                    bs.WriteString(starArray[i].Name);

                    if (curStar == i) scurStar = newMaxStars;
                    if (focusStar == i) sfocusStar = newMaxStars;
                    if (refStar == i) srefStar = newMaxStars;
                    newMaxStars++;
                }
            }
            bs.Index = 1;
            bs.WriteInt(newMaxStars);
            bs.Index = bs.EndIndex;

            bs.WriteInt(curStar);
            bs.WriteInt(focusStar);
            bs.WriteInt(refStar);

            bs.Save(saveFileDialog.FileName);

            TimerLogik.Start();
            TimeRun(true);
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
                    if (TimerLogik.Enabled) TimeRun(false);
                    else TimeRun(true);
                    break;
                case Keys.F:
                    if (focusStar == curStar) focusStar = -1;
                    else focusStar = curStar;
                    break;
                case Keys.R:
                    if (refStar == curStar) refStar = -1;
                    else refStar = curStar;
                    break;
                case Keys.OemPeriod:
                    if (timeScale < 512) timeScale *= 2;
                    break;
                case Keys.Oemcomma:
                    if (timeScale > 1) timeScale /= 2;
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
            TimeRun(false);
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

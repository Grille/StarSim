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


namespace AtomSim
{

    public struct Star
    {
        
        public bool Kill;
        public bool Enabled;
        public int UID;
        public float Mass;
        public float AbsMass;
        public float NewMass;
        public float SizeR;
        public float[] Pos;
        public float[] NewPos;
        public float[] Speed;
        public float[] NewSpeed;
        //public int Richtung; //ghkads

        public void Init(float mass, float posX, float posY, float speedX, float speedY)
        {
            Enabled = true;
            UpdateMass(mass);
            Pos = new float[2] { posX, posY };
            Speed = new float[2] { speedX, speedY };
            NewMass = -1;
        }
        public void UpdateMass()
        {
            if (NewMass == -1) return;
            UpdateMass(NewMass);
            NewMass = -1;
        }
        public void UpdateMass(float mass)
        {
            Mass = mass;
            AbsMass = Math.Abs(mass);
            SizeR = (float)(Math.Sqrt(Math.Abs(mass)) / Math.PI) * 10;
        }
    }

    public unsafe partial class MainWindow : Form
    {
        int MaxStars = 0;

        int RPF;
        Star[] starArray = new Star[1000];
        float[] power = new float[2];
        int[] WeltGroese = new int[4];
        float[] camPos = new float[2];
        int[] TxtBoxData = new int[2];

        int CurStar = -1;
        int FocusStar = -1;
        int RefStar = -1;
        int time = 1;
        bool run = false;

        float massCenterX = 0, massCenterY = 0;
        int enabeldStarNumber = 0;
        int finalEnabeldStarNumber = 0;
        float scaling = 1;

        Task logikTask;
        Task[] logikTasks;

        Point lastMousePos = new Point(0,0);

        int usedTime;

        public MainWindow()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(Window_MouseWheel);

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer,true);

            DoubleBuffered = true;
            TimerLogik.Enabled = true;
            TimerDraw.Enabled = true;
            WeltGroese[0] = -20000;
            WeltGroese[1] = -20000;
            WeltGroese[2] = 20000;
            WeltGroese[3] = 20000;

            //Feld.Refresh();
            
        }
        ~MainWindow()
        {
            //Application.Exit();
        }

        private void timerLogik_Tick(object sender, EventArgs e)
        {
            if (logikTask == null || logikTask.IsCompleted)
            {
                logikTask = new Task(() => Simulate());
                logikTask.Start();
            }
            //GameRun();
            float[] power = new float[2];
        }

        private void Simulate()
        {

            Stopwatch SWTotal = new Stopwatch();
            SWTotal.Start();

            for (int j = 0; j < time; j++)
            {
                int newEnabeldStarNumber = 0;

                int tasks = 8;
                int step = MaxStars / tasks;


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
                float totalmass = 0;
                for (int iS1 = 0; iS1 < MaxStars; iS1++)
                {
                    if (starArray[iS1].Enabled == false) continue;
                    else if (starArray[iS1].Kill == true) starArray[iS1].Enabled = false;
                    else
                    {
                        newEnabeldStarNumber++;
                        starArray[iS1].UpdateMass();

                        newMassCenterX += starArray[iS1].Pos[0] * starArray[iS1].AbsMass;
                        newMassCenterY += starArray[iS1].Pos[1] * starArray[iS1].AbsMass;
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

                enabeldStarNumber = newEnabeldStarNumber;

                if (FocusStar != -1 && starArray[FocusStar].Enabled == true)
                {
                    camPos[0] -= starArray[FocusStar].Speed[0];
                    camPos[1] -= starArray[FocusStar].Speed[1];
                }
                if (RefStar != -1 && starArray[RefStar].Enabled == true)
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
                    for (int iS2 = iS1; iS2 < MaxStars; iS2++)
                    {
                        if (starArray[iS2].Enabled == true && iS1 != iS2)
                        { //Vergleicher jedes object mit jedem anderen
                            RPF += 1;




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
                                if (iS2 == CurStar) CurStar = iS1;
                                if (iS2 == FocusStar) FocusStar = iS1;
                                if (iS2 == RefStar) RefStar = iS1;
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
            float[] centerPos = transformPoint(massCenterX,massCenterY);

            g.DrawEllipse(backPen, centerPos[0] - 20f, centerPos[1] - 20f, 40, 40);

            /*
            g.DrawLine(backPen, 0, Height / 2, Width, Height / 2);
            g.DrawLine(backPen, Width/2, 0 , Width / 2, Height);
            */
            float[] refPos = centerPos;
            if (RefStar != -1)
            {
                refPos = transformPoint(starArray[RefStar].Pos[0], starArray[RefStar].Pos[1]);
                g.DrawEllipse(backPen, refPos[0] - 10, refPos[1] - 10, 20, 20);
                g.DrawLine(backPen, centerPos[0], centerPos[1], refPos[0], refPos[1]);
            }
            float[] focusPos = centerPos;
            if (FocusStar != -1)
            {
                focusPos = transformPoint(starArray[FocusStar].Pos[0], starArray[FocusStar].Pos[1]);
            }
            int dist1 = 10, dist2 = 40;
            g.DrawLine(backPen, focusPos[0] + dist1, focusPos[1] + dist1, focusPos[0] + dist2, focusPos[1] + dist2);
            g.DrawLine(backPen, focusPos[0] - dist1, focusPos[1] + dist1, focusPos[0] - dist2, focusPos[1] + dist2);
            g.DrawLine(backPen, focusPos[0] + dist1, focusPos[1] - dist1, focusPos[0] + dist2, focusPos[1] - dist2);
            g.DrawLine(backPen, focusPos[0] - dist1, focusPos[1] - dist1, focusPos[0] - dist2, focusPos[1] - dist2);

            if (CurStar != -1)
            {
                float[] curPos = transformPoint(starArray[CurStar].Pos[0], starArray[CurStar].Pos[1]);
                g.DrawLine(backPen2, curPos[0], curPos[1], refPos[0], refPos[1]);
            }


            //try
            //{
            SolidBrush brush = new SolidBrush(Color.LightGray);

                for (int ii = 0; ii < MaxStars; ii++)
                {

                    if (starArray[ii].Enabled == true)
                {
                    float r = starArray[ii].SizeR;

                    byte RColor = (byte)starArray[ii].Mass;
                    float posX = starArray[ii].Pos[0] - r + camPos[0];
                    float posY = starArray[ii].Pos[1] - r + camPos[1];

                    posX *= scaling; posY *= scaling; r *= scaling;
                    posX += Width / 2f; posY += Height / 2f;

                    //posX = (int)posX; posY = (int)posY;
                    if (r < 0.01) r = 0.01f;
                    if (ii == CurStar)
                    {
                        //Pen pen = new Pen(Color.FromArgb(170, 255, 255), 1);
                        //g.DrawEllipse(new Pen(Color.FromArgb(170, 255, 255), 1), posX, posY, r * 2, r * 2);

                        if (starArray[ii].Mass > 0) g.DrawEllipse(new Pen(Color.FromArgb(170, 255, 255), 1), posX, posY, r * 2, r * 2);
                        else g.DrawEllipse(new Pen(Color.FromArgb(255, 170, 255), 1), posX, posY, r * 2, r * 2);

                        posX += r;
                        posY += r;
                        g.DrawLine(guiLine, new PointF(posX, posY), new PointF(posX + r * 1.5f, posY - r * 1.5f));
                        g.DrawLine(guiLine, new PointF(posX + r * 1.5f, posY - r * 1.5f), new PointF(posX + r * 1.5f + r * 2f + 4f, posY - r * 1.5f));
                        int txtPosX = (int)(posX + r * 1.5f + r * 2f + 9f);
                        int txtPosY = (int)(posY - r * 1.5f - 6f);
                        g.DrawString("S" + ii + "M" + starArray[ii].Mass, new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY));
                        g.DrawString("Mass:" + starArray[ii].Mass, new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                        g.DrawString("Speed:" + (Math.Abs(starArray[ii].Speed[0])+ Math.Abs(starArray[ii].Speed[1])), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                    }
                    else
                    {
                        if (starArray[ii].Mass > 0) g.DrawEllipse(new Pen(Color.FromArgb(100, 200, 255), 1), posX, posY, r * 2, r * 2);
                        else g.DrawEllipse(new Pen(Color.FromArgb(200, 100, 255), 1), posX, posY, r * 2, r * 2);
                    }


                }

            }

                g.DrawString("Stars " + enabeldStarNumber, new Font("Consolas", 9), brush, new Point(10, 10));
                g.DrawString("Ops " + enabeldStarNumber * enabeldStarNumber, new Font("Consolas", 9), brush, new Point(10, 20));

                g.DrawString("L_Time " + usedTime + "ms", new Font("Consolas", 9), brush, new Point(10, 40));
                g.DrawString("R_Time " + SWTotal.ElapsedMilliseconds + "ms", new Font("Consolas", 9), brush, new Point(10, 50));
                SWTotal.Stop();

            //}
            //catch (Exception er)
            //{
            //    g.FillRectangle(Brushes.Black, new RectangleF(0, 0, 400, 40));
            //    g.DrawString("Error "+ er.ToString(), new Font("Consolas", 9), System.Drawing.Brushes.Red, new Point(10, 10));
            //    TimerLogik.Stop();
            //    TimerDraw.Stop();
            //}
        }

        private void textBoxNumber_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBoxNumber.Text) > 1000)
                {
                    textBoxNumber.ForeColor = Color.Red;
                }
                else
                {
                    textBoxNumber.ForeColor = Color.Lime;
                }
            }
            catch
            {
                textBoxNumber.ForeColor = Color.Red;
            }
        }
        private void textBoxMass_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBoxMass.Text) > 100)
                {
                    textBoxMass.ForeColor = Color.Red;
                }
                else if (Convert.ToInt32(textBoxMass.Text) < 1)
                {
                    textBoxMass.ForeColor = Color.Red;
                }
                else
                {
                    textBoxMass.ForeColor = Color.Lime;
                }
            }
            catch
            {
                textBoxMass.ForeColor = Color.Red;
            }
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
            if (e.Button != MouseButtons.Middle) return;
            selectStar(e);
        }
        private void selectStar(MouseEventArgs e)
        {
            float posX = -camPos[0] + (e.X - Width / 2)/ scaling;
            float posY = -camPos[1] + (e.Y - Height / 2)/scaling;
            int nearestStar = -1;

            bool firstStar = true;
            float maxdist = 0;

            for (int iS = 0; iS < MaxStars; iS++)
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
            CurStar = nearestStar;
        }

        private void clearFocus()
        {
            label1.Focus();
        }
        private void buttonStart_Click(object sender, EventArgs e)
        {
            logikTask.Wait();
            for (int i = 0; i < logikTasks.Length; i++) logikTasks[i].Wait();
            try
            {
                MaxStars = Convert.ToInt32(textBoxNumber.Text);
                float maxSpeed = (float)Convert.ToDouble(textBoxSpeed.Text);
                float size = (float)Convert.ToDouble(textBoxMass.Text);
                starArray = new Star[MaxStars];
                Random rnd = new Random(); // initialisiert die Zufallsklasse
                int mode = 1;

                if (mode == 0)
                {
                    for (int ii = 0; ii < MaxStars; ii++)
                    {
                        starArray[ii].Init(size//rnd.NextDouble() > 0.5f ? size : -size

                            , (float)(((WeltGroese[0] - WeltGroese[2]) * rnd.NextDouble()) + WeltGroese[2])
                            , (float)(((WeltGroese[1] - WeltGroese[3]) * rnd.NextDouble()) + WeltGroese[3])

                            , (float)(maxSpeed * rnd.NextDouble() - maxSpeed / 2)
                            , (float)(maxSpeed * rnd.NextDouble() - maxSpeed / 2)
                            );
                    }
                }
                else if (mode == 1)
                {
                    for (int ii = 0; ii < MaxStars; ii++)
                    {
                        float speedX;
                        if (ii > MaxStars / 2) speedX = (float)(((ii / (float)(MaxStars)) * maxSpeed));
                        else
                        {
                            speedX = (float)(((ii / (float)(MaxStars)) * -maxSpeed));
                            //size = -size;
                        }
                        starArray[ii].Init(size

                            , 0f
                            , (float)(ii * 100)

                            , speedX
                            , 0f
                            );
                    }
                }


                for (int ii = 0; ii < MaxStars; ii++)
                {
                    starArray[ii].UID = ii;
                }
                switchTime(true);
                //button1.BackColor = Color.White;
            }
            catch
            {
                TimerLogik.Enabled = false;
                //button1.BackColor = Color.Red;
            }
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            switchTime(false);
            saveFileDialog.DefaultExt = "sm";
            saveFileDialog.AddExtension = true;
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog.ShowDialog();
        }
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "sm";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            logikTask.Wait();
            for (int i = 0; i < logikTasks.Length; i++) logikTasks[i].Wait();

            ByteStream bs = new ByteStream(openFileDialog.FileName);
            bs.ResetIndex();
            MaxStars = bs.ReadInt();
            camPos[0] = bs.ReadFloat();
            camPos[1] = bs.ReadFloat();
            scaling = bs.ReadFloat();
            CurStar = bs.ReadInt();
            FocusStar = bs.ReadInt();
            RefStar = bs.ReadInt();
            starArray = new Star[MaxStars];
            for (int i = 0; i < MaxStars; i++)
            {
                starArray[i] = new Star();
                starArray[i].Init(bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat(), bs.ReadFloat());
            }

            //TimerLogik.Start();
        }
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            logikTask.Wait();
            for (int i = 0; i < logikTasks.Length; i++) logikTasks[i].Wait();

            ByteStream bs = new ByteStream();
            bs.WriteInt(MaxStars);
            bs.WriteFloat(camPos[0]);
            bs.WriteFloat(camPos[1]);
            bs.WriteFloat(scaling);
            bs.WriteInt(CurStar);
            bs.WriteInt(FocusStar);
            bs.WriteInt(RefStar);
            int newMaxStars = 0;
            for (int i = 0; i < MaxStars; i++)
            {
                if (starArray[i].Enabled)
                {
                    newMaxStars++;
                    bs.WriteFloat(starArray[i].Mass);
                    bs.WriteFloat(starArray[i].Pos[0]);
                    bs.WriteFloat(starArray[i].Pos[1]);
                    bs.WriteFloat(starArray[i].Speed[0]);
                    bs.WriteFloat(starArray[i].Speed[1]);
                }
            }

            bs.Index = 0;
            bs.WriteInt(newMaxStars);
            bs.Index = bs.EndIndex;

            bs.Save(saveFileDialog.FileName);

            TimerLogik.Start();
            switchTime(true);
        }

        private void switchTime(bool run)
        {
            this.run = run;
            if (run == true)
            {
                TimerLogik.Start();
                buttonStop.Text = "=";
            }
            else
            {
                TimerLogik.Stop();
                buttonStop.Text = ">";
            }
        }
        private void buttonTimeAdd_Click(object sender, EventArgs e)
        {
            time *= 2;
            if (time > 512) time = 512;
            labelTime.Text = ""+time;
            this.Select();
        }
        private void buttonTimeSub_Click(object sender, EventArgs e)
        {
            time /= 2;
            if (time < 1) time = 1;
            labelTime.Text = "" + time;
            //this.Focus();
            this.Select();
        }

        private void MainWindow_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space)
            {
                if (TimerLogik.Enabled) switchTime(false);
                else switchTime(true);
            }
            else if (e.KeyData == Keys.F)
            {
                if (FocusStar == CurStar) FocusStar = -1;
                else FocusStar = CurStar;
            }
            else if (e.KeyData == Keys.R)
            {
                if (RefStar == CurStar) RefStar = -1;
                else RefStar = CurStar;
            }
            Focus();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (TimerLogik.Enabled) switchTime(false);
            else switchTime(true);
        }

        private void imageButton1_Click(object sender, EventArgs e)
        {

        }
    }
}

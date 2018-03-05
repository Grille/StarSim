using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            SizeR = (float)(Math.Sqrt(mass) / Math.PI) * 10;
        }
    }

    public unsafe partial class MainWindow : Form
    {
        int MaxStars = 0;

        int RPF;
        Star[] starArray = new Star[1000];
        float[] power = new float[2];
        int[] WeltGroese = new int[4];
        int[] camPos = new int[2];
        int[] TxtBoxData = new int[2];


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

            DoubleBuffered = true;
            TimerLogik.Enabled = true;
            TimerDraw.Enabled = true;
            WeltGroese[0] = -2000;
            WeltGroese[1] = -2000;
            WeltGroese[2] = 2000;
            WeltGroese[3] = 2000;


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
            for (int j = 0; j < 1; j++)
            {
                enabeldStarNumber = 0;
                Stopwatch SWTotal = new Stopwatch();
                SWTotal.Start();

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

                for (int iS1 = 0; iS1 < MaxStars; iS1++)
                {
                    if (starArray[iS1].Enabled == true)
                    {
                        if (starArray[iS1].Kill == true) starArray[iS1].Enabled = false;
                        else
                        {
                            starArray[iS1].UpdateMass();
                            starArray[iS1].Pos[0] += starArray[iS1].Speed[0] *= 1.000f;
                            starArray[iS1].Pos[1] += starArray[iS1].Speed[1] *= 1.000f;
                        }
                    }
                }
                usedTime = (int)SWTotal.ElapsedMilliseconds;
                SWTotal.Stop();
                finalEnabeldStarNumber = enabeldStarNumber;
            }
        }
        private void SimulateSelective(int start, int end)
        {
            for (int iS1 = start; iS1 < end; iS1++)
            {
                if (starArray[iS1].Enabled == true)
                {
                    enabeldStarNumber++;
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

                            float massS1 = (float)starArray[iS1].Mass / (starArray[iS1].Mass + starArray[iS2].Mass);
                            float massS2 = (float)starArray[iS2].Mass / (starArray[iS1].Mass + starArray[iS2].Mass);
                            float Fg = ((float)(starArray[iS1].Mass) * (starArray[iS2].Mass) / dist);

                            //AtomArray[ii].speed[1] += pY * Fg /1000;
                            float kolision = 0;
                            if (dist < (starArray[iS1].SizeR) + (starArray[iS2].SizeR))
                            {
                                starArray[iS2].Kill = true;
                                starArray[iS1].NewMass = starArray[iS1].Mass + starArray[iS2].Mass;
                                starArray[iS1].Pos[0] = (starArray[iS1].Pos[0] * massS1 + starArray[iS2].Pos[0] * massS2);
                                starArray[iS1].Pos[1] = (starArray[iS1].Pos[1] * massS1 + starArray[iS2].Pos[1] * massS2);

                                starArray[iS1].Speed[0] = (starArray[iS1].Speed[0] * massS1 + starArray[iS2].Speed[0] * massS2);
                                starArray[iS1].Speed[1] = (starArray[iS1].Speed[1] * massS1 + starArray[iS2].Speed[1] * massS2);

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
                                starArray[iS1].Speed[0] -= (pX * Fg / time) * massS2 * kolision;
                                starArray[iS2].Speed[0] += (pX * Fg / time) * massS1 * kolision;
                            }
                            else if (starArray[iS1].Pos[0] < starArray[iS2].Pos[0])
                            {
                                starArray[iS1].Speed[0] += (pX * Fg / time) * massS2 * kolision;
                                starArray[iS2].Speed[0] -= (pX * Fg / time) * massS1 * kolision;
                            }

                            if (starArray[iS1].Pos[1] > starArray[iS2].Pos[1])
                            {
                                starArray[iS1].Speed[1] -= (pY * Fg / time) * massS2 * kolision;
                                starArray[iS2].Speed[1] += (pY * Fg / time) * massS1 * kolision;
                            }
                            else if (starArray[iS1].Pos[1] < starArray[iS2].Pos[1])
                            {
                                starArray[iS1].Speed[1] += (pY * Fg / time) * massS2 * kolision;
                                starArray[iS2].Speed[1] -= (pY * Fg / time) * massS1 * kolision;
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
            Refresh();
        }
        private void this_Paint(object sender, PaintEventArgs e)
        {
            Stopwatch SWTotal = new Stopwatch();
            SWTotal.Start();
            Graphics g = e.Graphics;

            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //g.TranslateTransform(Width / 2, Height / 2);
            //g.ScaleTransform(scaling, scaling);
            g.DrawLine(new Pen(Color.FromArgb(20, 30, 40),1), 0, Height / 2, Width, Height / 2);
            g.DrawLine(new Pen(Color.FromArgb(20, 30, 40), 1), Width/2, 0 , Width / 2, Height);

            try
            {
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

                        g.DrawEllipse(new Pen(Color.FromArgb(255, 100, 200, 255), 1), posX, posY, r * 2, r * 2);
                    }

                }

                g.ResetTransform();

                SolidBrush brush = new SolidBrush(Color.LightGray);
                g.DrawString("Stars " + finalEnabeldStarNumber, new Font("Consolas", 9), brush, new Point(10, 10));
                g.DrawString("Ops " + finalEnabeldStarNumber * finalEnabeldStarNumber, new Font("Consolas", 9), brush, new Point(10, 20));

                g.DrawString("L_Time " + usedTime + "ms", new Font("Consolas", 9), brush, new Point(10, 40));
                g.DrawString("R_Time " + SWTotal.ElapsedMilliseconds + "ms", new Font("Consolas", 9), brush, new Point(10, 50));
                SWTotal.Stop();

            }
            catch (Exception er)
            {
                g.FillRectangle(Brushes.Black, new RectangleF(0, 0, 400, 40));
                g.DrawString("Error "+ er.ToString(), new Font("Consolas", 9), System.Drawing.Brushes.Red, new Point(10, 10));
            }
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
            if (e.Button == MouseButtons.Left)
            {
                camPos[0] += (int)((e.X - lastMousePos.X)/scaling);
                camPos[1] += (int)((e.Y - lastMousePos.Y) / scaling);
                Refresh();
            }
            lastMousePos.X = e.X;
            lastMousePos.Y = e.Y;
        }
        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
            scaling += (e.Delta / 500f) * scaling;
            if (scaling < 0.01) scaling = 0.01f;
            else if (scaling > 10) scaling = 10;
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
                Random Rnd = new Random(); // initialisiert die Zufallsklasse
                int mode = 1;

                if (mode == 0)
                {
                    for (int ii = 0; ii < MaxStars; ii++)
                    {
                        starArray[ii].Init(size

                            , (float)(((WeltGroese[0] - WeltGroese[2]) * Rnd.NextDouble()) + WeltGroese[2])
                            , (float)(((WeltGroese[1] - WeltGroese[3]) * Rnd.NextDouble()) + WeltGroese[3])

                            , (float)(maxSpeed * Rnd.NextDouble() - maxSpeed / 2)
                            , (float)(maxSpeed * Rnd.NextDouble() - maxSpeed / 2)
                            );
                    }
                }
                else if (mode == 1)
                {
                    for (int ii = 0; ii < MaxStars; ii++)
                    {
                        float speedX;
                        if (ii > MaxStars / 2) speedX = (float)(((ii / (float)(MaxStars)) * maxSpeed));
                        else speedX = (float)(((ii / (float)(MaxStars)) * -maxSpeed));
                        starArray[ii].Init(size

                            , 0f
                            , (float)(ii * 10)

                            , speedX
                            , 0f
                            );
                    }
                }


                for (int ii = 0; ii < MaxStars; ii++)
                {
                    starArray[ii].UID = ii;
                }

                TimerLogik.Enabled = true;
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
            Application.Exit();
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            TimerLogik.Stop();
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
            camPos[0] = bs.ReadInt();
            camPos[1] = bs.ReadInt();
            scaling = bs.ReadFloat();
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
            bs.WriteInt(camPos[0]);
            bs.WriteInt(camPos[1]);
            bs.WriteFloat(scaling);
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
        }
    }
}

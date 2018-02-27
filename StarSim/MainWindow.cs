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


namespace AtomSim
{

    public unsafe partial class Form1 : Form
    {
        int MaxStars = 0;

        struct Atom
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
                SizeR = (float)(Math.Sqrt(mass)/Math.PI)*10;
            }
        }
        int RPF;
        Atom[] starArray = new Atom[1000];
        float[] power = new float[2];
        int[] WeltGroese = new int[4];
        int[] WeltKamPos = new int[2];
        int[] TxtBoxData = new int[2];

        int enabeldStarNumber = 0;
        float scaling = 1;

        Task logikTask;
        bool logikRun = false;

        Point lastMousePos = new Point(0,0);

        int usedTime;

        public Form1()
        {
            InitializeComponent();


            TimerLogik.Enabled = true;
            TimerDraw.Enabled = true;
            WeltGroese[0] = -2000;
            WeltGroese[1] = -2000;
            WeltGroese[2] = 2000;
            WeltGroese[3] = 2000;


            //Feld.Refresh();
        }
        ~Form1()
        {
            Application.Exit();
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

        private void SimulateStars(int start,int end)
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
        private void Simulate()
        {
            logikRun = true;
            enabeldStarNumber = 0;
            Stopwatch SWTotal = new Stopwatch();
            SWTotal.Start();

            int tasks = 8;
            int step = MaxStars / tasks;
            //jj(0, MaxStars);

            Task[] renderTasks = new Task[tasks];
            for (int iT = 0; iT < tasks; iT++)
            {
                int index = iT;
                renderTasks[index] = new Task(() => SimulateStars(step * index, step * (index + 1)));
                Console.WriteLine("init: " + index);
            }
            for (int iT = 0; iT < tasks; iT++)
            {
                int index = iT;
                renderTasks[index].Start();
                Console.WriteLine("start: " + index);
            }

            for (int iT = 0; iT < tasks; iT++)
            {
                int index = iT;
                renderTasks[index].Wait();
                Console.WriteLine("end: " + index);
            }

            for (int iS1 = 0; iS1 < MaxStars; iS1++)
            {
                if (starArray[iS1].Enabled == true)
                {
                    if (starArray[iS1].Kill == true) starArray[iS1].Enabled = false;
                    else
                    {
                        starArray[iS1].UpdateMass();
                        starArray[iS1].Pos[0] += starArray[iS1].Speed[0]*=1.000f;
                        starArray[iS1].Pos[1] += starArray[iS1].Speed[1] *= 1.000f;
                    }
                }
            }
            usedTime = (int)SWTotal.ElapsedMilliseconds;
            SWTotal.Stop();
            logikRun = false;
        }

        private void TimerDraw_Tick(object sender, EventArgs e)
        {
            //Task renderTask = new Task(()=>Feld.Refresh());
            //renderTask.Start();
            Feld.Refresh();
        }

        private void Feld_Paint(object sender, PaintEventArgs e)
        {
            Stopwatch SWTotal = new Stopwatch();
            SWTotal.Start();
            Graphics g = e.Graphics;
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            //g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            g.TranslateTransform(Feld.Width / 2, Feld.Height / 2);
            g.ScaleTransform(scaling, scaling);
            try
            {
                g.DrawRectangle(new Pen(Color.FromArgb(255, 25, 25, 50), 2)
                    , WeltGroese[0] + WeltKamPos[0], WeltGroese[1] + WeltKamPos[1]
                    , WeltGroese[2]- WeltGroese[0], WeltGroese[3]- WeltGroese[1]);
                //BildP[0, 0] = 7;
                // Create a local version of the graphics object for the PictureBox.
                //System.Drawing.RectangleF Rec;
                //gd.Size = { 5,4,4,4};
                //Point ff = new Point(6,53);

                //PointF[] array3 = { new Point(6, 0), new Point(6, 53), new Point(68, 53)};
                //g.DrawString("" + GG, new Font("Consolas", 9), System.Drawing.Brushes.Lime, new Point(10, 10));


                float[] TotalEnergie = new float[3] { 0, 0, 0 };
                for (int ii = 0; ii < MaxStars; ii++)
                {
                    //GG = true;
                    //AtomArray[0].enabled = true;
                    if (starArray[ii].Enabled == true)
                    {
                        float r = starArray[ii].SizeR;
                        if (listBox1.SelectedIndex == 0)
                        {
                            g.FillEllipse(new SolidBrush(Color.FromArgb(5, 250, 250, 255))
                            , starArray[ii].Pos[0] - (r * 15) + WeltKamPos[0], starArray[ii].Pos[1] - (r * 15) + WeltKamPos[1]
                            , (r * 15) * 2, (r * 15) * 2);

                            g.FillEllipse(new SolidBrush(Color.FromArgb(255, 250, 250, 255))
                            , starArray[ii].Pos[0] - (r) + WeltKamPos[0], starArray[ii].Pos[1] - (r) + WeltKamPos[1]
                            , (r) * 2, (r) * 2);
                        }
                        else
                        {
                            byte RColor = (byte)starArray[ii].Mass;
                            //if (RColor > 255) { RColor = 255; }

                            g.DrawEllipse(new Pen(Color.FromArgb(255, 50, 100, 255), 1)
                                , starArray[ii].Pos[0] - r + WeltKamPos[0], starArray[ii].Pos[1] - r + WeltKamPos[1]
                                , r * 2, r * 2);
                        }

                        if (starArray[ii].Speed[0] < 0)
                        {
                            TotalEnergie[0] -= starArray[ii].Speed[0];
                            TotalEnergie[1] -= starArray[ii].Speed[0];
                        }

                        else
                        {
                            TotalEnergie[0] += starArray[ii].Speed[0];
                            TotalEnergie[1] += starArray[ii].Speed[0];
                        }

                        if (starArray[ii].Speed[1] < 0)
                        {
                            TotalEnergie[0] -= starArray[ii].Speed[1];
                            TotalEnergie[2] -= starArray[ii].Speed[1];
                        }
                        else
                        {
                            TotalEnergie[0] += starArray[ii].Speed[1];
                            TotalEnergie[2] += starArray[ii].Speed[1];
                        }
                        // TotalEnergie += Convert.ToInt32(AtomArray[ii].speed[0]) + Convert.ToInt32(AtomArray[ii].speed[1]);
                    }

                }

                g.ResetTransform();

                SolidBrush brush = new SolidBrush(Color.FromArgb(0, 255, 140));
                g.DrawString("T " + TotalEnergie[0] / MaxStars, new Font("Consolas", 9), brush, new Point(10, 10));
                g.DrawString("X " + TotalEnergie[1] / MaxStars, new Font("Consolas", 9), brush, new Point(10, 20));
                g.DrawString("Y " + TotalEnergie[2] / MaxStars, new Font("Consolas", 9), brush, new Point(10, 30));
                g.DrawString("Stars " + enabeldStarNumber, new Font("Consolas", 9), brush, new Point(10, 40));
                g.DrawString("Ops " + enabeldStarNumber* enabeldStarNumber, new Font("Consolas", 9), brush, new Point(10, 50));


                //float[] dist = new float[2];
                //distX = (AtomArray[0].pos[0] + 0) - (AtomArray[1].pos[0] + 0);
                //distY = (AtomArray[0].pos[1] + 0) - (AtomArray[1].pos[1] + 0);
                //float dist2 = (float)Math.Sqrt((distX * distX) + (distY * distY));
                //float Fg = (float)((AtomArray[0].Groeße) * (AtomArray[1].Groeße) / dist2);
                //g.DrawString("Fg " + Fg, new Font("Consolas", 9), System.Drawing.Brushes.Lime, new Point(10, 60));

                g.DrawString("L_Time " + usedTime + "ms", new Font("Consolas", 9), brush, new Point(10, 60));
                g.DrawString("R_Time " + SWTotal.ElapsedMilliseconds + "ms", new Font("Consolas", 9), brush, new Point(10, 70));
                SWTotal.Stop();
                //g.DrawLine(new Pen(Color.FromArgb(255, 0, 255, 0), 1), new Point((int)AtomArray[0].pos[0], (int)AtomArray[0].pos[1]), new Point((int)AtomArray[1].pos[0], (int)AtomArray[1].pos[1]));
                //g.DrawLine(new Pen(Color.FromArgb(255, 0, 255, 0), 1), new Point((int)AtomArray[2].pos[0], (int)AtomArray[2].pos[1]), new Point((int)AtomArray[1].pos[0], (int)AtomArray[1].pos[1]));
                //g.DrawLine(new Pen(Color.FromArgb(255, 0, 255, 0), 1), new Point((int)AtomArray[0].pos[0], (int)AtomArray[0].pos[1]), new Point((int)AtomArray[2].pos[0], (int)AtomArray[2].pos[1]));


                //g.DrawArc(System.Drawing.Pens.Lime, 50, 50, 64, 32);
                //g.DrawCurve(System.Drawing.Pens.Lime, array3, 1, 3, 1);
                // g.DrawPie(System.Drawing.Pens.Lime, 5, 5, 64, 64, 0 , 200);
                //g.DrawImage(Test as Bitmap, new RectangleF(64, 64, 64, 64), new RectangleF(0, 64, 64, 64), GraphicsUnit.Pixel);
            }
            catch (Exception er)
            {
                g.FillRectangle(Brushes.Black, new RectangleF(0, 0, 400, 40));
                g.DrawString("Error "+ er.ToString(), new Font("Consolas", 9), System.Drawing.Brushes.Red, new Point(10, 10));
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                MaxStars = Convert.ToInt32(textBox1.Text);
                int maxSpeed = Convert.ToInt32(textBox3.Text);
                starArray = new Atom[MaxStars];

                //for (int ii = 0; ii < MaxAtom; ii++)
                //{
                //    AtomArray[ii].Init(Convert.ToInt32(textBox2.Text), 20 + ii * 50, 20 + ii * 10, (2 + 0.1f * ii) * 0, (2 + 1f * ii) * 0);
                //}

                Random Rnd = new Random(); // initialisiert die Zufallsklasse
                for (int ii = 0; ii < MaxStars; ii++)
                {
                    starArray[ii].Init(Convert.ToInt32(textBox2.Text)
                        //(int)((1+(5) * Rnd.NextDouble()))

                        , (float)(((WeltGroese[0] - WeltGroese[2]) * Rnd.NextDouble()) + WeltGroese[2])
                        , (float)(((WeltGroese[1] - WeltGroese[3]) * Rnd.NextDouble()) + WeltGroese[3])

                        , (float)(maxSpeed * Rnd.NextDouble() - maxSpeed/2)
                        , (float)(maxSpeed * Rnd.NextDouble() - maxSpeed/2)
                        );
                }


                for (int ii = 0; ii < MaxStars; ii++)
                {
                    starArray[ii].UID = ii;
                }



                //            AtomArray[0].Init(
                //40,
                //500,
                //500,
                //0.0f,
                //0.0f);
                //            AtomArray[1].Init(
                //    1,
                //    500,
                //    600,
                //    -2f,
                //    0);
                //            AtomArray[2].Init(
                //1,
                //500,
                //400,
                //2f,
                //0);



                //        AtomArray[1].Init(
                //            Convert.ToInt32(textBox1.Text),
                //            Convert.ToInt32(textBox3.Text),
                //            Convert.ToInt32(textBox4.Text),
                //            Convert.ToInt32(textBox2.Text),
                //            Convert.ToInt32(textBox2.Text));
                TimerLogik.Enabled = true;
                //button1.BackColor = Color.White;
            }
            catch
            {
                TimerLogik.Enabled = false;
                //button1.BackColor = Color.Red;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBox1.Text) > 1000)
                {
                    textBox1.ForeColor = Color.Red;
                }
                else
                {
                    textBox1.ForeColor = Color.Lime;
                }
            }
            catch
            {
                textBox1.ForeColor = Color.Red;
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBox2.Text) > 100)
                {
                    textBox2.ForeColor = Color.Red;
                }
                else if (Convert.ToInt32(textBox2.Text) < 1)
                {
                    textBox2.ForeColor = Color.Red;
                }
                else
                {
                    textBox2.ForeColor = Color.Lime;
                }
            }
            catch
            {
                textBox2.ForeColor = Color.Red;
            }
        }

        private void Feld_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WeltKamPos[0] += (int)((e.X - lastMousePos.X)/scaling);
                WeltKamPos[1] += (int)((e.Y - lastMousePos.Y) / scaling);
                Feld.Refresh();
            }
            else if (e.Button == MouseButtons.Middle)
            {
                scaling += (e.Y - lastMousePos.Y) / 100f;
                if (scaling < 0.01) scaling = 0.01f;
            }
            lastMousePos.X = e.X;
            lastMousePos.Y = e.Y;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }
    }
}

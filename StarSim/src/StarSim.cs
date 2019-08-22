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
using WF = System.Windows.Forms;
using GGL.IO;
using System.Globalization;
using GGL.Graphic;
//using OpenCL;
using System.IO;
using System.Reflection;

namespace StarSim
{
    public class StarSim
    {


        //string code = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("StarSim.src.Kernel.c")).ReadToEnd();
        public event EventHandler FrameCalculatet;
        private WF.Timer TimerLogik;
        private Star[] starArray;

        public bool NewFrameCalculatet {
            get
            {
                if (newFrameCalculatet) {
                    newFrameCalculatet = false;
                    return true;
                }
                else return false;

            }
        }
        private bool newFrameCalculatet = false;
        private double massCenterX = 0, massCenterY = 0;
        private double speedCenterX = 0, speedCenterY = 0;
        private float totalMass = 0;
        private int starCount = 0;

        private bool running = false;

        private Task mainLogikTask;
        private Task[] logikTasks;

        private float usedTime = 0;

        public int SimSpeed = 1;
        public Star SelectetStar = null, FocusStar = null, RefStar = null;

        public int UsedTime
        {
            get { return (int)usedTime; }
        }
        public double MassCenterX { get { return massCenterX; } }
        public double MassCenterY { get { return massCenterY; } }
        public double SpeedCenterX { get { return speedCenterX; } }
        public double SpeedCenterY { get { return speedCenterY; } }
        public Star[] Stars
        {
            get { return starArray; }
            set
            {
                starArray = value;
            }
        }
        public int StarCount
        {
            get { return starCount; }
        }
        public bool Running
        {
            get
            {
                return running;
            }
            set
            {
                if (value == true) Start();
                else Stop();
            }
        }

        public StarSim()
        { 
            
            TimerLogik = new WF.Timer();
            TimerLogik.Interval = 25;
            TimerLogik.Tick += new System.EventHandler(this.simulationTick);
            starArray = new Star[0];
            Console.WriteLine("rfghjk");

           

            /*
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] names = assembly.GetManifestResourceNames();
            foreach (var name in names) Console.WriteLine(name);
            */

            //Console.WriteLine(Assembly.GetExecutingAssembly().GetFile("Core.c").Name);
        }

        public void Wait()
        {
            mainLogikTask.Wait();
            for (int i = 0; i < logikTasks.Length; i++) logikTasks[i].Wait();
        }
        public void Start()
        {
            running = true;
            TimerLogik.Start();
        }
        public void Stop()
        {
            TimerLogik.Stop();
            running = false;
        }
        public void Init(int mode, int size, int stars, float minMass, float maxMass, float disSpeed)
        {
            Wait();
            float maxSpeed = disSpeed / 2f;
            float minSpeed = -maxSpeed;
            try
            {
                SimSpeed = 1;
                /*
                camPosX = camPosY = 0;
                scaling = Math.Min(this.Width, this.Height) / (float)(size * 1.2f);
                */
                starArray = new Star[stars];
                Random rnd = new Random(); // initialisiert die Zufallsklasse

                if (mode == 0)
                {
                    for (int ii = 0; ii < starArray.Length; ii++)
                    {
                        starArray[ii] = new Star(ii, (float)((maxMass - minMass) * rnd.NextDouble() + minMass)//rnd.NextDouble() > 0.5f ? size : -size

                            , (float)(size * rnd.NextDouble() - size / 2)
                            , (float)(size * rnd.NextDouble() - size / 2)

                            , (float)((maxSpeed - minSpeed) * rnd.NextDouble() + minSpeed)
                            , (float)((maxSpeed - minSpeed) * rnd.NextDouble() + minSpeed)
                            );
                    }
                }
                else
                {
                    for (int ii = 0; ii < starArray.Length; ii++)
                    {
                        starArray[ii] = new Star(ii, (float)((maxMass - minMass) * rnd.NextDouble() + minMass)//rnd.NextDouble() > 0.5f ? size : -size

                            , (float)(0)
                            , (float)(size * rnd.NextDouble() - size / 2)

                            , (float)((maxSpeed - minSpeed) * rnd.NextDouble() + minSpeed)
                            , (float)(0)
                            );
                    }
                }

                Start();
                //button1.BackColor = Color.White;
            }
            catch
            {
                Start();
                //button1.BackColor = Color.Red;
            }
            //CollapseStarArray();
        }
        public void AddStar(float mass, double posX, double posY, double speedX, double speedY)
        {
            collapseStarArray(starCount + 1);
            starArray[starCount] = new Star(starCount, mass, posX, posY, speedX, speedY);
            starCount++;
        }

        private void collapseStarArray(int lenght)
        {
            int iDst = 0;
            Star[] newStars = new Star[lenght];
            for (int iSrc = 0; iSrc < starArray.Length; iSrc++)
            {
                if (starArray[iSrc].Enabled)
                {
                    starArray[iSrc].Idx = iDst;
                    //starArray[iSrc].ColisionsRef = null;
                    newStars[iDst] = starArray[iSrc];
                    /*
                    if (SelectetStar == iSrc) SelectetStar = iDst;
                    if (FocusStar == iSrc) FocusStar = iDst;
                    if (RefStar == iSrc) RefStar = iDst;
                    */
                    iDst++;
                }
            }
            starArray = newStars;
        }
        public void CollapseStarArray()
        {
            collapseStarArray(starCount);
        }

        private void simulationTick(object sender, EventArgs e)
        {

            if (mainLogikTask == null || mainLogikTask.IsCompleted)
            {
                if (mainLogikTask != null && mainLogikTask.Status == TaskStatus.Faulted) Console.WriteLine("Task Crash");
                mainLogikTask = new Task(() => simulate());
                mainLogikTask.Start();
            }
            //GameRun();
            float[] power = new float[2];
        }
        private void simulate()
        {
            for (int j = 0; j < SimSpeed; j++)
            {
                Stopwatch SWTotal = new Stopwatch();
                SWTotal.Start();

                int newEnabeldStarNumber = 0;

                int tasks = 8;
                float step = starArray.Length / (float)tasks;

                Console.WriteLine();
                Stopwatch[] stopwatch = new Stopwatch[tasks];
                logikTasks = new Task[tasks];
                for (int iT = 0; iT < tasks; iT++)
                {
                    int index = iT;
                    stopwatch[index] = new Stopwatch();
                    logikTasks[index] = new Task(() => simulateSelective((int)(step * index), (int)(step * (index + 1)), stopwatch[index]));
                    logikTasks[index].Start();
                }
                for (int iT = 0; iT < tasks; iT++)
                {
                    int index = iT; logikTasks[index].Wait();
                    Console.WriteLine("" + index + " => " + stopwatch[index].ElapsedMilliseconds);
                }

                double newMassCenterX = 0, newMassCenterY = 0;
                double newSpeedCenterX = 0, newSpeedCenterY = 0;
                float newTotalMass = 0;
                for (int iS1 = 0; iS1 < starArray.Length; iS1++)
                {
                    colide(starArray[iS1]);
                }
                for (int iS1 = 0; iS1 < starArray.Length; iS1++)
                {
                    Star star1 = starArray[iS1];
                    if (star1.Enabled == false) continue;
                    else
                    {
                        newEnabeldStarNumber++;

                        newMassCenterX += star1.PosX * star1.AbsMass;
                        newMassCenterY += star1.PosY * star1.AbsMass;
                        newSpeedCenterX += star1.SpeedX * star1.AbsMass;
                        newSpeedCenterY += star1.SpeedY * star1.AbsMass;

                        newTotalMass += star1.AbsMass;
                        //fix speed;
                        /*
                        double goalSpeed = 2;
                        double fullSpeed = Math.Abs(star1.SpeedX) + Math.Abs(star1.SpeedY);
                        double factor = (goalSpeed / fullSpeed);
                        star1.SpeedX *= factor;
                        star1.SpeedY *= factor;
                        */
                        star1.PosX += star1.SpeedX *= 1.000f;
                        star1.PosY += star1.SpeedY *= 1.000f;

                    }
                }
                totalMass = newTotalMass;
                if (totalMass != 0)
                {
                    massCenterX = newMassCenterX / newTotalMass; massCenterY = newMassCenterY / newTotalMass;
                    speedCenterX = newSpeedCenterX / newTotalMass; speedCenterY = newSpeedCenterY / newTotalMass;
                }
                else
                {
                    massCenterX = newMassCenterX; massCenterY = newMassCenterY;
                    speedCenterX = newSpeedCenterX; speedCenterY = newSpeedCenterY;

                }

                //if (starsNumber != newEnabeldStarNumber)
                starCount = newEnabeldStarNumber;
                if (starArray.Length - newEnabeldStarNumber > 100) CollapseStarArray();

                FrameCalculatet?.Invoke(this, new EventArgs());

                SWTotal.Stop();
                //usedTime = usedTime * 0.9f + SWTotal.ElapsedMilliseconds * 0.1f;
                usedTime = SWTotal.ElapsedMilliseconds;

                newFrameCalculatet = true;
                if (!running) break;
            }
        }
        private void colide(Star star1)
        {
            Star star2;
            if (((star2 = star1.ColisionsRef) != null) && star2.Enabled)
            {
                star1.ColisionsRef = null;
                //iS2.ColisionsRef = null;
                /*
                Console.WriteLine("colide " + iS1 + " width " + iS2);
                Console.WriteLine(starArray[iS1].Enabled);
                Console.WriteLine(starArray[iS2].Enabled);
                */
                if (star2.ColisionsRef != null) colide(star2);

                double massPS1 = (float)star1.AbsMass / (star1.AbsMass + star2.AbsMass);
                double massPS2 = (float)star2.AbsMass / (star1.AbsMass + star2.AbsMass);

                
                if (star2 == SelectetStar) SelectetStar = star1;
                if (star2 == FocusStar) FocusStar = star1;
                if (star2 == RefStar) RefStar = star1;
                star1.Marked |= star2.Marked;


                star1.UpdateMass(star1.Mass + star2.Mass);
                star1.PosX = (star1.PosX * massPS1 + star2.PosX * massPS2);
                star1.PosY = (star1.PosY * massPS1 + star2.PosY * massPS2);
                star1.SpeedX = (star1.SpeedX * massPS1 + star2.SpeedX * massPS2);
                star1.SpeedY = (star1.SpeedY * massPS1 + star2.SpeedY * massPS2);

                star2.Enabled = false;

                if (star1.Name == "") star1.Name = star2.Name;
                else if (star2.Name != "") star1.Name = star1.Name + star2.Name;
            }
        }
        private void simulateSelective(int start, int end, Stopwatch stopwatch)
        {
            stopwatch.Start();
            for (int iS1 = start; iS1 < end; iS1++)
            {
                Star star1 = starArray[iS1];
                if (starArray[iS1].Enabled == true)
                {
                    //--------------------------------------------------------------Atom_Kolision
                    for (int iS2 = iS1; iS2 < starArray.Length; iS2++)
                    {
                        Star star2 = starArray[iS2];
                        if (starArray[iS2].Enabled == true && iS1 != iS2)
                        { //Vergleicher jedes object mit jedem anderen

                            
                            double distX = (star1.PosX) - (star2.PosX);
                            double distY = (star1.PosY) - (star2.PosY);
                            double dist = Math.Sqrt((distX * distX) + (distY * distY));
                            
                            double relativDistXY = Math.Abs(distX) + Math.Abs(distY);
                            
                            double pX = distX / relativDistXY;
                            double pY = distY / relativDistXY;

                            double Fg = ((star1.AbsMass) * (star2.AbsMass) / dist) / 100;
                            double a1 = Fg / star1.Mass;
                            double a2 = Fg / star2.Mass;
                            
                            if (dist < (star1.Radius) + (star2.Radius))
                                star1.ColisionsRef = star2;

                            
                            star1.SpeedX -= (pX * a1);
                            star1.SpeedY -= (pY * a1);
                            star2.SpeedX += (pX * a2);
                            star2.SpeedY += (pY * a2);
                            
                        }
                    }  //Vergleicher jedes object mit jedem anderen
                    //--------------------------------------------------------------End_Atom_Kolision

                }
            }
            stopwatch.Stop();
            return;
        }

    }
}

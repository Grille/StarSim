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

        void setup()
        {

            //string code = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("StarSim.src.Kernel.c")).ReadToEnd();
            //Console.WriteLine(code);

        }
        public event EventHandler FrameCalculatet;
        private WF.Timer TimerLogik;
        private Star[] starArray;

        private double massCenterX = 0, massCenterY = 0;
        private double speedCenterX = 0, speedCenterY = 0;
        private float totalMass = 0;
        private int starsNumber = 0;

        private bool running = false;

        private Task mainLogikTask;
        private Task[] logikTasks;

        private float usedTime = 0;

        public int SimSpeed = 1;
        public int SelectetStar = -1, FocusStar = -1, RefStar = -1;

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
            get { return starsNumber; }
        }
        public bool Running
        {
            get
            {
                return running;
            }
            set
            {
                this.running = value;
                if (value == true) TimerLogik.Start();
                else TimerLogik.Stop();
            }
        }

        public StarSim()
        { 
            TimerLogik = new WF.Timer();
            TimerLogik.Interval = 25;
            TimerLogik.Tick += new System.EventHandler(this.simulationTick);
            starArray = new Star[0];
            Console.WriteLine("rfghjk");


            setup();
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
            SelectetStar = -1; FocusStar = -1; RefStar = -1;
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

                Start();
                //button1.BackColor = Color.White;
            }
            catch
            {
                Start();
                //button1.BackColor = Color.Red;
            }
        }
        public void CollapseStarArray()
        {
           
            int iDst = 0;
            Star[] newStars = new Star[starsNumber];
            for (int iSrc = 0; iSrc < starArray.Length; iSrc++)
            {
                if (starArray[iSrc].Enabled)
                {
                    newStars[iDst] = starArray[iSrc];
                    if (SelectetStar == iSrc) SelectetStar = iDst;
                    if (FocusStar == iSrc) FocusStar = iDst;
                    if (RefStar == iSrc) RefStar = iDst;
                    iDst++;
                }
            }
            starArray = newStars;
        }

        private void simulationTick(object sender, EventArgs e)
        {
            if (mainLogikTask == null || mainLogikTask.IsCompleted)
            {
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

                logikTasks = new Task[tasks];
                for (int iT = 0; iT < tasks; iT++)
                {
                    int index = iT;
                    logikTasks[index] = new Task(() => simulateSelective((int)(step * index), (int)(step * (index + 1))));
                    logikTasks[index].Start();
                }
                for (int iT = 0; iT < tasks; iT++)
                {
                    int index = iT; logikTasks[index].Wait();
                }


                double newMassCenterX = 0, newMassCenterY = 0;
                double newSpeedCenterX = 0, newSpeedCenterY = 0;
                float newTotalMass = 0;
                for (int iS1 = 0; iS1 < starArray.Length; iS1++)
                {
                    colide(iS1);
                }
                for (int iS1 = 0; iS1 < starArray.Length; iS1++)
                {
                    if (starArray[iS1].Enabled == false) continue;
                    else
                    {
                        newEnabeldStarNumber++;

                        newMassCenterX += starArray[iS1].PosX * starArray[iS1].AbsMass;
                        newMassCenterY += starArray[iS1].PosY * starArray[iS1].AbsMass;
                        newSpeedCenterX += starArray[iS1].SpeedX * starArray[iS1].AbsMass;
                        newSpeedCenterY += starArray[iS1].SpeedY * starArray[iS1].AbsMass;

                        newTotalMass += starArray[iS1].AbsMass;
                        //fix speed;
                        /*
                        double goalSpeed = 2;
                        double fullSpeed = Math.Abs(starArray[iS1].SpeedX) + Math.Abs(starArray[iS1].SpeedY);
                        double factor = (goalSpeed / fullSpeed);
                        starArray[iS1].SpeedX *= factor;
                        starArray[iS1].SpeedY *= factor;
                        */
                        starArray[iS1].PosX += starArray[iS1].SpeedX *= 1.000f;
                        starArray[iS1].PosY += starArray[iS1].SpeedY *= 1.000f;

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
                starsNumber = newEnabeldStarNumber;
                if (starArray.Length - newEnabeldStarNumber > 100) CollapseStarArray();

                FrameCalculatet?.Invoke(this, new EventArgs());

                SWTotal.Stop();
                usedTime = usedTime * 0.9f + SWTotal.ElapsedMilliseconds * 0.1f;

                if (!running) break;
            }

        }
        private void colide(int iS1)
        {

            int iS2;
            if (((iS2 = starArray[iS1].ColisionsRef) != -1) && starArray[iS2].Enabled)
            {
                starArray[iS1].ColisionsRef = -1;
                /*
                Console.WriteLine("colide " + iS1 + " width " + iS2);
                Console.WriteLine(starArray[iS1].Enabled);
                Console.WriteLine(starArray[iS2].Enabled);
                */
                if (starArray[iS2].ColisionsRef != -1) colide(iS2);

                double massPS1 = (float)starArray[iS1].AbsMass / (starArray[iS1].AbsMass + starArray[iS2].AbsMass);
                double massPS2 = (float)starArray[iS2].AbsMass / (starArray[iS1].AbsMass + starArray[iS2].AbsMass);


                if (iS2 == SelectetStar) SelectetStar = iS1;
                if (iS2 == FocusStar) FocusStar = iS1;
                if (iS2 == RefStar) RefStar = iS1;
                starArray[iS1].Marked |= starArray[iS2].Marked;


                starArray[iS1].UpdateMass(starArray[iS1].Mass + starArray[iS2].Mass);
                starArray[iS1].PosX = (starArray[iS1].PosX * massPS1 + starArray[iS2].PosX * massPS2);
                starArray[iS1].PosY = (starArray[iS1].PosY * massPS1 + starArray[iS2].PosY * massPS2);
                starArray[iS1].SpeedX = (starArray[iS1].SpeedX * massPS1 + starArray[iS2].SpeedX * massPS2);
                starArray[iS1].SpeedY = (starArray[iS1].SpeedY * massPS1 + starArray[iS2].SpeedY * massPS2);

                starArray[iS2].Enabled = false;

                if (starArray[iS1].Name == "") starArray[iS1].Name = starArray[iS2].Name;
                else if (starArray[iS2].Name != "") starArray[iS1].Name = starArray[iS1].Name + starArray[iS2].Name;
            }
        }
        private void simulateSelective(int start, int end)
        {
            for (int iS1 = start; iS1 < end; iS1++)
            {
                if (starArray[iS1].Enabled == true)
                {
                    //--------------------------------------------------------------Atom_Kolision
                    for (int iS2 = iS1; iS2 < starArray.Length; iS2++)
                    {
                        if (starArray[iS2].Enabled == true && iS1 != iS2)
                        { //Vergleicher jedes object mit jedem anderen

                            double distX = (starArray[iS1].PosX + 0) - (starArray[iS2].PosX + 0);
                            double distY = (starArray[iS1].PosY + 0) - (starArray[iS2].PosY + 0);
                            double dist = Math.Sqrt((distX * distX) + (distY * distY));

                            double relativDistXY = Math.Abs(distX) + Math.Abs(distY);

                            double pX = distX / relativDistXY;
                            double pY = distY / relativDistXY;

                            double massPS1 = starArray[iS1].AbsMass / (starArray[iS1].AbsMass + starArray[iS2].AbsMass);
                            double massPS2 = starArray[iS2].AbsMass / (starArray[iS1].AbsMass + starArray[iS2].AbsMass);
                            double Fg = ((starArray[iS1].Mass) * (starArray[iS2].Mass) / dist) / 100;
                            double a1 = Fg / starArray[iS1].Mass;
                            double a2 = Fg / starArray[iS2].Mass;

                            if (dist < (starArray[iS1].SizeR) + (starArray[iS2].SizeR))
                                starArray[iS1].ColisionsRef = iS2;

                            starArray[iS1].SpeedX -= (pX * a1);
                            starArray[iS1].SpeedY -= (pY * a1);
                            starArray[iS2].SpeedX += (pX * a2);
                            starArray[iS2].SpeedY += (pY * a2);

                        }
                    }  //Vergleicher jedes object mit jedem anderen
                    //--------------------------------------------------------------End_Atom_Kolision

                }
            }
            return;
        }

    }
}

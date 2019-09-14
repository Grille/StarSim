using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WF = System.Windows.Forms;

namespace StarSim
{
    public class StarSim
    {
        public event EventHandler FrameCalculatet;
        private WF.Timer TimerLogik;

        private bool newFrameCalculatet = false;
        private bool running = false;

        private Task mainLogikTask;
        private Task[] logikTasks;

        public int SimSpeed = 1;

        public Star[] Stars { get; set; }
        public Star SelectetStar = null, FocusStar = null, RefStar = null;

        public int UsedTime { get; private set; } = 0;
        public int StarCount { get; private set; } = 0;

        private float totalMass = 0;

        public double MassCenterX { get; private set; } = 0;
        public double MassCenterY { get; private set; } = 0;
        public double SpeedCenterX { get; private set; } = 0;
        public double SpeedCenterY { get; private set; } = 0;

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
        public bool NewFrameCalculatet
        {
            get
            {
                if (newFrameCalculatet)
                {
                    newFrameCalculatet = false;
                    return true;
                }
                else return false;
            }
        }

        public StarSim()
        { 
            TimerLogik = new WF.Timer();
            TimerLogik.Interval = 25;
            TimerLogik.Tick += new System.EventHandler(this.simulationTick);
            Stars = new Star[0];
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
                Stars = new Star[stars];
                var rnd = new Random();

                if (mode == 0)
                {
                    for (int ii = 0; ii < Stars.Length; ii++)
                    {
                        Stars[ii] = new Star(ii, (float)((maxMass - minMass) * rnd.NextDouble() + minMass)
                            , (float)(size * rnd.NextDouble() - size / 2)
                            , (float)(size * rnd.NextDouble() - size / 2)
                            , (float)((maxSpeed - minSpeed) * rnd.NextDouble() + minSpeed)
                            , (float)((maxSpeed - minSpeed) * rnd.NextDouble() + minSpeed)
                        );
                    }
                }
                else
                {
                    for (int ii = 0; ii < Stars.Length; ii++)
                    {
                        Stars[ii] = new Star(ii, (float)((maxMass - minMass) * rnd.NextDouble() + minMass)
                            , (float)(0)
                            , (float)(size * rnd.NextDouble() - size / 2)
                            , (float)((maxSpeed - minSpeed) * rnd.NextDouble() + minSpeed)
                            , (float)(0)
                        );
                    }
                }

                Start();
            }
            catch
            {
                Start();
            }
        }
        public void AddStar(float mass, double posX, double posY, double speedX, double speedY)
        {
            collapseStarArray(StarCount + 1);
            Stars[StarCount] = new Star(StarCount, mass, posX, posY, speedX, speedY);
            StarCount++;
        }

        private void collapseStarArray(int lenght)
        {
            int iDst = 0;
            Star[] newStars = new Star[lenght];
            for (int iSrc = 0; iSrc < Stars.Length; iSrc++)
            {
                if (Stars[iSrc].Enabled)
                {
                    Stars[iSrc].Idx = iDst;
                    newStars[iDst] = Stars[iSrc];
                    iDst++;
                }
            }
            Stars = newStars;
        }
        public void CollapseStarArray() { collapseStarArray(StarCount); }

        private void simulationTick(object sender, EventArgs e)
        {
            if (mainLogikTask == null || mainLogikTask.IsCompleted)
            {
                if (mainLogikTask != null && mainLogikTask.Status == TaskStatus.Faulted) Console.WriteLine("Task Crash");
                mainLogikTask = new Task(() => simulate());
                mainLogikTask.Start();
            }
            float[] power = new float[2];
        }
        private void simulate()
        {
            for (int j = 0; j < SimSpeed; j++)
            {
                var SWTotal = new Stopwatch();
                SWTotal.Start();

                int newEnabeldStarNumber = 0;

                int length = Stars.Length;
                int tasks = Environment.ProcessorCount * 2;
                int pairs = length * length / 2 - length / 2;
                float step = length / (float)tasks;

                Console.WriteLine();
                Stopwatch[] stopwatch = new Stopwatch[tasks];
                logikTasks = new Task[tasks];
                for (int iTask = 0; iTask < tasks; iTask++)
                {
                    int index = iTask;
                    stopwatch[index] = new Stopwatch();
                    logikTasks[index] = new Task(() => simulateSection((int)(step * index), (int)(step * (index + 1)), stopwatch[index]));
                    logikTasks[index].Start();
                }
                for (int iTask = 0; iTask < tasks; iTask++)
                {
                    int index = iTask; logikTasks[index].Wait();
                    Console.WriteLine("" + index + " => " + stopwatch[index].ElapsedMilliseconds);
                }

                double newMassCenterX = 0, newMassCenterY = 0;
                double newSpeedCenterX = 0, newSpeedCenterY = 0;
                float newTotalMass = 0;
                for (int iS1 = 0; iS1 < Stars.Length; iS1++)
                {
                    colide(Stars[iS1]);
                }
                for (int iS1 = 0; iS1 < Stars.Length; iS1++)
                {
                    var star1 = Stars[iS1];
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
                    MassCenterX = newMassCenterX / newTotalMass; MassCenterY = newMassCenterY / newTotalMass;
                    SpeedCenterX = newSpeedCenterX / newTotalMass; SpeedCenterY = newSpeedCenterY / newTotalMass;
                }
                else
                {
                    MassCenterX = newMassCenterX; MassCenterY = newMassCenterY;
                    SpeedCenterX = newSpeedCenterX; SpeedCenterY = newSpeedCenterY;
                }

                StarCount = newEnabeldStarNumber;
                if (Stars.Length - newEnabeldStarNumber > 100) CollapseStarArray();

                FrameCalculatet?.Invoke(this, new EventArgs());

                SWTotal.Stop();
                UsedTime = (int)SWTotal.ElapsedMilliseconds;

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
                else if (star2.Name != "") star1.Name += star2.Name;
            }
        }
        private void simulateSection(int start, int end, Stopwatch stopwatch)
        {
            stopwatch.Start();
            for (int iS1 = start; iS1 < end; iS1++)
            {
                var star1 = Stars[iS1];
                if (Stars[iS1].Enabled == true)
                {
                    for (int iS2 = iS1; iS2 < Stars.Length; iS2++)
                    {
                        var star2 = Stars[iS2];
                        if (Stars[iS2].Enabled == true && iS1 != iS2)
                        {
                            double distX = (star1.PosX) - (star2.PosX);
                            double distY = (star1.PosY) - (star2.PosY);
                            double dist = Math.Sqrt((distX * distX) + (distY * distY));
                            
                            double relativDistXY = Math.Abs(distX) + Math.Abs(distY);
                            
                            double pX = distX / relativDistXY;
                            double pY = distY / relativDistXY;

                            double Fg = (star1.AbsMass * star2.AbsMass / dist) / 100;
                            double a1 = Fg / star1.Mass;
                            double a2 = Fg / star2.Mass;
                            
                            if (dist < star1.Radius + star2.Radius)
                                star1.ColisionsRef = star2;

                            star1.SpeedX -= (pX * a1);
                            star1.SpeedY -= (pY * a1);
                            star2.SpeedX += (pX * a2);
                            star2.SpeedY += (pY * a2);
                        }
                    }
                }
            }
            stopwatch.Stop();
            return;
        }

    }
}

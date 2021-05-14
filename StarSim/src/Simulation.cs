using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WF = System.Windows.Forms;

namespace StarSim
{
    public class Simulation
    {
        public PerformanceWatch Stats;
        public PerformanceWatch[] TaskStats;

        public int TaskCount;

        private Task mainLogikTask;
        private Task[] subLogikTasks;

        private SimData data;
        private Camera camera;

        public int SimSpeed = 1;
        public double SimMultiplier = 1;



        public bool NewFrameCalculatet = true;

        public Simulation(Camera camera, SimData data)
        {
            this.camera = camera;
            this.data = data;

            TaskCount = Environment.ProcessorCount;
            Stats = new PerformanceWatch();
            TaskStats = new PerformanceWatch[TaskCount];
            for (int i = 0; i < TaskCount; i++)
                TaskStats[i] = new PerformanceWatch();
        }

        public void WaitForIdle()
        {
            mainLogikTask.Wait();
            if (subLogikTasks != null)
                for (int i = 0; i < subLogikTasks.Length; i++) subLogikTasks[i].Wait();
        }
        public void Run()
        {
            if (mainLogikTask == null || mainLogikTask.IsCompleted)
            {
                if (mainLogikTask != null && mainLogikTask.Status == TaskStatus.Faulted) Console.WriteLine("Task Crash");
                mainLogikTask = new Task(() => Simulate());
                mainLogikTask.Start();
            }
        }

        public void Simulate()
        {
            for (int i = 0; i< SimSpeed; i++)
            {
                simulate();
            }
        }
        private void simulate()
        {
            Stats.Begin();

            int newEnabeldStarNumber = 0;

            var stars = data.Stars;
            int length = stars.Length;


            if (length == 0)
                return;

            int taskCount = TaskCount;
            int pairs = length * length / 2 - length / 2;
            float step = length / (float)taskCount;

            Console.WriteLine();
            subLogikTasks = new Task[taskCount];
            for (int iTask = 0; iTask < taskCount; iTask++)
            {
                int index = iTask;
                subLogikTasks[index] = new Task(() => simulateSection(index, (int)(step * index), (int)(step * (index + 1))));
                subLogikTasks[index].Start();
            }
            for (int iTask = 0; iTask < taskCount; iTask++)
            {
                int index = iTask; subLogikTasks[index].Wait();
            }

            double newMassCenterX = 0, newMassCenterY = 0;
            double newSpeedCenterX = 0, newSpeedCenterY = 0;
            double newTotalMass = 0;
            for (int iS1 = 0; iS1 < stars.Length; iS1++)
            {
                colide(stars[iS1]);
            }
            for (int iS1 = 0; iS1 < stars.Length; iS1++)
            {
                var star1 = stars[iS1];
                if (star1.Enabled == false) continue;
                else
                {
                    newEnabeldStarNumber++;

                    star1.PosX += star1.SpeedX * SimMultiplier;
                    star1.PosY += star1.SpeedY * SimMultiplier;

                    newMassCenterX += star1.PosX * star1.AbsMass;
                    newMassCenterY += star1.PosY * star1.AbsMass;
                    newSpeedCenterX += star1.SpeedX * star1.AbsMass * SimMultiplier;
                    newSpeedCenterY += star1.SpeedY * star1.AbsMass * SimMultiplier;

                    newTotalMass += star1.AbsMass;
                }
            }
            
            data.TotalMass = newTotalMass;
            if (data.TotalMass != 0)
            {
                data.MassCenterX = newMassCenterX / newTotalMass; 
                data.MassCenterY = newMassCenterY / newTotalMass;
                data.SpeedCenterX = newSpeedCenterX / newTotalMass; 
                data.SpeedCenterY = newSpeedCenterY / newTotalMass;
            }
            else
            {
                data.MassCenterX = newMassCenterX; 
                data.MassCenterY = newMassCenterY;
                data.SpeedCenterX = newSpeedCenterX; 
                data.SpeedCenterY = newSpeedCenterY;
            }

            if (data.FocusStar != null && data.FocusStar.Enabled == true)
            {
                camera.PosX += data.FocusStar.SpeedX;
                camera.PosY += data.FocusStar.SpeedY;
            }
            else
            {
                camera.PosX += data.SpeedCenterX;
                camera.PosY += data.SpeedCenterY;
            }

            data.StarCount = newEnabeldStarNumber;
            if (stars.Length - newEnabeldStarNumber > 100) 
                data.CollapseStarArray();

            Stats.EndAndLog();

            NewFrameCalculatet = true;

        }
        private void colide(Star star1)
        {
            Star star2;
            if (((star2 = star1.ColisionsRef) != null) && star2.Enabled)
            {
                star1.ColisionsRef = null;

                if (star2.ColisionsRef != null) colide(star2);

                double massPS1 = star1.AbsMass / (star1.AbsMass + star2.AbsMass);
                double massPS2 = star2.AbsMass / (star1.AbsMass + star2.AbsMass);
                
                if (star2 == data.SelectetStar) data.SelectetStar = star1;
                if (star2 == data.FocusStar) data.FocusStar = star1;
                if (star2 == data.RefStar) data.RefStar = star1;
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
        private void simulateSection(int id, int start, int end)
        {
            var stars = data.Stars;
            var stats = TaskStats[id];
            stats.Begin();

            for (int iS1 = start; iS1 < end; iS1++)
            {
                var star1 = stars[iS1];
                if (stars[iS1].Enabled == true)
                {
                    for (int iS2 = iS1; iS2 < stars.Length; iS2++)
                    {
                        var star2 = stars[iS2];
                        if (stars[iS2].Enabled == true && iS1 != iS2)
                        {
                            double distX = (star1.PosX) - (star2.PosX);
                            double distY = (star1.PosY) - (star2.PosY);
                            double dist = Math.Sqrt((distX * distX) + (distY * distY));
                            
                            double relativDistXY = Math.Abs(distX) + Math.Abs(distY);
                            double propX = distX / relativDistXY;
                            double propY = distY / relativDistXY;

                            double fg = (star1.AbsMass * star2.AbsMass) / (dist * dist) * 0.01;
                            double fgPropS1 = fg / star1.Mass;
                            double fgPropS2 = fg / star2.Mass;
                            
                            if (dist < star1.Radius + star2.Radius)
                                star1.ColisionsRef = star2;

                            star1.SpeedX -= fgPropS1 * propX * SimMultiplier;
                            star1.SpeedY -= fgPropS1 * propY * SimMultiplier;
                            star2.SpeedX += fgPropS2 * propX * SimMultiplier;
                            star2.SpeedY += fgPropS2 * propY * SimMultiplier;
                        }
                    }
                }
            }

            stats.EndAndLog();
            return;
        }

    }
}

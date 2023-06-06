using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using WF = System.Windows.Forms;

namespace StarSim
{
    public class Simulation
    {
        public PerformanceWatch Stats;
        public PerformanceWatch[] TaskStats;

        public double GravitationalConstant;
        private double gravitationalForce;

        public int TaskCount;

        private SimulationData data;
        private Camera camera;

        public int SimSpeed = 1;
        public double SimMultiplier = 1;



        public bool NewFrameCalculatet = true;

        public Simulation(Camera camera, SimulationData data)
        {
            this.camera = camera;
            this.data = data;

            GravitationalConstant = 0.01; //Math.Pow(6.674*10, -11);
            //Console.WriteLine(GravitationalConstant);

            TaskCount = Environment.ProcessorCount;
            Stats = new PerformanceWatch();
            TaskStats = new PerformanceWatch[TaskCount];
            for (int i = 0; i < TaskCount; i++)
                TaskStats[i] = new PerformanceWatch();
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

            gravitationalForce = GravitationalConstant;

            int newEnabeldStarNumber = 0;

            var stars = data;
            int length = stars.Count;


            if (length == 0)
                return;

            for (int iS1 = 0; iS1 < stars.Count; iS1++)
            {
                stars[iS1].PullX = 0;
                stars[iS1].PullY = 0;
            }

            var tasks = new Task[TaskCount];
            for (int iTask = 0; iTask < TaskCount; iTask++)
            {
                tasks[iTask] = RunCollateTask(iTask, TaskCount);
            }
            Task.WaitAll(tasks);

            double newMassCenterX = 0, newMassCenterY = 0;
            double newSpeedCenterX = 0, newSpeedCenterY = 0;
            double newTotalMass = 0;
            for (int iS1 = 0; iS1 < stars.Count; iS1++)
            {
                ApplyColisions(stars[iS1]);
            }
            for (int iS1 = 0; iS1 < stars.Count; iS1++)
            {
                var star1 = stars[iS1];
                if (star1.Enabled == false) continue;
                else
                {
                    newEnabeldStarNumber++;

                    star1.SpeedX += star1.PullX;
                    star1.SpeedY += star1.PullY;

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

            data.EnabledCount = newEnabeldStarNumber;
            if (stars.Count - newEnabeldStarNumber > 100) 
                data.CollapseStarArray();

            Stats.EndAndLog();

            NewFrameCalculatet = true;

        }

        private void ApplyColisions(Star star1)
        {
            var star2 = star1.ColisionsRef;

            if (star2 == null || !star2.Enabled)
                return;

            star1.ColisionsRef = null;

            if (star2.ColisionsRef != null) 
                ApplyColisions(star2);

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

            if (star1.Name == "")
                star1.Name = star2.Name;
            else if (star2.Name != "")
                star1.Name += star2.Name;
        }

        private void simulateSection(int id, int start, int end)
        {
            var stars = data;
            var stats = TaskStats[id];
            stats.Begin();

            for (int iS1 = start; iS1 < end; iS1++)
            {
                var star1 = stars[iS1];
                if (stars[iS1].Enabled == true)
                {
                    for (int iS2 = iS1; iS2 < stars.Count; iS2++)
                    {
                        var star2 = stars[iS2];
                        if (stars[iS2].Enabled == true && iS1 != iS2)
                        {
                            CollateStars(star1, star2);
                        }
                    }
                }
            }

            stats.EndAndLog();
            return;
        }

        private Range GetTaskRange(int taskIndex, int totalTaskCount)
        {
            long totalComparisons = data.TotalComparisons;

            long numTasks = totalTaskCount;
            long comparisonsPerTask = totalComparisons / numTasks;

            long startIndex = taskIndex * comparisonsPerTask;
            long endIndex = (taskIndex == numTasks - 1) ? totalComparisons - 1 : (taskIndex + 1) * comparisonsPerTask - 1;

            return new Range((int)startIndex, (int)endIndex);
        }

        private Task RunCollateTask(int taskIndex, int totalTaskCount)
        {
            var stats = TaskStats[taskIndex];

            return Task.Run(() =>
            {
                stats.Begin();

                var range = GetTaskRange(taskIndex, totalTaskCount);
                int startIndex = range.Start;
                int endIndex = range.End;

                // Perform comparisons within the assigned range
                for (int i = startIndex; i <= endIndex; i++)
                {

                    var pair = data.GetComparisonPair(i);

                    if (pair.Enabled)
                    {
                        CollateStars(pair.Star1, pair.Star2);
                    }
                }

                stats.EndAndLog();
            });

        }

        private void CollateStars(Star star1, Star star2)
        {
            double distX = (star1.PosX) - (star2.PosX);
            double distY = (star1.PosY) - (star2.PosY);
            double dist = Math.Sqrt((distX * distX) + (distY * distY));

            double relativDistXY = Math.Abs(distX) + Math.Abs(distY);
            double propX = distX / relativDistXY;
            double propY = distY / relativDistXY;

            double fg = (star1.AbsMass * star2.AbsMass) / (dist * dist) * gravitationalForce;
            double fgStar1 = fg / star1.Mass;
            double fgStar2 = fg / star2.Mass;

            if (dist < star1.Radius + star2.Radius)
                star1.ColisionsRef = star2;

            star1.PullX -= fgStar1 * propX * SimMultiplier;
            star1.PullY -= fgStar1 * propY * SimMultiplier;
            star2.PullX += fgStar2 * propX * SimMultiplier;
            star2.PullY += fgStar2 * propY * SimMultiplier;
        }

    }
}

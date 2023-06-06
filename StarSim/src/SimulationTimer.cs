using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSim
{
    public class SimulationTimer
    {
        Task timer;

        public float TickrateInMs;

        public readonly Simulation Simulation;

        private bool enabled;
        private bool enableOnFree;
        private bool isLocked = false;

        public SimulationTimer(Simulation simulation)
        {
            Simulation = simulation;
        }

        public bool Running => enabled;


        public void Start()
        {
            AssertNotLocked(nameof(Lock));

            if (enabled)
                return;

            enabled = true;

            timer = Task.Run(() =>
            {
                while (true)
                {
                    if (!enabled)
                        return;

                    Simulation.Simulate();


                    //Task.Delay(10).Wait();
                }
            });
        }

        public void Stop()
        {
            AssertNotLocked(nameof(Lock));

            enabled = false;
            timer.Wait();
        }

        public void Lock()
        {
            AssertNotLocked(nameof(Lock));

            enableOnFree = enabled;
            Stop();
            isLocked = true;
        }

        public void Free()
        {
            if (!isLocked)
                throw new InvalidOperationException();

            isLocked = false;
            if (enableOnFree)
                Start();
        }

        private void AssertNotLocked(string name)
        {
            if (isLocked)
            {
                throw new InvalidOperationException($"Action {name} can't be executed while locked.");
            }
        }
    }
}

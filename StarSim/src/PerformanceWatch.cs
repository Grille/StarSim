using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSim
{
    public class PerformanceWatch
    {
        long beginDate;
        long logDate;

        double time = 0;
        double avTime;
        int count = 0;

        private SortedList<int, double> times;

        public float AverageTime
        {
            get
            {
                return (float)time;
            }
        }
        public float FPS { private set; get; } = 0;

        public void Begin()
        {
           beginDate = DateTime.Now.Ticks;
        }

        public void EndAndLog()
        {
            time = (DateTime.Now.Ticks - beginDate) / (double)TimeSpan.TicksPerMillisecond;
            count += 1;

            if (((DateTime.Now.Ticks - logDate) / (double)TimeSpan.TicksPerSecond) > 1)
            {
                logDate = DateTime.Now.Ticks;
                FPS = count;
                count = 0;
            }
            
        }


    }
}

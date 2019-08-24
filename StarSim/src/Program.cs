using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace StarSim
{
    static class Program
    {
        public static StarSim Simulation;
        /// <summary>
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Simulation = new StarSim();
            Application.Run(new MainWindow());
        }
    }
}

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
            //DoSomethingInCpp(10);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Application.EnableVisualStyles();
            //Application.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Application.SetCompatibleTextRenderingDefault(false);
            Simulation = new StarSim();
            Application.Run(new MainWindow());
            //Application.Restart();
        }
    }
}

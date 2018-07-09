using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices; // Needed for DllImport
using System.Diagnostics; // Used for Debug.WriteLine

namespace StarSim
{
    static class Program
    {
        public static StarSim Simulation;
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
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

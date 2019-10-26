using AutoUpdaterDotNET;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace TraCuuThongBaoPhatHanh_v2
{
    static class Program
    {
        internal static readonly Version Version = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormEntry());
        }

        internal static void CheckForUpdate()
        {
            {
                AutoUpdater.RunUpdateAsAdmin = false;
                AutoUpdater.UpdateFormSize = new System.Drawing.Size(800, 600);

                AutoUpdater.Start("https://raw.githubusercontent.com/Devancy/TraCuuThongBaoPhatHanh/master/Deploy/2/AutoUpdater.xml");
            }
        }
    }
}

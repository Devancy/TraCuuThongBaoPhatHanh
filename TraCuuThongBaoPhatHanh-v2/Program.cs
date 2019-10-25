using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoUpdaterDotNET;

namespace TraCuuThongBaoPhatHanh_v2
{
    static class Program
    {
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

                AutoUpdater.Start("https://raw.githubusercontent.com/Devancy/TraCuuThongBaoPhatHanh/master/TraCuuThongBaoPhatHanh/Deploy/AutoUpdater2.xml");
            }
        }
    }
}

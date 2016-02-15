using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RealTimeTicket.WinFormServer
{
    static class Program
    {
        internal static FrmServer MainForm { get; set; }
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm = new FrmServer();
            Application.Run(MainForm);
        }
    }
}

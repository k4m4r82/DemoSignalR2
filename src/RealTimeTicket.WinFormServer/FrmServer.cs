using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealTimeTicket.WinFormServer
{
    public partial class FrmServer : Form
    {
        private IDisposable signalR { get; set; }
        private const string SERVER_URL = "http://192.168.56.1:8282";

        public FrmServer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method adds a line to the RichTextBoxConsole control, using Invoke if used
        /// from a SignalR hub thread rather than the UI thread.
        /// </summary>
        /// <param name="message"></param>
        internal void WriteToConsole(String message)
        {
            if (lstConsole.InvokeRequired)
            {
                this.Invoke((Action)(() =>
                    WriteToConsole(message)
                ));

                return;
            }

            lstConsole.Items.Add(message);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            WriteToConsole("Starting server...");

            btnStart.Enabled = false;
            Task.Run(() => StartServer());
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Starts the server and checks for error thrown when another server is already 
        /// running. This method is called asynchronously from Button_Start.
        /// </summary>
        private void StartServer()
        {
            try
            {
                signalR = WebApp.Start(SERVER_URL);
            }
            catch (TargetInvocationException)
            {
                WriteToConsole("Server failed to start. A server is already running on " + SERVER_URL);

                //Re-enable button to let user try to start server again
                this.Invoke((Action)(() => btnStart.Enabled = true));
                return;
            }

            this.Invoke((Action)(() => btnStop.Enabled = true));
            WriteToConsole("Server started at " + SERVER_URL);
        }

        private void FrmServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (signalR != null)
            {
                signalR.Dispose();
            }
        }
    }
}

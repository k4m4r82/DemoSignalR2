using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RealTimeTicket.WinFormClient
{
    public partial class FrmClient : Form
    {
        private const string SERVER_URL = "http://192.168.56.1:8282";

        private HubConnection hubConnection;
        private IHubProxy hubProxy;
        private IList<Button> listOfBtnKursi = new List<Button>();

        public FrmClient()
        {
            InitializeComponent();
            InitInformasiKursi();

            hubConnection = new HubConnection(SERVER_URL);

            // membuat objek proxy dari class hub yang ada di server
            hubProxy = hubConnection.CreateHubProxy("ServerHub");

            // set mode listen untuk method RefreshData
            // method RefreshData sebelumnya harus didefinisikan di server
            hubProxy.On<string, bool>("RefreshData", (noKursi, isOrder) => RefreshData(noKursi, isOrder));
            ConnectAsync();
        }

        private void RefreshData(string noKursi, bool isOrder)
        {
            var btnKursi = listOfBtnKursi.Where(f => f.Tag.ToString() == noKursi)
                                         .SingleOrDefault();
            if (btnKursi != null)
            {
                btnKursi.BackColor = isOrder ? Color.Red : Color.Green;
            }
        }

        /// <summary>
        /// Creates and connects the hub connection and hub proxy.
        /// </summary>
        private async void ConnectAsync()
        {
            try
            {
                await hubConnection.Start();
            }
            catch
            {
                MessageBox.Show("Unable to connect to server: Start server before connecting clients.");
                return;
            }

        }

        private void InitInformasiKursi()
        {
            for (int i = 1; i < 57; i++)
            {
                var btnKursi = new Button
                {
                    Font = new Font(new Button().Font.Name, 10f, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(65, 65),
                    Tag = i.ToString()
                };

                btnKursi.Text = string.Format("{0:00}", i);
                btnKursi.BackColor = Color.Green;
                btnKursi.Click += btnKursi_Click;

                listOfBtnKursi.Add(btnKursi);
                pnlKursi.Controls.Add(btnKursi);
            }            
        }

        private void btnKursi_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var noKursi = btn.Tag.ToString();
            var isOrder = btn.BackColor == Color.Green;

            // panggil method server
            hubProxy.Invoke("OrderTicket", noKursi, isOrder);
        }

        private void FrmClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hubConnection != null)
            {
                hubConnection.Stop();
                hubConnection.Dispose();
            }
        }
    }
}

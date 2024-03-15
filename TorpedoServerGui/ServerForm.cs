using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Torpedo;

namespace TorpedoServerGui
{
    public partial class ServerForm : Form
    {
        Thread logFeederThread;
        Thread gamesFeederThread;
        Thread serverThread;

        List<string> logs;

        public ServerForm()
        {
            InitializeComponent();
            label1.Text = $"LevTorpedó szerver. Port: {HelperClass.serverport}";
            logs  = new List<string>();

            HelperClass.sf = this;

            LevLogger logger = new LevLogger(LogLevel.LogInfo, "none");
            HelperClass.logger = logger;
            HelperClass.communicator.logger = logger;

            // Feederek elinditasa
            ThreadStart logFeeder = HelperClass.LogFeeder;
            ThreadStart gamesFeeder = HelperClass.GamesFeeder;
            logFeederThread = new Thread(logFeeder);
            gamesFeederThread = new Thread(gamesFeeder);
            logFeederThread.Start();
            gamesFeederThread.Start();

            // Main szerver thread elinditasa
            ThreadStart threadStart = HelperClass.communicator.StartServerLoop;
            serverThread = new Thread(threadStart);
            serverThread.Start();


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void TextBox_Log_TextChanged_1(object sender, EventArgs e)
        {
            TextBox_Log.SelectionStart = TextBox_Log.Text.Length;
            TextBox_Log.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string selectedRoomCode = listBox1.GetItemText(listBox1.SelectedItem);
            HelperClass.communicator.DeleteRoom(selectedRoomCode);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            string selectedRoomCode = listBox1.GetItemText(listBox1.SelectedItem);
            string[] players = HelperClass.communicator.GetPlayerNames(selectedRoomCode);
            listBox2.DataSource = players;
        }

        delegate void SetTextCallback(string[] txtArr);

        public void UpdateLogs(string[] logs)
        {
            if (this.TextBox_Log.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(UpdateLogs);
                this.Invoke(d, new object[] { logs });
            }
            else
            {
                foreach (string log in logs)
                {
                    if (!this.logs.Contains(log))
                    {
                        this.logs.Add(log);
                        TextBox_Log.Text = TextBox_Log.Text + log + "\n";
                    }
                }
                TextBox_Log.SelectionStart = TextBox_Log.Text.Length;
                TextBox_Log.ScrollToCaret();
            }

        }

        public void UpdateGames(string[] games)
        {
            if (this.listBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(UpdateGames);
                this.Invoke(d, new object[] { games });
            }
            else
            {
                if (games.Length == 0)
                {
                    listBox1.DataSource = games;
                }
                else
                {
                    listBox1.DataSource = games;
                    if (listBox1.SelectedIndex < listBox1.Items.Count)
                    {
                        int currIndex = listBox1.SelectedIndex;
                        listBox1.SelectedIndex = currIndex;
                    }
                }
            }
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            logFeederThread.Interrupt();
            gamesFeederThread.Interrupt();
            serverThread.Interrupt();
            HelperClass.form1.Show();
            this.Close();
        }
    }
}

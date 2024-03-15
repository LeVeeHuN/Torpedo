namespace TorpedoServerGui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button_startServer_Click(object sender, EventArgs e)
        {
            try
            {
                int port = int.Parse(textBox_serverPort.Text);
                HelperClass.serverport = port;
                HelperClass.communicator = new Torpedo.Communicator(port);
                HelperClass.form1 = this;

                Form serverForm = new ServerForm();
                this.Hide();
                serverForm.Show();
                //button_startServer.Enabled = false;
            }
            catch
            {
                MessageBox.Show("�rv�nytelen port! Csak sz�mokat adj meg.", "�rv�nytelen port!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TFTP
{
    public partial class ClientForm : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);
        string client_env = "";
        string client_file = "";
        public delegate void log(string msg);
        public log Log;
        public ClientForm()
        {
            InitializeComponent();
            Log = LogWorkServer;
        }
        object ob = new object();
        private void LogWorkServer(string msg)
        {
            lock (ob)
                toolStripStatusLabel1.Text = msg;
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (groupBox2.Visible == false)
                groupBox2.Visible = true;
            else
                groupBox2.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (groupBox1.Visible == false)
                groupBox1.Visible = true;
            else
                groupBox1.Visible = false;
        }

        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog FBD = new OpenFileDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                client_file = FBD.FileName;
                button4.Enabled = true;
            }
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            groupBox1.Visible = groupBox2.Visible = false;
            button4.Enabled = button6.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var client = new Client();
            client.form = this;
            client.upload(textBox1.Text, client_file);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                client_env = FBD.SelectedPath;
                button6.Enabled = true;
            }
        }
        bool check_ip()
        {
            IPAddress iPAddress;
            if (IPAddress.TryParse(textBox1.Text, out iPAddress))
            {
                return true;
            }
            else return false;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (!check_ip()) { MessageBox.Show("Не верный IP!"); return; }
            if (textBox3.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите имя файла"); return;
            }
            var client = new Client();
            client.form = this;
            client.download(textBox1.Text, textBox3.Text, client_env);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
    }
}
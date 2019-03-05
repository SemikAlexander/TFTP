using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TFTP
{
    public partial class ServerForm : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hwnd, int wmsg, int wparam, int lparam);
        public delegate void log(string msg);
        public log Log;
        Server server;
        string serv_env = "";
        public ServerForm()
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
        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                serv_env = FBD.SelectedPath;
                dir_serv.Visible = false;
                button1.Visible = true;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            server = new Server();
            server.run(serv_env, this);
            button1.Enabled = false;
            button2.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Visible = button2.Visible = false;
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            button1.Visible = button2.Visible = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
    }
}
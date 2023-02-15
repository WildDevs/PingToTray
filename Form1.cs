using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;

namespace PingToTray
{
    public partial class Form1 : Form
    {
        Thread t;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            t = new Thread(delegate()
            {
				Ping myPing = new Ping();
				string host = "37.120.190.250";
				byte[] buffer = new byte[32];
				int timeout = 4000;
				notifyIcon1.Text = "";

				PingOptions pingOptions = new PingOptions();
				int currentping = 0;
				while (true)
				{
					try
					{
						PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
						
						if (reply.Status == IPStatus.Success)
						{
							currentping = Int32.Parse(reply.RoundtripTime.ToString());
						}
						else if (reply.Status == IPStatus.TimedOut)
						{
							currentping = -1;
						}
					}
					catch (Exception ex)
					{
						
					}

					UpdateTaskBar(currentping);
					Thread.Sleep(2000);
				}
            });
            t.Start();
            Opacity = 0;
            ShowInTaskbar = false;
        }

        private void UpdateTaskBar(int ping)
        {
            Bitmap bitmap = new Bitmap(16,16);
            SolidBrush brush = new SolidBrush(Color.White);
            Graphics graphics = Graphics.FromImage(bitmap);
            FontFamily myFontFamily = new FontFamily("Comic Sans MS");
            Font myFont = new Font(myFontFamily, 10, FontStyle.Regular, GraphicsUnit.Pixel);

            if (ping < 50)
            {
                brush = new SolidBrush(Color.White);
                graphics.DrawString(ping.ToString(), myFont, brush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            } else if (ping < 100)
            {
                brush.Color = Color.Orange;
                graphics.DrawString(ping.ToString(), myFont, brush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            } 
            else
            {
                brush.Color = Color.Red;
                graphics.DrawString(ping.ToString(), myFont, brush, -3, 0);
            } 

            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);
            notifyIcon1.Icon = icon;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t.Abort();
            Application.Exit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Control.MousePosition);
            }
        }
    }
}

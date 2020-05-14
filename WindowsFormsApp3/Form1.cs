using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        Net net;
        PictureBox pb;
        Graphics bitmapGraphics;
        MouseEventArgs bufer;

        public Form1()
        {
            InitializeComponent();
            
            pb = new PictureBox()
            {
                Width = Width - 100,
                Height = Height - 100,
                Top = 100,
                Left = 100,
                Parent = this,
                BackColor = Color.LightCyan
            };

            Bitmap myBitmap = new Bitmap(pb.Width, pb.Height);
            bitmapGraphics = Graphics.FromImage(myBitmap);
            Button refresh = new Button()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Text = "Refresh"
            };
            refresh.Click += Refresh_Click;

            pb.Image = myBitmap;
                       

            pb.Click += Pb_Click;
            bufer = null;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            net = new Net(bitmapGraphics, 10, pb.Width, pb.Height);
            this.Refresh();
        }

        private void Pb_Click(object sender, EventArgs e)
        {
            MouseEventArgs e2 = (MouseEventArgs)e;
            if (bufer != null)
            {
                net.Swap(e2.X, e2.Y, bufer.X, bufer.Y);
                bufer = null;
            }
            else
            {
                bufer = e2;
            }

            pb.Refresh();
        }
    }
}

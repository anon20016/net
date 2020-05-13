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
        public Form1()
        {
            InitializeComponent();
            
            pb = new PictureBox()
            {
                Width = Width,
                Height = Height,
                Parent = this
            };

            Bitmap myBitmap = new Bitmap(Width, Height);
            Graphics bitmapGraphics = Graphics.FromImage(myBitmap);
            pb.Image = myBitmap;

            net = new Net(bitmapGraphics, 15, Width, Height, 15);

            pb.Click += Pb_Click;
        }

        private void Pb_Click(object sender, EventArgs e)
        {
            MouseEventArgs e2 = (MouseEventArgs)e;
            net.moveBall(e2.X, e2.Y);
            pb.Refresh();
        }
    }
}

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
            // Начало игры
            net = new Net(bitmapGraphics, 6, pb.Width, pb.Height, pb, 8, numColors:6, true);
            net.onScoreChaged += ScoreChange;
            ScoreChange(-1);
        }

        private void Pb_Click(object sender, EventArgs e)
        {
            MouseEventArgs e2 = (MouseEventArgs)e;
            if (bufer != null)
            {
                if (net.Swap(e2.X, e2.Y, bufer.X, bufer.Y, 200))
                {
                    bufer = null;
                }
                else
                {
                    DeSelectBall(bufer.X, bufer.Y);
                    bufer = null;
                    pb.Refresh();
                }                
            }
            else
            {
                if (net.GetBallNumber(e2.X, e2.Y) != -1)
                {
                    bufer = e2;
                    SelectBall(e2.X, e2.Y);
                    pb.Refresh();
                }
            }
        }

        private void SelectBall(float x, float y)
        {
            net.GetBall(net.GetBallNumber(x, y)).Select();
        }
        private void DeSelectBall(float x, float y)
        {
            net.GetBall(net.GetBallNumber(x, y)).DeSelect();
        }
    
        private void ScoreChange(int s)
        {
            if (s == -1)
            {
                Controls["score"].Text = "Score: 0";
            }
            else
            {
                Controls["score"].Text = "Score: " + (Int32.Parse(Controls["score"].Text.Substring(7)) + s).ToString();
                Controls["score"].Refresh();
            }
        }    
    }
}

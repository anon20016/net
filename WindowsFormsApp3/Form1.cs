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
            //this.Size = new Size(1000, 1000);
            InitializeComponent();

            Label Score = new Label()
            {
                Name = "Score",
                Parent = this,
                Left = 100,
                Top = 0,
                Text = "Score: 0",
                Width = Width - 100,
                Height = 100,
                Font = new Font("Arial", 30),
                TextAlign = ContentAlignment.MiddleCenter
            };


           
            
            

            Score.Hide();
            Panel titul = new Panel()
            {
                Name = "main",
                Parent = this,
                Top = 0,
                Left = 0,
                Width = Width,
                Height = Height,
                BackColor = Color.White,
            };
            titul.Click += Titul_Click;
            Panel main = new Panel()
            {
                Name = "main",
                Parent = this,
                Top = 0,
                Left = 0,
                Width = Width,
                Height = Height,
                BackColor = Color.Red
            };
            Panel settings = new Panel()
            {
                Parent = main,
                BackColor = Color.Blue,
                Left = main.Width / 4,
                Top = 0,
                Width = main.Width / 4 * 3,
                Height = main.Height
            };
            settings.Hide();
            Panel game = new Panel()
            {
                Parent = main,
                BackColor = Color.Green,
                Left = main.Width / 4,
                Top = 0,
                Width = main.Width / 4 * 3,
                Height = main.Height
            };
            int mapsize = game.Width < game.Height ? game.Width : game.Height;
            pb = new PictureBox()
            {
                Width = mapsize,
                Height = mapsize,
                Top = game.Height - mapsize,
                Left = 0,
                Parent = game,
                BackColor = Color.LightCyan
            };
            Bitmap myBitmap = new Bitmap(pb.Width, pb.Height);
            bitmapGraphics = Graphics.FromImage(myBitmap);

            Button refresh = new Button()
            {
                Parent = main,
                Top = 200,
                Left = 0,
                Height = 200, 
                Width = main.Width / 4,
                Text = "Refresh"
            };
            refresh.Click += Refresh_Click;


            pb.Image = myBitmap;
                       

            pb.Click += Pb_Click;
            bufer = null;
        }

        private void Titul_Click(object sender, EventArgs e)
        {
            (sender as Panel).Hide();
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
                this.Controls["Score"].Text = "Score: 0";
            }
            else
            {
                Controls["Score"].Text = "Score: " + (Int32.Parse(Controls["Score"].Text.Substring(7)) + s).ToString();
                Controls["Score"].Refresh();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

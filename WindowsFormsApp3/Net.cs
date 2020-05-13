using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{

    class Net
    {
        private readonly Color[] colors = new Color[5] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Black };

        Random r;
        Graphics graphics;
        public int Height { get; set; }
        public int Width { get; set; }
        public int CountLevels {get; set;}
        public int CountEdge { get; set; }
        private float BallSize;
        private int N;

        List<List<Ball>> balls;

        PointF Center { get; set; }

        public Net(Graphics g, int countLevels, int width, int heght, int countEdge = 8)
        {
            graphics = g;
            Height = heght;
            Width = width;
            CountEdge = countEdge;
            CountLevels = countLevels;
            Center = new PointF(Width / 2, Height / 2);
            BallSize = Height / (6 * CountLevels);
            N = 0;
            r = new Random(DateTime.Now.Millisecond);
            balls = new List<List<Ball>>();
            for (int i = 0; i < countEdge; i++)
            {
                balls.Add(new List<Ball>(countLevels));
            }
            DrawMap();
            InitMap();
        }
       

        public int getBallNumber(float x, float y)
        {
            foreach (var o in balls)
            {
                foreach (var b in o)
                {
                    if (dist(new PointF(x, y), b.coord) <= BallSize / 2)
                    {
                        return b.Id;
                    }
                }
            }
            return -1;
        }
        public int getBallNumber(PointF x)
        {
            return getBallNumber(x.X, x.Y);
        }

        private Ball GetBall(int x)
        {
            foreach (var o in balls)
            {
                foreach (var b in o)
                {
                    if (b.Id == x)
                    {
                        return b;
                    }
                }
            }
            return null;
        }

        public Color getColor(int n)
        {
            return GetBall(n).color;
        }
        public Color getColor(float x, float y)
        {
            return GetBall(getBallNumber(x, y)).color;
        }
        public Color getColor(PointF x)
        {
            return GetBall(getBallNumber(x)).color;
        }

        public void setColor(int n, Color c)
        {
            GetBall(n).color = c;
        }
        public void setColor(float x, float y, Color c)
        {
            GetBall(getBallNumber(x, y)).color = c;
        }
        public void setColor(PointF x, Color c)
        {
            GetBall(getBallNumber(x)).color = c;
        }

        public void moveBall(int n)
        {
            int axis = getAxis(n);
            int k = -1;
            if (axis == -1)
            {
                return;
            }
            for (int i = 0; i < balls[axis].Count; i++)
            {
                if (balls[axis][i].Id == n)
                {
                    k = i;
                }
            }
            if (k == -1)
            {
                return;
            }
            for (int i = k; i > 0; i--)
            {
                balls[axis][i].color = balls[axis][i - 1].color;
            }
            GenerateNewBallAxis(axis);
        }
        public void moveBall(float x, float y)
        {
            moveBall(getBallNumber(x, y));
        }

        // TODO
        private void GenerateNewBall(int n)
        {
            int axis = getAxis(n);
            int k = -1;
            if (axis == -1)
            {
                return;
            }
            for (int i = 0; i < balls[axis].Count; i++)
            {
                if (balls[axis][i].Id == n)
                {
                    k = i;
                }
            }
            
            balls[axis][k].color = colors[r.Next(5)];
        }
        private void GenerateNewBallAxis(int x)
        {
            balls[x][0].color = colors[new Random().Next() % 5];
        }

        //Можем ли шарик n покрасить в цвет c 
        private bool checkColor(int n, Color c)
        {
            // Проверить всех соседей
            return true;
        }

        // Есть ли уже на оси axis черный шарик
        private bool checkBlack(int axis)
        {
            return true;
        }

        // Как проверить и сделать так чтобы не было двух черных на оси

        // Написать проверку трех и более шариков подряд

        private int getAxis(int n)
        {
            for (int i = 0; i < CountEdge; i++)
            {
                foreach (var b in balls[i])
                {
                    if (b.Id == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private float dist(PointF x, PointF y)
        {
            return (float)(Math.Sqrt((x.X - y.X) * (x.X - y.X) + (x.Y - y.Y) * (x.Y - y.Y)));
        }
        private PointF Rotate(float angle, PointF a)
        {
            return new PointF((float)(a.X * Math.Cos(angle) - a.Y * Math.Sin(angle)), 
                (float)(a.X * Math.Sin(angle) + a.Y * Math.Cos(angle)));
        }
        private PointF AddDelta(PointF x, PointF delta)
        {
            return new PointF(x.X + delta.X, x.Y + delta.Y);
        }

        private void octagon(int h, PointF center, int countEdge)
        {
            float angle = (float)(Math.PI * 2 / countEdge);
            List<PointF> p = new List<PointF>();

            p.Add(new PointF(0, h));

            for (int i = 0; i < countEdge - 1; i++)
            {
                p.Add(Rotate(angle, p[p.Count - 1]));

            }
            for (int i = 0; i < countEdge - 1; i++)
            {
                var x = AddDelta(p[i], center);
                var y = AddDelta(p[i + 1], center);
                balls[i].Add(new Ball(graphics, x, Color.Gray, BallSize, N++));
                graphics.DrawLine(new Pen(Color.Black), x, y);
            }
            graphics.DrawLine(new Pen(Color.Black), AddDelta(p[0], center), AddDelta(p[countEdge - 1], center));
            var temp = AddDelta(p[countEdge - 1], center);
            balls[countEdge - 1].Add(new Ball(graphics, temp, Color.Gray, BallSize, N++));
            
        }

        private void DrawMap()
        {
            int r = Height / (3 * CountLevels);

            for (int i = 0; i < CountLevels; i++)
            {
                octagon(r * (i + 1), Center, CountEdge);
            }
        }
        private void InitMap()
        {
            for (int i = 0; i < CountLevels * CountEdge; i++)
            {
                GenerateNewBall(i);
            }
        }
    }
}



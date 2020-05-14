using System;
using System.Collections.Generic;
using System.Drawing;

namespace WindowsFormsApp3
{

    class Net
    {
        private readonly Color[] colors = new Color[6] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange, Color.Black };

        private readonly Random r;
        Graphics graphics;
        public int Height { get; set; }
        public int Width { get; set; }
        public int CountLevels {get; set;}
        public int CountEdge { get; set; }
        private float BallSize;
        private int N;

        List<Ball> balls;

        PointF Center { get; set; }

        public Net(Graphics g, int countLevels, int width, int heght, int countEdge = 8)
        {
            graphics = g;
            Height = heght;
            Width = width;
            CountEdge = countEdge;
            CountLevels = countLevels;
            Center = new PointF(Width / 2, Height / 2);
            BallSize = Height / (5 * CountLevels);
            N = 0;
            r = new Random(DateTime.Now.Millisecond);
            balls = new List<Ball>();
            DrawMap();
            InitMap();
        }

        public int GetBallNumber(float x, float y)
        {
            foreach (var b in balls)
            {
                if (Dist(new PointF(x, y), b.coord) <= BallSize / 2)
                {
                    return b.Id;
                }
            }            
            return -1;
        }
        public int GetBallNumber(PointF x)
        {
            return GetBallNumber(x.X, x.Y);
        }

        private Ball GetBall(int x)
        {
            if (x >= N) return null;
            return balls[x];
        }

        public Color GetColor(int n)
        {
            return GetBall(n).color;
        }
        public Color GetColor(float x, float y)
        {
            return GetBall(GetBallNumber(x, y)).color;
        }
        public Color GetColor(PointF x)
        {
            return GetBall(GetBallNumber(x)).color;
        }

        public void SetColor(int n, Color c)
        {
            GetBall(n).color = c;
        }
        public void SetColor(float x, float y, Color c)
        {
            GetBall(GetBallNumber(x, y)).color = c;
        }
        public void SetColor(PointF x, Color c)
        {
            GetBall(GetBallNumber(x)).color = c;
        }

        private void MoveBall(int n)
        {     
            for (int i = n; i >= CountEdge; i -= CountEdge)
            {
                balls[i].color = balls[i - CountEdge].color;
            }
            GenerateNewColor(GetAxis(n));
        }
        
        public void Swap(float x, float y, float x1, float y1)
        {
            int number1 = GetBallNumber(x, y);
            int number2 = GetBallNumber(x1, y1);

            if (number1 != -1 && number2 != -1)
            {
                var t = GetBall(number1).color;
                GetBall(number1).color = GetBall(number2).color;
                GetBall(number2).color = t;
                Get();
            }
        }

        private void GenerateNewColor(int n)
        {
            Color c = colors[r.Next(6)];
            while (!CheckColor(n, c))
            {
                c = colors[r.Next(6)];
            }
            GetBall(n).color = c;
        }
        
        private bool CheckColor(int n, Color c)
        {
            int k = 0;
            foreach (var i in GetNeighors(n))
            {
                if (GetBall(i).color == c)
                {
                    k++;
                }
            }
            return (k < 2) & (!(c == Color.Black && CheckBlack(n)));
        }
        
        // Есть ли на оси с шариком номер n еще один черный шарик
        private bool CheckBlack(int n)
        {
            int ax = GetAxis(n);
            for (int i = ax; i < N; i += CountEdge)
            {
                if (GetBall(i).color == Color.Black)
                {
                    return true;
                }
            }
            return false;
        }

        // Обход в глубину для поиска компонент связанности одного цвета
        public void Get()
        {
            List<int> l = new List<int>(4);
            bool flag = true;
            while (flag)
            {
                flag = false;
                for (int i = 0; i < N; i++)
                {
                    l.Clear();
                    AddNeigbors(i, l);
                    if (l.Count > 2)
                    {                        
                        SortCenter(l);
                        for(int j = 0; j < l.Count; j++) { 
                            MoveBall(l[j]);
                        }
                        flag = true;
                        break;
                    }
                }
            }
            DrawBalls();
        }
        private void AddNeigbors(int x, List<int> l)
        {
            l.Add(x);
            foreach (var i in GetNeighors(x))
            {
                if (GetBall(i).color == GetBall(x).color && !l.Contains(i))
                {
                    AddNeigbors(i, l);
                }
            }            
        }
        //
        // Сортировка пузырьком для шариков, по дальность удаления от центра
        private void SortCenter(List<int> l)
        {
            for (int i = 0; i < l.Count; i++)
            {
                for (int j = 0; j < l.Count - 1; j++)
                {
                    if (l[j] / CountEdge > l[j + 1] / CountEdge)
                    {
                        var t = l[j];
                        l[j] = l[j + 1];
                        l[j + 1] = t;
                    }
                }
            }
        }

        private List<int> GetNeighors(int n)
        {
            List<int> res = new List<int>();
            int level = n / CountEdge;
            if ((n + 1) / CountEdge != level)
            {
                res.Add(level * CountEdge);
            }
            else
            {
                res.Add(n + 1);
            }
            if ((n - 1) / CountEdge != level || n - 1 < 0)
            {
                res.Add((level + 1) * CountEdge - 1);
            }
            else
            {
                res.Add(n - 1);
            }
            if (n + CountEdge < N)
            {
                res.Add(n + CountEdge);
            }
            if (n - CountEdge >= 0)
            {
                res.Add(n - CountEdge);
            }
            return res;
        }

        private int GetAxis(int n)
        {
            return n % CountEdge;
        }

        private float Dist(PointF x, PointF y)
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

        private void Octagon(int h, PointF center, int countEdge)
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
                balls.Add(new Ball(graphics, x, Color.Gray, BallSize, N++));
                graphics.DrawLine(new Pen(Color.Black), x, y);
            }
            graphics.DrawLine(new Pen(Color.Black), AddDelta(p[0], center), AddDelta(p[countEdge - 1], center));
            var temp = AddDelta(p[countEdge - 1], center);
            balls.Add(new Ball(graphics, temp, Color.Gray, BallSize, N++));            
        }

        private void DrawMap()
        {
            int r = Height / CountLevels / 3;

            for (int i = 0; i < CountLevels; i++)
            {
                Octagon(r * (i + 1), Center, CountEdge);
            }
            for (int i = 0; i < CountEdge; i++)
            {
                for (int j = i + CountEdge; j < N; j += CountEdge)
                {
                    graphics.DrawLine(new Pen(Color.Black), balls[j].coord, balls[j - CountEdge].coord);
                }
            }
        }
        private void InitMap()
        {
            for (int i = 0; i < N; i++)
            {
                GenerateNewColor(i);
            }
            Get();
        }
        private void DrawBalls()
        {
            foreach (var i in balls)
            {
                i.Draw();
            }
        }        
    }
}



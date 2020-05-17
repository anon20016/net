using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp3
{

    class Net
    {
        public delegate void RefreshScore(int s);
        public event RefreshScore onScoreChaged;

        private readonly Color[] colors = new Color[10] { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange, Color.Violet, Color.Brown, Color.GreenYellow, Color.DeepPink };

        private readonly Random r;
        Graphics graphics;
        public int Height { get; set; }
        public int Width { get; set; }
        public int CountLevels {get; set;}
        public int CountEdge { get; set; }
        public int CountColors { get; set; }
        private float BallSize;
        private int N;
        private readonly GameEndHandler gameEndHandler;

        PictureBox pb;

        List<Ball> balls;

        PointF Center { get; set; }

        /// <summary>
        /// Генерация новой игры с полем
        /// </summary>
        /// <param name="g">класс Graphics, где отрисовывается поле</param>
        /// <param name="countLevels">Количество слоём многоугольников</param>
        /// <param name="width">Ширина поля для отрисовки</param>
        /// <param name="heght">Высота поля для отрисовки</param>
        /// <param name="picture">PictureBox для обновления состояний</param>
        /// <param name="countEdge">Количесвто углов в многоугольнике</param>
        /// <param name="numColors">Количество цветов в игре (5 - 10)</param>
        /// <param name="black">Флаг присутствия черного шарика в игре</param>
        public Net(Graphics g, int countLevels, int width, int heght, PictureBox picture, int countEdge = 8, int numColors = 6, bool black = true)
        {
            if (!black)
            {
                colors[0] = Color.Chocolate;
            }
            graphics = g;
            Height = heght;
            Width = width;
            CountEdge = countEdge;
            CountLevels = countLevels;
            CountColors = (numColors < 5 || numColors > 10) ? 5 : numColors;
            Center = new PointF(Width / 2, Height / 2);
            BallSize = Height / (5 * CountLevels);
            N = 0;
            r = new Random(DateTime.Now.Millisecond);
            balls = new List<Ball>();
            pb = picture;
            gameEndHandler = new GameEndHandler(CountEdge);
            gameEndHandler.OnGameEnded += GameEnd;
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

        public Ball GetBall(int x)
        {
            if (x >= N) return null;
            return balls[x];
        }

        private Color GetColor(int n)
        {
            return GetBall(n).color;
        }
        private Color GetColor(float x, float y)
        {
            return GetBall(GetBallNumber(x, y)).color;
        }
        private Color GetColor(PointF x)
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
            if (GetColor(n) == Color.Black)
            {
                gameEndHandler.addAxisDone(GetAxis(n));
            }
            for (int i = n; i >= CountEdge; i -= CountEdge)
            {
                balls[i].color = balls[i - CountEdge].color;
            }
            GenerateNewColor(GetAxis(n));
        }
        
        public bool Swap(float x, float y, float x1, float y1, int sleep = 500)
        {
            int number1 = GetBallNumber(x, y);
            int number2 = GetBallNumber(x1, y1);

            if (number1 == -1 || number2 == -1)
            {
                return false;
            }

            if (GetColor(number1) == Color.Black || GetColor(number2) == Color.Black)
            {
                return false;
            }

            if (isNeighor(number1, number2))
            {
                var t = GetBall(number1).color;
                GetBall(number1).color = GetBall(number2).color;
                GetBall(number2).color = t;
                DrawBalls(sleep / 2);

                int addscore = MapResearch(sleep);
                if (addscore == 0)
                {
                    t = GetBall(number1).color;
                    GetBall(number1).color = GetBall(number2).color;
                    GetBall(number2).color = t;
                    DrawBalls(500);
                }
                else
                {
                    onScoreChaged?.Invoke(addscore);
                    return true;
                }
            }
            return false;
        }

        private bool isNeighor(int number1, int number2)
        {
            return GetNeighors(number1).Contains(number2);
        }

        /// <summary>
        /// Генерирует случайный цвет для шарика с номером n
        /// </summary>
        /// <param name="n">Номер шарика</param>
        /// <param name="canBlack">Нужно ли ставить черный шарик, 1 - не нужно, 0 - нужно</param>
        private void GenerateNewColor(int n, int canBlack = 1)
        {
            Color c = colors[canBlack + r.Next(CountColors - canBlack)];
            while (!CheckColor(n, c))
            {
                c = colors[r.Next(CountColors)];
            }
            GetBall(n).color = c;
        }
        
        private bool CheckColor(int n, Color c)
        {
            int k = 0;
            if (c == Color.Black)
            {
                if (!gameEndHandler.canAddAxis(GetAxis(n))){
                    return false;
                }
                else
                {
                    gameEndHandler.addAxisAll(GetAxis(n));
                    return true;
                }
            }
            foreach (var i in GetNeighors(n))
            {
                if (GetBall(i).color == c)
                {
                    k++;
                }
            }
            return (k < 2);
        }
        
        // Обход в глубину для поиска компонент связанности одного цвета
        public int MapResearch(int sleep = 0)
        {
            List<int> l = new List<int>(4);
            bool flag = true;
            int count = 0;
            while (flag)
            {
                flag = false;
                for (int i = 0; i < N; i++)
                {
                    if (i / CountEdge == CountLevels - 1 && GetColor(i) == Color.Black)
                    {
                        MoveBall(i);
                        flag = true;
                        break;
                    }
                    l.Clear();
                    AddNeigbors(i, l);
                    if (l.Count > 2)
                    {
                        SortCenter(l);
                        for (int j = 0; j < l.Count; j++)
                        {
                            MoveBall(l[j]);
                        }
                        flag = true;
                        count += l.Count;
                    }
                    
                }
                DrawBalls(sleep);
            }
            return count;
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

        /// <summary>
        /// Полчить список всех соседей данного шарика
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
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
                GenerateNewColor(i, 0);
            }
            MapResearch();           
        }
        private void DrawBalls(int sleep = 0, List<int> update = null)
        {
            if (update == null)
            {
                foreach (var i in balls)
                {
                    i.Draw();
                }
            }
            else
            {
                foreach(var i in update)
                {
                    GetBall(i).Draw();
                }
            }
            pb.Refresh();
            Thread.Sleep(sleep);
        }        

        private void GameEnd()
        {
            graphics.DrawString("You Win!!", new Font("Arial", 100), new SolidBrush(Color.Black), new PointF(0, 400));
            pb.Refresh();
        }
    }
}

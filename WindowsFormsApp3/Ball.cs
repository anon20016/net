using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    class Ball
    {
        public int Id { get; set; }
        Graphics graphics;
        public PointF coord { get; set; }
        private Color _color;
        public Color color
        {
            get { return _color; }
            set
            {
                _color = value;
                Draw();
            }

        }
        public float Radius { get; set; }

        public Ball(Graphics g, PointF p, Color c, float radius, int id)
        {
            Id = id;
            graphics = g;
            coord = p;
            Radius = radius;
            color = c;
        }

        private void Draw()
        {
            graphics.FillEllipse(new SolidBrush(color), coord.X - Radius / 2, coord.Y - Radius / 2, Radius, Radius);
        }

    }
}

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
        public Color color { get; set; }        
        public float Radius { get; set; }

        public Ball(Graphics g, PointF p, Color c, float radius, int id)
        {
            Id = id;
            graphics = g;
            coord = p;
            Radius = radius;
            color = c;
        }

        public void Draw()
        {
            graphics.FillEllipse(new SolidBrush(color), coord.X - Radius / 2, coord.Y - Radius / 2, Radius, Radius);
            //graphics.DrawString(Id.ToString(), new Font("Arial", 15), new SolidBrush(Color.Indigo), coord);
        }

        public void Select()
        {
            graphics.DrawEllipse(new Pen(Color.White, 6), coord.X - Radius / 2 + 3, coord.Y - Radius / 2 + 3, Radius - 6, Radius - 6);
            //graphics.DrawString(Id.ToString(), new Font("Arial", 15), new SolidBrush(Color.Indigo), coord);
        }
        public void DeSelect()
        {
            graphics.DrawEllipse(new Pen(color, 6), coord.X - Radius / 2 + 3, coord.Y - Radius / 2 + 3, Radius - 6, Radius - 6);
            //graphics.DrawString(Id.ToString(), new Font("Arial", 15), new SolidBrush(Color.Indigo), coord);
        }
    }
}

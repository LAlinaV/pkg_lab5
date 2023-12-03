using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pkglab5
{
    struct Vec
    {
        public Vec(KeyValuePair<PointF, PointF> a)
        {
            A = a.Key.X - a.Value.X;
            B = a.Key.Y - a.Value.Y;
        }
        public Vec(float Vx, float Vy)
        {
            this.A = Vx;
            this.B = Vy;
        }
        public Vec(PointF begin, PointF end)
        {
            A = begin.X - end.X;
            B = begin.Y - end.Y;
        }
        public float A;
        public float B;
    }

    struct RectangleClipper
    {
        public RectangleClipper(Point min, Point max)
        {
            pMin = min;
            pMax = max;
        }
        public RectangleClipper(float x_min, float y_min, float x_max, float y_max)
        {
            pMin = new PointF(x_min, y_min);
            pMax = new PointF(x_max, y_max);
        }
        public PointF pMin;
        public PointF pMax;
    }

    class Clipper
    {
        public void SetRectangleClipper(float x_min, float y_min, float x_max, float y_max)
        {
            rect = new RectangleClipper(x_min, y_min, x_max, y_max);
        }
        public void SetRectangleClipper(Point min, Point max)
        {
            rect = new RectangleClipper(min, max);
        }
        public bool LiangBarski(PointF p1, PointF p2, ref float t_enter, ref float t_outer)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            float t_min = 0.0f;
            float t_max = 1.0f;

            float[] p = { -dx, dx, -dy, dy };
            float[] q = { p1.X - rect.pMin.X, rect.pMax.X - p1.X, p1.Y - rect.pMin.Y, rect.pMax.Y - p1.Y };

            for (int i = 0; i < 4; i++)
            {
                if (p[i] == 0)
                {
                    if (q[i] < 0)
                    {
                        return false;  // Line is parallel and outside the window
                    }
                }
                else
                {
                    float t = q[i] / p[i];
                    if (p[i] < 0)
                    {
                        t_min = Math.Max(t, t_min);
                    }
                    else
                    {
                        t_max = Math.Min(t, t_max);
                    }
                }
            }

            if (t_min > t_max)
            {
                return false;  // Line is outside the window
            }

            t_enter = t_min;
            t_outer = t_max;
            return true;  // Line is inside or intersects the window
        }
        public RectangleClipper rect;
        public float ScalarMultiply(Vec vec1, Vec vec2)
        {
            return (vec1.A * vec2.A + vec1.B * vec2.B);
        }

        public float VectorMultiply(Vec vec1, Vec vec2)
        {
            return vec1.A * vec2.B - vec1.B * vec2.A;
        }
    }
}

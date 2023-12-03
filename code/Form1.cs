using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pkglab5
{
    public partial class Form1 : Form
    {
        enum mode
        {
            LiangBarski,
            Polugon
        }
        mode curMode = mode.LiangBarski;
        List<KeyValuePair<PointF, PointF>> segmentsForLiangBarski;
        List<KeyValuePair<PointF, PointF>> segmentsForPolygon;
        private int scale = 30;
        private int shift = 15;
        Clipper clipper = new Clipper();
        public Form1()
        {
            InitializeComponent();
            
            clipper.rect1 = new RectangleClipper(-1, -3, 5, 4);
            clipper.SetRectangleClipper1(-1, -3, 5, 4);
            clipper.rect2 = new RectangleClipper(-1, -3, 5, 4);
            clipper.SetRectangleClipper2(-1, -3, 5, 4);
            numericUpDown1.Value = scale;
            segmentsForLiangBarski = readRectangle("liang.txt");
            segmentsForPolygon = readPolygon("polygon.txt");
        }

        public void drawMarkup(Graphics gr, Panel panel)
        {
            Pen pen_digits = new Pen(Color.DarkBlue, 1);
            var cx = panel.Width / 2;
            var cy = panel.Height / 2;
            int cur_x = 0, cur_y = 0;
            Font font = new Font("Arial", scale / 3);
            int counter = 0;
            gr.DrawString(counter.ToString(), font, pen_digits.Brush, new PointF(cx, cy));
            while (cx - cur_x >= 0)
            {
                counter++;
                cur_x += scale;
                PointF pr = new PointF(cx + cur_x, cy);
                PointF pl = new PointF(cx - cur_x, cy);
                gr.DrawString(counter.ToString(), font, pen_digits.Brush, pr);
                gr.DrawString("-" + counter.ToString(), font, pen_digits.Brush, pl);
            }
            counter = 0;
            while (cy - cur_y >= 0)
            {
                counter++;
                cur_y += scale;
                PointF pl = new PointF(cx, cy + cur_y);
                PointF pr = new PointF(cx, cy - cur_y);
                gr.DrawString(counter.ToString(), font, pen_digits.Brush, pr);
                gr.DrawString("-" + counter.ToString(), font, pen_digits.Brush, pl);

            }
            Pen penR = new Pen(Color.DarkBlue, 2);
            gr.DrawLine(penR, cx, 0, cx, panel.Height);
            gr.DrawLine(penR, 0, cy, panel.Width, cy);
            PointF[] x_vec = new PointF[] { new PointF(cx, 0), new PointF(cx - 2, 5), new PointF(cx + 2, 5) };
            PointF[] y_vec = new PointF[] { new PointF(panel.Width - 1, cy), new PointF(panel.Width - 6, cy - 2), new PointF(panel.Width - 6, cy + 2) };
            gr.DrawPolygon(penR, x_vec);
            gr.DrawPolygon(penR, y_vec);
            gr.DrawString("x", new Font("Arial", scale / 2), penR.Brush, new PointF(panel.Width - 20, cy - 20));
            gr.DrawString("y", new Font("Arial", scale / 2), penR.Brush, new PointF(cx - 15, 0));

        }
        public void drawPlot(Graphics gr, Panel panel)
        {
            Pen pen = new Pen(Color.Gray, 1);
            Pen pen_dash = new Pen(Color.LightGray, 1);
            pen_dash.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            gr.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            var cx = panel.Width / 2;
            var cy = panel.Height / 2;
            int cur_x = 0, cur_y = 0;
            while (cx - cur_x >= 0)
            {
                gr.DrawLine(pen_dash, cx - cur_x, 0, cx - cur_x, panel.Height);
                gr.DrawLine(pen_dash, cx + cur_x, 0, cx + cur_x, panel.Height);
                cur_x += scale;
            }
            while (cy - cur_y >= 0)
            {
                gr.DrawLine(pen_dash, 0, cy - cur_y, panel.Width, cy - cur_y);
                gr.DrawLine(pen_dash, 0, cy + cur_y, panel.Width, cy + cur_y);
                cur_y += scale;

            }
        }

        public Point toPanelCoordinates(Panel panel, PointF point)
        {
            var cx = panel.Width / 2;
            var cy = panel.Height / 2;
            return new Point((int)Math.Round(point.X * (float)scale) + cx, -(int)Math.Round(point.Y * (float)scale) + cy);
        }

        public void drawRectangle1(Graphics gr, Panel panel)
        {
            Pen pen = new Pen(Color.Indigo, (int)Math.Floor((decimal)shift / 3));
            var a = (int)Math.Round((clipper.rect1.pMax.X - clipper.rect1.pMin.X) * (float)scale);
            var b = (int)Math.Round((clipper.rect1.pMax.Y - clipper.rect1.pMin.Y) * (float)scale);
            var cx = panel.Width / 2;
            var cy = panel.Height / 2;
            gr.DrawRectangle(pen, new Rectangle((int)Math.Round(clipper.rect1.pMin.X * (float)scale) + cx, -(int)Math.Round(clipper.rect1.pMax.Y * (float)scale) + cy, a, b));
        }

        public void drawRectangle2(Graphics gr, Panel panel)
        {
            Pen pen = new Pen(Color.Indigo, (int)Math.Floor((decimal)shift / 3));
            var a = (int)Math.Round((clipper.rect2.pMax.X - clipper.rect2.pMin.X) * (float)scale);
            var b = (int)Math.Round((clipper.rect2.pMax.Y - clipper.rect2.pMin.Y) * (float)scale);
            var cx = panel.Width / 2;
            var cy = panel.Height / 2;
            gr.DrawRectangle(pen, new Rectangle((int)Math.Round(clipper.rect2.pMin.X * (float)scale) + cx, -(int)Math.Round(clipper.rect2.pMax.Y * (float)scale) + cy, a, b));
        }

        public List<KeyValuePair<PointF, PointF>> readRectangle(string file)
        {
            List<KeyValuePair<PointF, PointF>> list = new List<KeyValuePair<PointF, PointF>>();
            StreamReader r = new StreamReader(file);
            int num = int.Parse(r.ReadLine());
            for (int i = 0; i < num; i++)
            {
                var str_ = r.ReadLine().Split();
                list.Add(new KeyValuePair<PointF, PointF>(new PointF(float.Parse(str_[0]), float.Parse(str_[1])), new PointF(float.Parse(str_[2]), float.Parse(str_[3]))));
            }
            var str = r.ReadLine().Split();
            r.Close();
            clipper.SetRectangleClipper1(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]), float.Parse(str[3]));
            return list;
        }

        public List<KeyValuePair<PointF, PointF>> readPolygon(string file)
        {
            List<KeyValuePair<PointF, PointF>> polygon = new List<KeyValuePair<PointF, PointF>>();
            StreamReader r = new StreamReader(file);
            int num = int.Parse(r.ReadLine());
            for (int i = 0; i < num; i++)
            {
                var str_ = r.ReadLine().Split();
                polygon.Add(new KeyValuePair<PointF, PointF>(new PointF(float.Parse(str_[0]), float.Parse(str_[1])), new PointF(float.Parse(str_[2]), float.Parse(str_[3]))));
            }
            var str = r.ReadLine().Split();
            r.Close();
            clipper.SetRectangleClipper2(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]), float.Parse(str[3]));
            return polygon;
        }


        public void clipperSegmentsLiangBarski(Graphics gr, Panel panel, Pen entering, Pen outering)
        {
            float t_1 = -1, t_2 = -1;
            for (int i = 0; i < segmentsForLiangBarski.Count; i++)
            {
                drawSegment(gr, panel, segmentsForLiangBarski[i].Key, segmentsForLiangBarski[i].Value, outering);
                if (clipper.LiangBarski(segmentsForLiangBarski[i].Key, segmentsForLiangBarski[i].Value, ref t_1, ref t_2))
                {
                    drawParametricSegment(gr, panel, segmentsForLiangBarski[i].Key, segmentsForLiangBarski[i].Value, t_1, t_2, entering);
                }
            }

        }
        public void clipperSegmentsPolygon(Graphics gr, Panel panel, Pen entering, Pen outering)
        {
            float t_1 = -1, t_2 = -1;
            for (int i = 0; i < segmentsForPolygon.Count; i++)
            {
                drawSegment(gr, panel, segmentsForPolygon[i].Key, segmentsForPolygon[i].Value, outering);
                if (clipper.LiangBarskiPolygon(segmentsForPolygon[i].Key, segmentsForPolygon[i].Value, ref t_1, ref t_2))
                {
                    drawParametricSegment(gr, panel, segmentsForPolygon[i].Key, segmentsForPolygon[i].Value, t_1, t_2, entering);
                }
            }
        }

        public void drawSegment(Graphics gr, Panel panel, PointF begin, PointF end, Pen pen)
        {
            gr.DrawLine(pen, toPanelCoordinates(panel1, begin), toPanelCoordinates(panel1, end));
        }

        public void drawParametricSegment(Graphics gr, Panel panel, PointF begin, PointF end, float t_1, float t_2, Pen pen)
        {
            if (t_1 < 0 || t_1 > 1 || t_2 < 0 || t_2 > 1)
            {
                return;
            }

            PointF p1 = new PointF(begin.X + t_1 * (end.X - begin.X), begin.Y + t_1 * (end.Y - begin.Y));
            PointF p2 = new PointF(begin.X + t_2 * (end.X - begin.X), begin.Y + t_2 * (end.Y - begin.Y));
            drawSegment(gr, panel, p1, p2, pen);
            Point p1_ = toPanelCoordinates(panel1, p1);
            Point p2_ = toPanelCoordinates(panel1, p2);
            var rect1 = new Rectangle(p1_.X - 1, p1_.Y - 1, 2, 2);

            var rect2 = new Rectangle(p2_.X - 1, p2_.Y - 1, 2, 2);
            Pen dots = new Pen(Color.Red, 3);
            gr.DrawRectangle(dots, rect1);
            gr.DrawRectangle(dots, rect2);
            gr.FillRectangle(dots.Brush, rect1);

            gr.FillRectangle(dots.Brush, rect2);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            drawMarkup(gr, panel1);
            drawPlot(gr, panel1);
            Pen entering = new Pen(Color.MintCream, shift / 4 + 2);
            Pen outering = new Pen(Color.Lime, shift / 4 + 1);
            if (curMode == mode.LiangBarski)
            {
                drawRectangle1(gr, panel1);
                clipperSegmentsLiangBarski(gr, panel1, entering, outering);
            }
            else
            {
                drawRectangle2(gr, panel1);
                clipperSegmentsPolygon(gr, panel1, entering, outering);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            scale = (int)numericUpDown1.Value;
            shift = (int)Math.Floor((decimal)scale / 2);
            panel1.Invalidate();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                curMode = mode.LiangBarski;
            }
            panel1.Invalidate();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                curMode = mode.Polugon;
            }
            panel1.Invalidate();
        }
    }
}

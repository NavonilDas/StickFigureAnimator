using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StickFigureDemo
{
    class Face
    {
        public int x;
        public int y;
        public int r;
        Rectangle myface;
        SolidBrush sb;
        public Face(int sx, int sy, int r)
        {
            sb = new SolidBrush(Color.Black);
            this.r = r;
            x = sx - r;
            y = sy - (2 * r);
            myface = new Rectangle(x, y, 2 * r, 2 * r);
        }
        public void UpdateCord(int sx, int sy)
        {
            x = sx - r;
            y = sy - (2 * r);
            myface.X = x;
            myface.Y = y;
        }
        public void Draw(Graphics g)
        {
            g.FillEllipse(sb, myface);
        }
    }
    class Line
    {
        public float sx, sy, ex = 0, ey = 0, len;
        public double ang;
        Pen p;
        public Line(float sx, float sy, float len, double ang)
        {
            this.ang = ang;

            this.sx = sx;
            this.sy = sy;
            this.len = len;
            Update();
            p = new Pen(Color.Black, 8);
            p.StartCap = p.EndCap = LineCap.Round;

        }
        public void Update()
        {
            ex = sx + len * (float)Math.Cos(ang);
            ey = sy + len * (float)Math.Sin(ang);
        }
        public void Draw(Graphics g)
        {
            Update();
            g.DrawLine(p, new PointF(sx, sy), new PointF(ex, ey));
        }

    }
    class Pivot
    {
        public int x, y;
        SolidBrush sb;
        Rectangle myPivot;
        public int r = 5;
        Line l;
        public Pivot(int x, int y, ref Line l)
        {
            this.l = l;
            this.x = x - r;
            this.y = y - r;
            sb = new SolidBrush(Color.Red);
            myPivot = new Rectangle(this.x, this.y, 2 * r, 2 * r);
        }
        public void move(Point p)
        {
            double ang = Math.Atan2(p.Y - l.sy, p.X - l.sx);
            l.ang = ang;
            l.Update();
            x = (int)l.ex - r;
            y = (int)l.ey - r;
            myPivot.X = x;
            myPivot.Y = y;
        }
        public void check()
        {
            x = (int)l.ex - r;
            y = (int)l.ey - r;
            myPivot.X = x;
            myPivot.Y = y;
        }
        public void Draw(Graphics g)
        {
            g.FillEllipse(sb, myPivot);
        }
    }
    // Origin Pivot
    class PivotO
    {
        public int x, y;
        SolidBrush sb;
        Rectangle myPivot;
        public int r = 6;
        public PivotO(int sx, int sy)
        {
            x = sx - r;
            y = sy - r;
            sb = new SolidBrush(Color.FromArgb(0, 255, 0));
            myPivot = new Rectangle(this.x, this.y, 2 * r, 2 * r);
        }
        public void Move(Point p)
        {
            x = p.X - r;
            y = p.Y - r;
            myPivot.X = x;
            myPivot.Y = y;
        }
        public void Draw(Graphics g)
        {
            g.FillEllipse(sb, myPivot);
        }
    }
    class DrawingClass : Control
    {
        int i = 0;
        Face f;
        List<Line> lines;
        List<Pivot> pivots;
        PivotO piv;
        public String TmpPath = "";
        Bitmap prevFrame = null;

        void DrawLine(int ind, int ang)
        {
            Line tmp = lines[ind];
            lines.Add(new Line(tmp.ex, tmp.ey, 60, Radians(ang)));
        }
        public DrawingClass()
        {
            TmpPath = Directory.GetCurrentDirectory() + "\\Tmp";
            if (!Directory.Exists(TmpPath)) {
                // Create The Directory if not exist
                Directory.CreateDirectory(TmpPath);
            }
            lines = new List<Line>();
            pivots = new List<Pivot>();
            piv = new PivotO(100, 100);
            Line tmp;

            f = new Face(100, 100, 20);

            lines.Add(new Line(100f, 100f, 60, Radians(90))); // 0  chest
            lines.Add(new Line(100f, 100f, 60, Radians(30)));// 1 rhand
            lines.Add(new Line(100f, 100f, 60, Radians(150))); // 2 lhand

            DrawLine(0, 30);// 3 rleg
            DrawLine(0, 150); // 4 lleg
            DrawLine(1, 270);// 5 rhand wrist
            DrawLine(2, 270);// 6 lhand wrist
            DrawLine(3, 90);// 7 lleg foot
            DrawLine(4, 90);// 8 rleg foot
            for (int i = 0; i <= 8; i++)
            {
                tmp = lines[i];
                pivots.Add(new Pivot((int)tmp.ex, (int)tmp.ey, ref tmp));
            }

            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }
        double Radians(double val)
        {
            return ((val * Math.PI) / 180);
        }
        void UpdateLine(Line l1, Line l2)
        {
            l2.sx = l1.ex;
            l2.sy = l1.ey;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (prevFrame == null)
                e.Graphics.Clear(Color.White);
            else
            {
                e.Graphics.DrawImage(prevFrame, new Point(0, 0));
            }
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            f.Draw(e.Graphics);

            UpdateLine(lines[0], lines[3]);
            UpdateLine(lines[0], lines[4]);
            UpdateLine(lines[1], lines[5]);
            UpdateLine(lines[2], lines[6]);
            UpdateLine(lines[3], lines[7]);
            UpdateLine(lines[4], lines[8]);


            foreach (var x in lines) x.Draw(e.Graphics);

            //l1.Draw(e.Graphics);
            foreach (var x in pivots)
            {
                x.check();
                x.Draw(e.Graphics);
            }
            piv.Draw(e.Graphics);
        }
        bool moving = false;
        Pivot selP = null;
        PivotO selPo = null;
        bool DistanceBtw(Point p, Pivot joint)
        {
            bool ret = false;
            double x = p.X - joint.x;
            double y = p.Y - joint.y;
            x *= x;
            y *= y;
            if (Math.Sqrt(x + y) <= 2 * joint.r) ret = true;
            return ret;
        }
        bool DistanceBtwO(Point p, PivotO joint)
        {
            bool ret = false;
            double x = p.X - joint.x;
            double y = p.Y - joint.y;
            x *= x;
            y *= y;
            if (Math.Sqrt(x + y) <= 2 * joint.r) ret = true;
            return ret;
        }
        public void SaveMyDrawing()
        {
            Bitmap bmp = new Bitmap(500, 500);
            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.HighQuality;
            f.Draw(g);
            foreach (var x in lines) x.Draw(g);
            bmp.Save(TmpPath + "\\" + i + ".png");
            i++;
            prevFrame = CreateTransparent(bmp, 0.6f);
            Refresh();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            moving = true;
            if (DistanceBtwO(e.Location, piv)) selPo = piv;
            for (int i = 0; i < pivots.Count; i++) if (DistanceBtw(e.Location, pivots[i])) selP = pivots[i];
        }

        void UpdateStartingOfLine(Line l, int x, int y)
        {
            l.sx = x;
            l.sy = y;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (moving && selP != null)
            {
                selP.move(e.Location);

                Refresh();
            }
            if (moving && selPo != null)
            {
                selPo.Move(e.Location);
                f.UpdateCord(e.X, e.Y);
                UpdateStartingOfLine(lines[0], e.X, e.Y);
                UpdateStartingOfLine(lines[1], e.X, e.Y);
                UpdateStartingOfLine(lines[2], e.X, e.Y);
                Refresh();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            moving = false;
            selP = null;
            selPo = null;
        }

        Bitmap CreateTransparent(Bitmap bmp, float opacity)
        {
            Bitmap ret = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(ret))
            {
                ColorMatrix mat = new ColorMatrix();
                mat.Matrix33 = opacity;

                ImageAttributes attr = new ImageAttributes();
                attr.SetColorMatrix(mat, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.Clear(Color.White);
                g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attr);
            }
            return ret;
        }
    }
}

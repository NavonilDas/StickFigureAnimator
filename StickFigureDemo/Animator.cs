using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace StickFigureDemo
{
    class Animator:Control
    {
        
        Bitmap[] images;
        int i = 0,len;
        Timer t;
        public Animator(string path,int interval)
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            string[] paths = Directory.GetFiles(path);
            len = paths.Length;
            images = new Bitmap[len];
            
            for(int i = 0; i < len; i++)
            {
                images[i] = new Bitmap(paths[i]);
            }

            t = new Timer();
            t.Interval = interval;
            t.Tick += new EventHandler(t_ticked);
            t.Start();
        }
        public void ResetTimer(int interval)
        {
            i = 0;
            t.Stop();
            t.Interval = interval;
            t.Stop();
        }

        private void t_ticked(object sender, EventArgs e)
        {
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (i < len)
            {
                e.Graphics.Clear(Color.White);
                e.Graphics.DrawImage(images[i], new Point(0, 0));
            }
            i++;
            if (i >= len) t.Stop();
        }
        public void Restart()
        {
            i = 0;
            t.Start();
        }
        public void Close()
        {
            t.Stop();
        }
        public void Start()
        {
            if (i >= len) i = 0;
            t.Start();
        }
        public void Clear()
        {
            for (int i = 0; i < images.Length; ++i) {
                images[i].Dispose();
            }
            images = null;
        }
    }
}

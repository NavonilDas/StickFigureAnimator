using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StickFigureDemo
{
    public partial class Form1 : Form
    {
        DrawingClass dc;
        public Form1()
        {
            InitializeComponent();
            dc = new DrawingClass();
            dc.Size = new Size(500, 500);
            dc.Location = new Point(0, 0);
//            this.Size = new Size(700,500);
            this.panel1.Controls.Add(dc);
//            Controls.Add(dc);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dc.SaveMyDrawing();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new PlayAnimation(dc.TmpPath).Show();
        }
    }
}

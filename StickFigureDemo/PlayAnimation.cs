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
    public partial class PlayAnimation : Form
    {
        Animator am;
        public PlayAnimation(string TmpPath)
        {
            InitializeComponent();
            am = new Animator(TmpPath,100);
            am.Size = new Size(500, 500);

            Controls.Add(am);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            am.Restart();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar1.Value + "";
            am.ResetTimer((int)trackBar1.Value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            am.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            am.Start();
        }

        private void PlayAnimation_FormClosing(object sender, FormClosingEventArgs e)
        {
            am.Clear();
        }
    }
}

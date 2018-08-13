using System;
using System.Drawing;
using System.Windows.Forms;

namespace BubbleBreaker
{
    public partial class NewBreakerSet : Form
    {
        int BREAKERSET = 0;

        PictureBox[] pbBREAKERSET = new PictureBox[3];

        public NewBreakerSet(Point p, int breakerset)
        {
            BREAKERSET = breakerset;

            pbBREAKERSET[0] = pictureBox20;
            pbBREAKERSET[1] = pictureBox19;
            pbBREAKERSET[2] = pictureBox18;

            InitializeComponent();

            pictureBox1.Visible = true;

            timer1.Enabled = true;
            timer1.Start();

            this.Location = new Point(p.X - this.Size.Width / 2 - 4, p.Y - this.Size.Height / 2 - 4);

            DisegnaSCORE();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DisegnaSCORE()
        {
            string str = "000" + BREAKERSET.ToString();

            str = str.Substring(str.Length - 3, 3);

            CrtaBrojeve(str, pbBREAKERSET, ilRed32x32);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            pictureBox1.Visible = !pictureBox1.Visible;

            timer2.Enabled = true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;

            pictureBox1.Visible = !pictureBox1.Visible;

            timer1.Enabled = true;
        }

        private void CrtaBrojeve(string numero, PictureBox[] pb, ImageList il)
        {
            int i = 0;

            foreach (char c in numero)
            {
                int n = Int32.Parse(c.ToString());

                pb[i].Image = il.Images[n];

                i++;
            }
        }
    }
}
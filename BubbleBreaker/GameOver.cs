using System;
using System.Drawing;
using System.Windows.Forms;

namespace BubbleBreaker
{
    public partial class GameOver : Form
    {
        int SCORE = 0;
        int HISCORE = 0;
        bool NewHiScore = false;

        PictureBox[] pbSCORE = new PictureBox[5];
        PictureBox[] pbHISCORE = new PictureBox[5];

        public GameOver(Point p, int score, int hiscore, bool newHiScore)
        {
            InitializeComponent();

            pbHISCORE[0] = pictureBox13;
            pbHISCORE[1] = pictureBox14;
            pbHISCORE[2] = pictureBox15;
            pbHISCORE[3] = pictureBox16;
            pbHISCORE[4] = pictureBox17;

            pbSCORE[0] = pictureBox22;
            pbSCORE[1] = pictureBox21;
            pbSCORE[2] = pictureBox20;
            pbSCORE[3] = pictureBox19;
            pbSCORE[4] = pictureBox18;

            SCORE = score;
            HISCORE = hiscore;
            NewHiScore = newHiScore;

            if (NewHiScore == true)
            {
                pictureBox1.Visible = true;

                timer1.Enabled = true;
                timer1.Start();
            }

            this.Location = new Point(p.X - this.Size.Width / 2 - 4, p.Y - this.Size.Height / 2 - 4);

            DisegnaSCORE();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DisegnaSCORE()
        {
            string str = "00000" + SCORE.ToString();

            str = str.Substring(str.Length - 5, 5);

            CrtaBrojeve(str, pbSCORE, ilRed32x32);

            str = "00000" + HISCORE.ToString();

            str = str.Substring(str.Length - 5, 5);

            CrtaBrojeve(str, pbHISCORE, ilBlueNegative24x24);
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
// Simple Board Game - BubbleBreaker
//
// by Silvio Duka
//
// Last modified date: 2012-02-08
//
// Created with MS VisualStudio Comunity 2017
//
//
//
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BubbleBreaker
{
    public enum eGameTipe { STANDARD = 1, CONTINUOUS, SHIFTER, MEGASHIFT };

    public partial class MainScreen : Form
    {
        PictureBox[,] pbCelle = new PictureBox[11, 11];
        PictureBox[] pbCelleMegaShift = new PictureBox[11];

        PictureBox[] pbSCORE = new PictureBox[5];
        PictureBox[] pbHISCORE = new PictureBox[5];
        PictureBox[] pbGAMES = new PictureBox[5];

        public Hashtable htCells = new Hashtable();

        Random n;

        int sizeX = 11;
        int sizeY = 11;

        int offX = 45;
        int offY = 76 + 56;

        int sizeGrafX = 32;
        int sizeGrafY = 32;

        int[,] GameField;
        int[,] tmpGameField;
        int[,] UndoGameField;
        int[,] GameFieldSelection;
        int[,] tmpGameFieldSelection;
        int[,] GameResultsHiScore;
        int[,] GameResultsNoGames;
        int[,] GameResultsBreakerSet;

        const int GAMETYPE = 4;
        const int GAMECOLORS = 6;

        int[] MegaShift;
        int[] tmpMegaShift;

        bool GAME = false;
        int SCORE = 0;
        int HISCORE = 0;
        int NoGAMES = 0;
        int BREAKERSET = 0;

        bool shiftRight = true;
        bool shiftDown = true;

        int GameType = 1;

        int audio = 1;

        public MainScreen()
        {
            n = new Random(DateTime.Now.Millisecond);

            GameField = new int[sizeX, sizeY];
            tmpGameField = new int[sizeX, sizeY];
            UndoGameField = new int[sizeX, sizeY];
            GameFieldSelection = new int[sizeX, sizeY];
            tmpGameFieldSelection = new int[sizeX, sizeY];

            GameResultsHiScore = new int[GAMECOLORS, GAMETYPE];
            GameResultsNoGames = new int[GAMECOLORS, GAMETYPE];
            GameResultsBreakerSet = new int[GAMECOLORS, GAMETYPE];

            LoadHiScore();

            MegaShift = new int[sizeY];
            tmpMegaShift = new int[sizeY];

            InitializeComponent();

            pbCelleMegaShift[0] = pictureBox2;
            pbCelleMegaShift[1] = pictureBox3;
            pbCelleMegaShift[2] = pictureBox4;
            pbCelleMegaShift[3] = pictureBox5;
            pbCelleMegaShift[4] = pictureBox6;
            pbCelleMegaShift[5] = pictureBox7;
            pbCelleMegaShift[6] = pictureBox8;
            pbCelleMegaShift[7] = pictureBox9;
            pbCelleMegaShift[8] = pictureBox10;
            pbCelleMegaShift[9] = pictureBox11;
            pbCelleMegaShift[10] = pictureBox12;

            pbSCORE[0] = pictureBox13;
            pbSCORE[1] = pictureBox14;
            pbSCORE[2] = pictureBox15;
            pbSCORE[3] = pictureBox16;
            pbSCORE[4] = pictureBox17;

            pbHISCORE[0] = pictureBox22;
            pbHISCORE[1] = pictureBox21;
            pbHISCORE[2] = pictureBox20;
            pbHISCORE[3] = pictureBox19;
            pbHISCORE[4] = pictureBox18;

            pbGAMES[0] = pictureBox27;
            pbGAMES[1] = pictureBox26;
            pbGAMES[2] = pictureBox25;
            pbGAMES[3] = pictureBox24;
            pbGAMES[4] = pictureBox23;

            comboBox1.SelectedIndex = Properties.Settings.Default.StartGameStyle;
            comboBox2.SelectedIndex = Properties.Settings.Default.StartBreakerSet;
            audio = Properties.Settings.Default.StartAudio;

            DisegnaSCORE();

            pbZvucnik.Image = ilZvucnik.Images[Properties.Settings.Default.StartAudio];

            comboBox1.Text = comboBox1.Items[comboBox1.SelectedIndex].ToString();
            comboBox2.Text = comboBox2.Items[comboBox2.SelectedIndex].ToString();

            GameType = comboBox1.SelectedIndex + 1;

            for (int j = 0; j < sizeY; j++)
            {
                for (int i = 0; i < sizeX; i++)
                {
                    pbCelle[i, j] = new System.Windows.Forms.PictureBox();

                    pbCelle[i, j].Image = ilCell_Pune.Images[0];
                    pbCelle[i, j].Location = new System.Drawing.Point(offX + i * (sizeGrafX), offY + j * (sizeGrafY));
                    pbCelle[i, j].Name = "pictureBox1";
                    pbCelle[i, j].Size = new System.Drawing.Size(32, 32);
                    pbCelle[i, j].TabIndex = 0;
                    pbCelle[i, j].TabStop = false;
                    pbCelle[i, j].Visible = true;
                    pbCelle[i, j].MouseDown += new MouseEventHandler(Cella_MouseDown);

                    htCells.Add(pbCelle[i, j].GetHashCode(), i + j * sizeX);

                    this.Controls.Add(this.pbCelle[i, j]);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to EXIT a game?", "Exit Game?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SaveHiScore();

                Properties.Settings.Default.StartGameStyle = comboBox1.SelectedIndex;
                Properties.Settings.Default.StartBreakerSet = comboBox2.SelectedIndex;
                Properties.Settings.Default.StartAudio = audio;

                Properties.Settings.Default.Save();

                this.Close();
            }
        }

        private void GeneraGameField()
        {
            Random r = new Random(DateTime.Now.Millisecond);

            int nc = 0;

            if (comboBox2.SelectedIndex == 0) nc = 3;
            if (comboBox2.SelectedIndex == 1) nc = 4;
            if (comboBox2.SelectedIndex == 2) nc = 5;
            if (comboBox2.SelectedIndex == 3) nc = 6;
            if (comboBox2.SelectedIndex == 4) nc = 7;
            if (comboBox2.SelectedIndex == 5) nc = 8;

            for (int j = 0; j < sizeY; j++)
            {
                for (int i = 0; i < sizeX; i++)
                {
                    int n = 1 + r.Next(nc);

                    GameField[i, j] = n;
                    GameFieldSelection[i, j] = 0;
                    UndoGameField[i, j] = n;
                    tmpGameField[i, j] = 0;
                    tmpGameFieldSelection[i, j] = 0;
                }

                if (GameType == (int)eGameTipe.STANDARD || GameType == (int)eGameTipe.SHIFTER)
                {
                    MegaShift[j] = 0;
                    tmpMegaShift[j] = 1;
                }
                else
                {
                    tmpMegaShift[j] = 0;
                }
            }

            if (GameType != (int)eGameTipe.STANDARD && GameType != (int)eGameTipe.SHIFTER)
            {
                GeneraMegaShift();
            }
        }

        private void DisegnaGameField()
        {
            for (int j = 0; j < sizeY; j++)
            {
                for (int i = 0; i < sizeX; i++)
                {
                    if (GameFieldSelection[i, j] == 0)
                    {
                        if (GameField[i, j] != tmpGameField[i, j] || GameFieldSelection[i, j] != tmpGameFieldSelection[i, j])
                        {
                            pbCelle[i, j].Image = ilCell_Pune.Images[GameField[i, j]];
                        }
                    }
                    else
                    {
                        pbCelle[i, j].Image = ilCelleSel01.Images[GameField[i, j]];
                    }
                }
            }

            for (int j = 0; j < sizeY; j++)
            {
                pbCelleMegaShift[j].Image = ilCell_Pune.Images[MegaShift[j]];
            }

            tmpGameField = (int[,])GameField.Clone();
            tmpGameFieldSelection = (int[,])GameFieldSelection.Clone();
            tmpMegaShift = (int[])MegaShift.Clone();
        }

        private void DisegnaSCORE()
        {
            string str = "00000" + SCORE.ToString();

            str = str.Substring(str.Length - 5, 5);

            CrtaBrojeve(str, pbSCORE, ilBrojevi_R);

            HISCORE = GameResultsHiScore[comboBox2.SelectedIndex, comboBox1.SelectedIndex];

            str = "00000" + HISCORE.ToString();

            str = str.Substring(str.Length - 5, 5);

            CrtaBrojeve(str, pbHISCORE, ilBrojevi_B);

            NoGAMES = GameResultsNoGames[comboBox2.SelectedIndex, comboBox1.SelectedIndex];

            str = "00000" + NoGAMES.ToString();

            str = str.Substring(str.Length - 5, 5);

            CrtaBrojeve(str, pbGAMES, ilBrojevi_G);
        }

        private void CrtaBrojeve(string numero, PictureBox[] pb, ImageList il)
        { 
            int i = 0;

            foreach(char c in numero)
            {
                int n = Int32.Parse(c.ToString());

                pb[i].Image = il.Images[n];

                i++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            dr = DialogResult.Yes;

            if (GAME == true)
            {
                dr = MessageBox.Show("Do you really want to\nbegin a new game?", "New Game?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }

            if (dr == DialogResult.Yes)
            {
                GAME = true;
                SCORE = 0;
                BREAKERSET = 0;
                GameResultsNoGames[comboBox2.SelectedIndex, comboBox1.SelectedIndex]++;

                GeneraGameField();
                DisegnaGameField();
                DisegnaSCORE();

                GameType = comboBox1.SelectedIndex + 1;
            }
        }

        private void Cella_MouseDown(object sender, MouseEventArgs e)
        {
            int i = (int)htCells[sender.GetHashCode()] % sizeX;
            int j = (int)htCells[sender.GetHashCode()] / sizeX;

            if(GameField[i,j] == 0) return; 

            if (GameFieldSelection[i, j] == 0)
            {
                AzzeraSelezione();
                DisegnaGameField();
                int n = CreaSelezione(i, j);

                int selX = 0;
                int selY = 0;
                bool nasao = false;

                for (int jj = 0; jj < sizeY; jj++)
                { 
                    for(int ii = 0; ii < sizeX; ii++)
                    {
                        if (GameFieldSelection[ii, jj] == 1)
                        {
                            selX = ii;
                            selY = jj;

                            nasao = true;

                            break;
                        }
                    }

                    if (nasao == true) break;
                }

                lbSelezione.Text = (n * (n - 1)).ToString();

                lbSelezione.Location = new Point(offX + selX * sizeGrafX - lbSelezione.Size.Width, offY + selY * sizeGrafY - lbSelezione.Size.Height);

                switch (GameField[i, j])
                { 
                    case 1:
                        this.lbSelezione.ForeColor = System.Drawing.Color.Yellow;
                        break;
                    case 2:
                        this.lbSelezione.ForeColor = System.Drawing.Color.FromArgb(255, 90, 90);
                        break;
                    case 3:
                        this.lbSelezione.ForeColor = System.Drawing.Color.FromArgb(90, 220, 90);
                        break;
                    case 4:
                        this.lbSelezione.ForeColor = System.Drawing.Color.FromArgb(122,185,220);
                        break;
                    case 5:
                        this.lbSelezione.ForeColor = System.Drawing.Color.Violet;
                        break;
                    case 6:
                        this.lbSelezione.ForeColor = System.Drawing.Color.Pink;
                        break;
                    case 7:
                        this.lbSelezione.ForeColor = System.Drawing.Color.Gold;
                        break;
                    case 8:
                        this.lbSelezione.ForeColor = System.Drawing.Color.Black;
                        break;
                    default:
                        this.lbSelezione.ForeColor = System.Drawing.Color.Black;
                        break;
                }

                if (n * (n - 1) > 0)
                    lbSelezione.Visible = true;
                else
                    lbSelezione.Visible = false;
            }
            else
            {
                CancellaCelleSelezionate();

                timer3.Enabled = true;
                timer3.Start();
            }
        }

        private bool ControllaMegaShift()
        {
            bool stato = false;

            for (int j = 0; j < sizeY; j++)
            {
                if (GameField[0, j] != 0)
                {
                    stato = true;

                    break;
                }
            }

            if (stato == false)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    GameField[0, j] = MegaShift[j];
                }

                GeneraMegaShift();

                return true;
            }

            return false;
        }

        private void GeneraMegaShift()
        {
            Random r = new Random(DateTime.Now.Millisecond);

            int nMax = n.Next(9) + 2;

            int nc = 0;

            if (comboBox2.SelectedIndex == 0) nc = 3;
            if (comboBox2.SelectedIndex == 1) nc = 4;
            if (comboBox2.SelectedIndex == 2) nc = 5;
            if (comboBox2.SelectedIndex == 3) nc = 6;
            if (comboBox2.SelectedIndex == 4) nc = 7;
            if (comboBox2.SelectedIndex == 5) nc = 8;

            for (int c = 0; c < sizeY; c++)
            {
                MegaShift[c] = 0;
            }

            for (int c = 0; c < nMax; c++)
            {
                    MegaShift[sizeY - c -1] = 1 + r.Next(nc);
            }
        }

        private bool ControllaGameOver()
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY - 1; j++)
                {
                    if (GameField[i, j] == GameField[i, j + 1] && GameField[i, j] != 0) return false;
                }
            }

            for (int j = 0; j < sizeY; j++)
            {
                for (int i = 0; i < sizeX - 1; i++)
                {
                    if (GameField[i, j] == GameField[i + 1, j] && GameField[i, j] != 0) return false;
                }
            }

            return true;
        }

        private bool ControllaBreakerSet()
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY - 1; j++)
                {
                    if (GameField[i, j] != 0) return false;
                }
            }

            return true;
        }

        private void ShiftDown()
        {
            bool move = true;

            while (move == true)
            {
                move = false;

                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = sizeY - 1; j > 0; j--)
                    {
                        if (GameField[i, j] == 0 && GameField[i, j - 1] != 0)
                        {
                            move = true;
                            GameField[i, j] = GameField[i, j - 1];
                            GameField[i, j - 1] = 0;

                            DisegnaGameField();
                        }
                    }
                }
            }
        }

        private void ShiftRightCPLX()
        {
            bool move = true;

            while (move == true)
            {
                move = false;

                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = sizeX - 1; x > 0; x--)
                    {
                        if (GameField[x, y] == 0 && GameField[x - 1, y] != 0)
                        {
                            move = true;
                            GameField[x, y] = GameField[x - 1, y];
                            GameField[x - 1, y] = 0;

                            DisegnaGameField();
                        }
                    }
                }
            }
        }

        private void ShiftRight()
        {
            bool move = true;

            while (move == true)
            {
                move = false;

                for (int x = sizeX - 1; x > 0; x--)
                {
                    if (GameField[x, sizeY - 1] == 0 && GameField[x - 1, sizeY - 1] != 0)
                    {
                        move = true;

                        for (int y = 0; y < sizeY; y++)
                        {
                            GameField[x, y] = GameField[x - 1, y];
                            GameField[x - 1, y] = 0;
                        }

                        DisegnaGameField();
                    }
                }
            }
        }

        private int CreaSelezione(int i, int j)
        {
            lbSelezione.Visible = false;

            GameFieldSelection[i, j] = 1;

            int numSel = 1;
            int tmpNumSel = 0;

            while (numSel != (tmpNumSel = GeneraSelezione()))
            {
                numSel = tmpNumSel;
            }

            if (numSel == 1)
            {
                GameFieldSelection[i, j] = 0;
                numSel = 0;
            }

            DisegnaGameField();

            return numSel;
        }

        private int GeneraSelezione()
        {
            lbSelezione.Visible = false;

            for (int j = 0; j < sizeY; j++)
            {
                for (int i = 0; i < sizeX; i++)
                {
                    if (GameFieldSelection[i, j] == 0)
                    {
                        if (i > 0)
                        {
                            if (GameFieldSelection[i - 1, j] != 0 && GameField[i - 1, j] == GameField[i, j]) GameFieldSelection[i, j] = 1;

                        }

                        if (i < sizeX - 1)
                        {
                            if (GameFieldSelection[i + 1, j] != 0 && GameField[i + 1, j] == GameField[i, j]) GameFieldSelection[i, j] = 1;

                        }

                        if (j > 0)
                        {
                            if (GameFieldSelection[i, j - 1] != 0 && GameField[i, j - 1] == GameField[i, j]) GameFieldSelection[i, j] = 1;

                        }

                        if (j < sizeY - 1)
                        {
                            if (GameFieldSelection[i, j + 1] != 0 && GameField[i, j + 1] == GameField[i, j]) GameFieldSelection[i, j] = 1;

                        }
                    }
                }
            }

            int numCelleSelezionate = 0;

            for (int j = 0; j < sizeY; j++)
            {
                for (int i = 0; i < sizeX; i++)
                {
                    if (GameFieldSelection[i, j] != 0)
                    {
                        numCelleSelezionate++;
                    }
                }
            }

            return numCelleSelezionate;
        }

        private void CancellaCelleSelezionate()
        {
            lbSelezione.Visible = false;

            UndoGameField = (int[,])GameField.Clone();

            int numSel = 0;

            for (int j = 0; j < sizeY; j++)
            {
                for (int i = 0; i < sizeX; i++)
                {
                    if (GameFieldSelection[i, j] != 0)
                    {
                        int sel = GameField[i, j];

                        GameField[i, j] = 0;
                        GameFieldSelection[i, j] = 0;

                        pbCelle[i, j].Image = ilCelle_Expl.Images[sel];

                        numSel++;
                    }
                }
            }

            SCORE += numSel * (numSel - 1);

            DisegnaSCORE();

            if(audio == 1) Sound.Play("sound_1.wav", PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_ASYNC);
        }

        private void AzzeraSelezione()
        {
            for (int j = 0; j < sizeY; j++)
            {
                for (int i = 0; i < sizeX; i++)
                {
                    GameFieldSelection[i, j] = 0;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GameField = (int[,])UndoGameField.Clone();

            DisegnaGameField();
        }

        //private void Pausa(int millisecond)
        //{
        //    for (long i = 0; i < millisecond * 100000; i++) { }
        //}

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;

            if (shiftDown == true)
            {
                ShiftDown();
            }

            timer2.Enabled = true;
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            timer2.Enabled = false;

            bool megashift = false;

            if (GameType == (int)eGameTipe.STANDARD || GameType == (int)eGameTipe.CONTINUOUS)
            {
                ShiftRight();

                if (GameType == (int)eGameTipe.CONTINUOUS)
                    megashift = ControllaMegaShift();

                if (megashift == true)
                {
                    DisegnaGameField();

                    timer2.Enabled = true;
                    timer2.Start();
                }
            }

            if (GameType == (int)eGameTipe.SHIFTER || GameType == (int)eGameTipe.MEGASHIFT)
            {
                ShiftRightCPLX();

                if (GameType == (int)eGameTipe.MEGASHIFT)
                    megashift = ControllaMegaShift();

                if (megashift == true)
                {
                    DisegnaGameField();

                    timer2.Enabled = true;
                    timer2.Start();
                }
            }

            if (megashift == false)
            {
                if (ControllaGameOver() == true)
                {
                    GAME = false;

                    bool newHiScore = false;

                    if (SCORE > GameResultsHiScore[comboBox2.SelectedIndex, comboBox1.SelectedIndex])
                    {
                        if (audio == 1) Sound.Play("sound_3.wav", PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_ASYNC | PlaySoundFlags.SND_LOOP);

                        HISCORE = SCORE;

                        newHiScore = true;

                        GameResultsHiScore[comboBox2.SelectedIndex, comboBox1.SelectedIndex] = SCORE;
                        GameResultsBreakerSet[comboBox2.SelectedIndex, comboBox1.SelectedIndex] = BREAKERSET;

                        DisegnaSCORE();
                    }
                    else
                    {
                        if (audio == 1) Sound.Play("sound_2.wav", PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_ASYNC);
                    }

                    GameOver go = new GameOver(new Point(this.Location.X + offX + 360 / 2, this.Location.Y + offY + 360 / 2), SCORE, HISCORE, newHiScore);

                    go.ShowDialog();

                    go.Close();

                    if (audio == 1) Sound.Play(null, PlaySoundFlags.SND_FILENAME);
                }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Stop();
            timer3.Enabled = false;

            DisegnaGameField();

            if (ControllaBreakerSet() == true)
            {
                if (audio == 1) Sound.Play("sound_4.wav", PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_ASYNC);

                NewBreakerSet nbs = new NewBreakerSet(new Point(this.Location.X + offX + 360 / 2, this.Location.Y + offY + 360 / 2), BREAKERSET);

                nbs.ShowDialog();

                nbs.Close();

                if (audio == 1) Sound.Play(null, PlaySoundFlags.SND_FILENAME);

                NewGame();

                return;
            }

            timer1.Enabled = true;
            timer1.Start();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameType = comboBox1.SelectedIndex + 1;

            if (comboBox1.SelectedIndex >= 0 && comboBox2.SelectedIndex >= 0) DisegnaSCORE();
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox2.SelectedIndex >= 0) DisegnaSCORE();
        }

        private void NewGame()
        {
            BREAKERSET++;

            GeneraGameField();
            DisegnaGameField();
            DisegnaSCORE();
        }

        private void LoadHiScore()
        {
            string[] hs = Properties.Settings.Default.HiScore.Split(';');
            string[] ng = Properties.Settings.Default.NoGames.Split(';');
            string[] bs = Properties.Settings.Default.BreakerSet.Split(';');

            int f = 0;

            for (int j = 0; j < GAMETYPE; j++)
            {
                for (int i = 0; i < GAMECOLORS; i++)
                {
                    GameResultsHiScore[i, j] = Int32.Parse(hs[f]);
                    GameResultsNoGames[i, j] = Int32.Parse(ng[f]);
                    GameResultsBreakerSet[i, j] = Int32.Parse(bs[f]);

                    f++;
                }
            }
        }

        private void SaveHiScore()
        {
            string hs = "";
            string ng = "";
            string bs = "";

            for (int j = 0; j < GAMETYPE; j++)
            {
                for (int i = 0; i < GAMECOLORS; i++)
                {
                    hs += GameResultsHiScore[i, j].ToString() + ";";
                    ng += GameResultsNoGames[i, j].ToString() + ";";
                    bs += GameResultsBreakerSet[i, j].ToString() + ";";
                }
            }

            Properties.Settings.Default.HiScore = hs.TrimEnd(';');
            Properties.Settings.Default.NoGames = ng.TrimEnd(';');
            Properties.Settings.Default.BreakerSet = bs.TrimEnd(';');

            Properties.Settings.Default.Save();
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            audio = Math.Abs(1 - audio);

            pbZvucnik.Image = ilZvucnik.Images[audio];
        }
   }

    public class Sound
    {
        public static void Play(string strFileName, PlaySoundFlags soundFlags)
        {
            PlaySound(strFileName, IntPtr.Zero, soundFlags);
            // passes to Playsound the filename and a pointer
            // to the Flag
        }

        [DllImport("winmm.dll")] //inports the winmm.dll used for sound
        private static extern bool PlaySound(string szSound, IntPtr hMod, PlaySoundFlags flags);
    }

    [Flags] //enumeration treated as a bit field or set of flags
    public enum PlaySoundFlags : int
    {

        SND_SYNC = 0x0000, /* play synchronously (default) */
        SND_ASYNC = 0x0001, /* play asynchronously */
        SND_NODEFAULT = 0x0002, /* silence (!default) if sound notfound */
        SND_LOOP = 0x0008, /* loop the sound until nextsndPlaySound */
        SND_NOSTOP = 0x0010, /* don't stop any currently playingsound */
        SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
        SND_FILENAME = 0x00020000, /* name is file name */
        SND_RESOURCE = 0x00040004 /* name is resource name or atom */
    } 
}
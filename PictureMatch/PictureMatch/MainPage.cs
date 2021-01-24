using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PictureMatch
{
    public partial class MatchingGame : Form
    {
        Random rnd = new Random();
        List<string> icons = new List<string>()
        {
            "!","!","N","N","H","H","k","k",
            "b","b","v","v","w","w","z","z"
        };

        Label firstClick, secondClick;
        Player player1 = new Player("1. Oyuncu", 0);
        Player player2 = new Player("2. Oyuncu", 0);
        string sira = "1. Oyuncu";
        bool ingame = false;
        public MatchingGame()
        {
            InitializeComponent();


        }
        private void MatchingGame_Load(object sender, EventArgs e)
        {
            tbLayoutPanelPictures.Enabled = false;
            //tbLayoutPanelPictures.Controls.Clear();
            //BAŞLA TUŞUNA TEKRAR BASINCA BUG OLUYOR, ZAMAN AYARLAMASINI YAPAMADIM :D
            //RESİMLER GÖRÜNÜRKEN TIKLANABİLİYOR
            //LİSTVİEW OLMADI ÜHÜHÜÜHÜHDSÜFGÜSDFHĞF
            

        }


        public void ShowAndHidePictures()
        {

            Task.Factory.StartNew(() =>
            {
                Label labelPictures;
                for (int i = 0; i < tbLayoutPanelPictures.Controls.Count; i++)
                {
                    labelPictures = tbLayoutPanelPictures.Controls[i] as Label;
                    if (labelPictures != null)
                    {
                        labelPictures.ForeColor = Color.RosyBrown;
                    }

                }
                Thread.Sleep(3000);
                for (int i = 0; i < tbLayoutPanelPictures.Controls.Count; i++)
                {
                    labelPictures = tbLayoutPanelPictures.Controls[i] as Label;
                    if (labelPictures != null)
                        labelPictures.ForeColor = Color.OldLace;
                }

                tbLayoutPanelPictures.Invoke(new MethodInvoker(delegate { tbLayoutPanelPictures.Enabled = true; }));
                ingame = true;
            });

        }

        private void pictureClick(object sender, EventArgs e)
        {
            if (firstClick != null && secondClick != null)
                return;

            Label clickedLabel = sender as Label;

            if (clickedLabel == null)
                return;
            if (clickedLabel.ForeColor == Color.Green || clickedLabel.ForeColor == Color.Red)
                return;
            if (sira == player1.name)
            {
                if (firstClick == null)
                {
                    firstClick = clickedLabel;
                    firstClick.ForeColor = Color.Green;
                    return;
                }
                secondClick = clickedLabel;
                secondClick.ForeColor = Color.Green;
                CheckForWinner();

                if (firstClick.Text == secondClick.Text)
                {
                    timerUser.Start();
                    firstClick = null;
                    secondClick = null;
                    player1.score++;
                    listView1.Items[0].SubItems[1].Text = player1.score.ToString();
                    userTimeControl = 10;
                }
                else
                {
                    timerGame.Start();
                    sira = player2.name;
                    userTimeControl = 10;
                }
            }
            else if (sira == player2.name)
            {
                if (firstClick == null)
                {
                    firstClick = clickedLabel;
                    firstClick.ForeColor = Color.Red;
                    return;
                }
                secondClick = clickedLabel;
                secondClick.ForeColor = Color.Red;
                CheckForWinner();

                if (firstClick.Text == secondClick.Text)
                {
                    timerUser.Start();
                    firstClick = null;
                    secondClick = null;
                    player2.score++;
                    listView1.Items[1].SubItems[1].Text = player2.score.ToString();
                    userTimeControl = 10;
                }
                else
                {
                    timerGame.Start();
                    sira = player1.name;
                    userTimeControl = 10;
                }
            }
        }

        //İconları random şekilde yerleştirme.
        private void AssignIconsToSquare()
        {
            Label label;
            int randomNumber;

            for (int i = 0; i < tbLayoutPanelPictures.Controls.Count; i++)
            {
                if (tbLayoutPanelPictures.Controls[i] is Label)
                    label = (Label)tbLayoutPanelPictures.Controls[i];
                else continue;

                randomNumber = rnd.Next(0, icons.Count);
                label.Text = icons[randomNumber];

                icons.RemoveAt(randomNumber);
            }
        }

        private void CheckForWinner()
        {
            Label label;
            for (int i = 0; i < tbLayoutPanelPictures.Controls.Count; i++)
            {
                label = tbLayoutPanelPictures.Controls[i] as Label;
                if (label != null && label.ForeColor == label.BackColor)
                    return;
            }
            MessageBox.Show("Tebrikler!");
            ingame = false;
            tbLayoutPanelPictures.Controls.Clear();
        }

        //Resimlerin görünme süresini ayarlayan Timer.
        private void timerGame_Tick(object sender, EventArgs e)
        {
            timerGame.Stop();

            firstClick.ForeColor = firstClick.BackColor;
            secondClick.ForeColor = secondClick.BackColor;

            firstClick = null;
            secondClick = null;
        }

        int pictureShowControl = 0;
        private void timerPictures_Tick(object sender, EventArgs e)
        {
            pictureShowControl++;
        }

        int userTimeControl = 10;
        private void timerUser_Tick(object sender, EventArgs e)
        {
            userTimeControl--;
            labelTimer.Text = userTimeControl.ToString();
            if (userTimeControl == 0)
            {
                timerUser.Stop();
                userTimeControl = 10;
                sira = (sira == player1.name) ? player2.name : player1.name;
                timerUser.Start();
            }

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (ingame) return;
            timerPictures.Start();
            AssignIconsToSquare();
            ShowAndHidePictures();
            listView1.Columns.Clear();
            listView1.Items.Clear();
            player1.score = 0;
            player2.score = 0;
            listView1.Columns.Add("Oyuncu", 100);
            listView1.Columns.Add("Puan", 75);
            listView1.Items.Add(new ListViewItem(new string[] { "Oyuncu 1", "0" }));
            listView1.Items.Add(new ListViewItem(new string[] { "Oyuncu 2", "0" }));
        }
    }
    public class Player
    {
        public int score = 0;
        public string name = "";
        public Player(string name, int score)
        {
            this.score = score;
            this.name = name;
        }
    }
}

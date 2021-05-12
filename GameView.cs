using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseSim2021
{
    public partial class GameView : Form
    {
        int margin;
        private readonly WorldState theWorld;
        
        /// <summary>
        /// The constructor for the main window
        /// </summary>
        public GameView(WorldState world)
        {
            InitializeComponent();
            theWorld = world;
            margin = 10;
        }
        /// <summary>
        /// Method called by the controler whenever some text should be displayed
        /// </summary>
        /// <param name="s"></param>
        public void WriteLine(string s)
        {
            List<string> strs = s.Split('\n').ToList();
            strs.ForEach(str=>outputListBox.Items.Add(str));
            if (outputListBox.Items.Count > 0)
            {
                outputListBox.SelectedIndex = outputListBox.Items.Count - 1;
            }
            outputListBox.Refresh();
        }
        /// <summary>
        /// Method called by the controler whenever a confirmation should be asked
        /// </summary>
        /// <returns>Yes iff confirmed</returns>
        public bool ConfirmDialog()
        {
            string message = "Confirmer ?";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            return MessageBox.Show(message, "", buttons) == DialogResult.Yes;
        }
        #region Event handling
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                e.SuppressKeyPress = true; // Or beep.
                GameController.Interpret(inputTextBox.Text);
            }
        }

        private void GameView_Paint(object sender, PaintEventArgs e)
        {
            diffLabel.Text = "Difficulté : " + theWorld.TheDifficulty;
            turnLabel.Text = "Tour " + theWorld.Turns;
            moneyLabel.Text = "Trésor : " + theWorld.Money + " pièces d'or";
            gloryLabel.Text = "Gloire : " + theWorld.Glory;

            nextButton.Visible = true;
            Policies(e);
            Perks(e);
            Crises(e);
            Indicators(e);
            Groups(e);

        }
        #endregion

        private void NextButton_Click(object sender, EventArgs e)
        {
            GameController.Interpret("suivant");
        }

        public void LoseDialog(IndexedValue indexedValue)
        {
            if (indexedValue == null)
            {
                MessageBox.Show("Partie perdue : dette insurmontable.");
            }
            else
            {
                MessageBox.Show("Partie perdue :" + indexedValue.CompletePresentation());
            }
            nextButton.Enabled = false;
        }
        public void WinDialog()
        {
            MessageBox.Show("Partie gagné !");
            nextButton.Enabled = false;
        }
        
        

        public void Policies(PaintEventArgs e)
        {
            Rectangle PolRectangle = new Rectangle(new Point(0, 500), new Size(80, 80));
            int x = PolRectangle.X + margin, y = PolRectangle.Y + margin;
            List<IndexedValueView> polViews = new List<IndexedValueView>();
            foreach (IndexedValue p in theWorld.Policies)
            {
                polViews.Add(new IndexedValueView(p, new Size(80, 80), Color.Black, new Point(x, y), p.Type.ToString(), p.Name, p.Value.ToString()));
                x += PolRectangle.Width + margin;
                if (x+PolRectangle.Width+margin > Width)
                {
                    x = Width/2 - Width/4 + margin*2 + margin/2;
                    y += PolRectangle.Height + margin;
                }
            }

            foreach (IndexedValueView p in polViews)
            {
                p.Dessine(e.Graphics);
            }
        }

        public void Perks(PaintEventArgs e)
        {
            Rectangle PerksRectangle = new Rectangle(new Point(180, 30), new Size(80, 80));
            int x = PerksRectangle.X + margin, y = PerksRectangle.Y + margin;
            List<IndexedValueView> perksViews = new List<IndexedValueView>();
            foreach(IndexedValue p in theWorld.Perks)
            {
                perksViews.Add(new IndexedValueView(p, new Size(80, 80), Color.Black, new Point(x, y), p.Type.ToString(), p.Name, p.Value.ToString()));
                x += PerksRectangle.Width + margin;
                if (x + PerksRectangle.Width + margin > Width)
                {
                    x = Width  - Width/2 + margin*2;
                    y += PerksRectangle.Height + margin;
                }
            }

            foreach (IndexedValueView p in perksViews)
            {
                p.Dessine(e.Graphics);
            }

        }

        public void Crises(PaintEventArgs e)
        {
            Rectangle CrisesRectangle = new Rectangle(new Point(405, 120), new Size(80, 80));
            int x = CrisesRectangle.X + margin, y = CrisesRectangle.Y + margin;
            List<IndexedValueView> crisesViews = new List<IndexedValueView>();
            foreach(IndexedValue p in theWorld.Crises)
            {
                crisesViews.Add(new IndexedValueView(p, new Size(80, 80), Color.Black, new Point(x, y), p.Type.ToString(), p.Name, p.Value.ToString()));
                x += CrisesRectangle.Width + margin;
                if (x + CrisesRectangle.Width + margin > Width)
                {
                    x = Width - Width / 2 + margin * 2;
                    y += CrisesRectangle.Height + margin;
                }
            }

            foreach (IndexedValueView p in crisesViews)
            {
                p.Dessine(e.Graphics);
            }
        }
        public void Indicators(PaintEventArgs e)
        {
            Rectangle IndicRectangle = new Rectangle(new Point(90, 200), new Size(80, 80));
            int x = IndicRectangle.X + margin, y = IndicRectangle.Y + margin;
            List<IndexedValueView> indicViews = new List<IndexedValueView>();
            foreach (IndexedValue p in theWorld.Indicators)
            {
                indicViews.Add(new IndexedValueView(p, new Size(80, 80), Color.Black, new Point(x, y), p.Type.ToString(), p.Name, p.Value.ToString()));
                y += IndicRectangle.Width + margin;
                if(y + IndicRectangle.Width + margin > 480)
                {
                    x = Width - 190;
                    y = 200;
                }

            }

            foreach (IndexedValueView p in indicViews)
            {
                p.Dessine(e.Graphics);
            }
        }

        public void Groups(PaintEventArgs e)
        {
            Rectangle GroupsRectangle = new Rectangle(new Point(535, 290), new Size(80, 80));
            int x = GroupsRectangle.X + margin, y = GroupsRectangle.Y + margin;
            List<IndexedValueView> indicViews = new List<IndexedValueView>();
            foreach (IndexedValue p in theWorld.Groups)
            {
                indicViews.Add(new IndexedValueView(p, new Size(80, 80), Color.Black, new Point(x, y), p.Type.ToString(), p.Name, p.Value.ToString()));
                x += GroupsRectangle.Width + margin;
                
            }

            foreach (IndexedValueView p in indicViews)
            {
                p.Dessine(e.Graphics);
            }
        }

        

        private void victoryEasyButton_Click(object sender, EventArgs e)
        {
            GameController.Interpret("etat");
            GameController.Interpret("liste politiques");
            GameController.Interpret("liste taxes");

            GameController.Interpret("politique gardes 100");
            GameController.Interpret("politique pretres 100");
            GameController.Interpret("politique impots 40");

            GameController.Interpret("suivant");
            //GameController.Interpret("etat");
            //GameController.Interpret("liste");

            GameController.Interpret("politique subventions 100");
            GameController.Interpret("politique doleances 100");
            GameController.Interpret("politique quetegraal 10");

            GameController.Interpret("politique ecoles 100");
            GameController.Interpret("politique enchanteurs 100");
            GameController.Interpret("politique taxeluxe 10");

            GameController.Interpret("suivant");

            GameController.Interpret("politique theatres 100");
            GameController.Interpret("politique taxealcool 5");
            GameController.Interpret("politique agrandir territoires 2");
            GameController.Interpret("politique monstres 2");

            GameController.Interpret("suivant");
            GameController.Interpret("suivant");

            GameController.Interpret("politique thermes 100");
            GameController.Interpret("politique juges 100");
            GameController.Interpret("politique taxefonciere 5");
            GameController.Interpret("politique dragons 5");


            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
            GameController.Interpret("suivant");
        }
    }
}

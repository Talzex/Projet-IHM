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
        public List<IndexedValueView> indexedValueViews;
        public List<Traits> traits;
        public readonly int margin;
        public IndexedValueView sélection;
        private readonly WorldState theWorld;
        private readonly int w;
        private readonly int h;

        /// <summary>
        /// The constructor for the main window
        /// </summary>
        public GameView(WorldState world)
        {
            InitializeComponent();
            theWorld = world;
            margin = 10;
            indexedValueViews = new List<IndexedValueView>();
            traits = new List<Traits>();
             w = 80;
             h = 80;
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
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                e.SuppressKeyPress = true; // Or beep.
                GameController.Interpret(inputTextBox.Text);
            }
        }
        #region Event handling

        private void GameView_Paint(object sender, PaintEventArgs e)
        {
            diffLabel.Text = "Difficulté : " + theWorld.TheDifficulty;
            turnLabel.Text = "Tour " + theWorld.Turns;
            moneyLabel.Text = "Trésor : " + theWorld.Money + " pièces d'or";
            gloryLabel.Text = "Gloire : " + theWorld.Glory;

            nextButton.Visible = true;
            Policies();
            Perks();
            Crises();
            Indicators();
            Groups();
            indexedValueViews.ForEach(n => n.Dessine(e.Graphics));
            traits.ForEach(n => n.Dessine(e.Graphics));

        }
        #endregion
        private void GameView_MouseDown(object sender, MouseEventArgs e)
        {
            sélection = Selection(e.Location);
            if(e.Button == MouseButtons.Left)
            {
                if(sélection != null)
                {
                    if(sélection.Type.ToString().Equals("Policy") &&
                       (sélection.theValue.Active != false ||
                       sélection.theValue.AvailableAt <= theWorld.Turns))
                    {
                        int val = Convert.ToInt32(sélection.theValue.Value);
                        ValueExplorer valueExplorer = new ValueExplorer(sélection,val,theWorld);
                        if (valueExplorer.ShowDialog() == DialogResult.OK)
                        {
                            val = valueExplorer.Valeur;
                            int mCost;
                            int gCost;
                            if (val != sélection.theValue.Value)
                            {
                                sélection.theValue.PreviewPolicyChange(ref val, out mCost, out gCost);
                                theWorld.CostGlory(gCost);
                                sélection.theValue.ChangeTo(val, out _, out _);
                            }
                        }
                        Refresh();
                    }
                }
            }
        }
        private void GameView_MouseMove(object sender, MouseEventArgs e)
        {
            sélection = Selection(e.Location);
            traits.Clear();
            if (sélection != null)
            {
                if (sélection.Type.ToString().Equals("Policy") && (sélection.theValue.Active != false || sélection.theValue.AvailableAt <= theWorld.Turns))
                {
                    foreach (KeyValuePair<IndexedValue, double> p in sélection.theValue.OutputWeights)
                    {
                        int index = indexedValueViews.FindIndex(c => c.theValue.Equals(p.Key));
                        #region ChangementCouleur Traits
                        if (index != -1)
                        {
                            if (p.Value > 0)
                            {
                                if (p.Value < 1 && p.Value > 0.09)
                                {
                                    traits.Add(new Traits
                                    {
                                        color = Color.Green,
                                        Source = sélection,
                                        Destination = indexedValueViews[index]
                                    });
                                }
                                else if (p.Value < 0.1 && p.Value > 0.009)
                                {
                                    traits.Add(new Traits
                                    {
                                        color = Color.Lime,
                                        Source = sélection,
                                        Destination = indexedValueViews[index]
                                    });
                                }
                                else
                                {
                                    traits.Add(new Traits
                                    {
                                        color = Color.PaleGreen,
                                        Source = sélection,
                                        Destination = indexedValueViews[index]
                                    });
                                }
                            }
                            if (p.Value < 0)
                            {
                                if (p.Value > -1 && p.Value < -0.09)
                                {
                                    traits.Add(new Traits
                                    {
                                        color = Color.Red,
                                        Source = sélection,
                                        Destination = indexedValueViews[index]
                                    });
                                }
                                else if (p.Value > -0.1 && p.Value < -0.009)
                                {
                                    traits.Add(new Traits
                                    {
                                        color = Color.Orange,
                                        Source = sélection,
                                        Destination = indexedValueViews[index]
                                    });
                                }
                                else
                                {
                                    traits.Add(new Traits
                                    {
                                        color = Color.Yellow,
                                        Source = sélection,
                                        Destination = indexedValueViews[index]
                                    });
                                }
                            }

                        }
                        #endregion 
                    }
                }
            }
            Refresh();
        }

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
        
        public IndexedValueView Selection(Point p)
        {
            return indexedValueViews.FirstOrDefault(c => c.Contient(p));
        }
        #region Construction Views
        public void Policies()
        {
            Brush c;
            Rectangle PolRectangle = new Rectangle(new Point(0, 500), new Size(1380, 200));
            int x = PolRectangle.X + margin, y = PolRectangle.Y + margin;
            foreach (IndexedValue p in theWorld.Policies)
            {
                if(p.Active != false || p.AvailableAt <= theWorld.Turns)
                {
                    c = Brushes.LightSteelBlue;
                } else 
                {
                    c = Brushes.DarkSlateGray;
                }
                indexedValueViews.Add(new IndexedValueView(p, new Point(x, y), p.Type.ToString(), p.Name, p.Value.ToString(), c));
                x += w + margin;
                if (x+w+margin > PolRectangle.Right)
                {
                    x = PolRectangle.Right / 2 - PolRectangle.Right / 4 + margin*6 ;
                    y += h + margin;
                }
            }
        }
        public void Perks()
        {
            Rectangle PerksRectangle = new Rectangle(new Point(180, 30), new Size(1380, 100));
            int x = PerksRectangle.X + margin, y = PerksRectangle.Y + margin;
            foreach(IndexedValue p in theWorld.Perks)
            {
                indexedValueViews.Add(new IndexedValueView(p, new Point(x, y), p.Type.ToString(), p.Name, p.Value.ToString(), Brushes.Cyan));
                x += w + margin;
            }
        }
        public void Crises()
        {
            Rectangle CrisesRectangle = new Rectangle(new Point(405, 120), new Size(1380, 100));
            int x = CrisesRectangle.X + margin, y = CrisesRectangle.Y + margin;
            foreach(IndexedValue p in theWorld.Crises)
            {
                indexedValueViews.Add(new IndexedValueView(p, new Point(x, y), p.Type.ToString(), p.Name, p.Value.ToString(), Brushes.Brown));
                x += w + margin;
            }
        }
        public void Indicators()
        {
            Rectangle IndicRectangle = new Rectangle(new Point(90, 200), new Size(80, 80));
            int x = IndicRectangle.X + margin, y = IndicRectangle.Y + margin;
            foreach (IndexedValue p in theWorld.Indicators)
            {
                indexedValueViews.Add(new IndexedValueView(p, new Point(x, y), p.Type.ToString(), p.Name, p.Value.ToString(), Brushes.LightYellow));
                y += IndicRectangle.Width + margin;
                if(y + IndicRectangle.Width + margin > 480)
                {
                    x = Width - 190;
                    y = 200;
                }
            }
        }
        public void Groups()
        {
            Rectangle GroupsRectangle = new Rectangle(new Point(535, 290), new Size(280, 100));
            int x = GroupsRectangle.X + margin, y = GroupsRectangle.Y + margin;
            foreach (IndexedValue p in theWorld.Groups)
            {
                indexedValueViews.Add(new IndexedValueView(p, new Point(x, y), p.Type.ToString(), p.Name, p.Value.ToString(), Brushes.LightSalmon));
                x += w + margin;
                
            }
        }
        #endregion


    }
}

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
    public partial class ValueExplorer : Form
    {
        public int Valeur { get { return (int)numericUpDown1.Value; } }
        public IndexedValueView selection;
        private readonly WorldState theWorld;
        public List<Rectangle> backrectangle;
        public List<Rectangle> initValrectangle;
        public List<Rectangle> changeValrectangle;
        private int gcost;
        private int val;
        
        public ValueExplorer(IndexedValueView selection, int numericvalue, WorldState world)
        {
            InitializeComponent();
            theWorld = world;
            this.selection = selection;
            numericUpDown1.Value = numericvalue;
            gcost = 0;
            backrectangle = new List<Rectangle>();
            initValrectangle = new List<Rectangle>();
            changeValrectangle = new List<Rectangle>();
           
        }

        private void ValueExplorer_Paint(object sender, PaintEventArgs e)
        {
            TitreLabel.Text = selection.Texte;
            TitreLabel.Left = Width / 2 - TitreLabel.Width / 2;
            TitreLabel.Font = new Font("Times New Roman", 16, FontStyle.Bold);

            DescLabel.Text = selection.theValue.Description;
            DescLabel.Left = Width / 2 - DescLabel.Width / 2;
            DescLabel.Font = new Font("Times New Roman", 10, FontStyle.Regular);

            gloryEst.Text = "Gloire : " + theWorld.Glory + " " + gcost;
            gloryEst.Font = new Font("Times New Roman", 10, FontStyle.Regular);

            OutputWeight(e.Graphics);
        }

        private void OutputWeight(Graphics g)
        {
            Rectangle PolRectangle = new Rectangle(new Point(10, 75), new Size(Width, Height-50));
            int x = PolRectangle.X, y = PolRectangle.Y;
            foreach(KeyValuePair<IndexedValue,double> p in selection.theValue.OutputWeights)
            {
                g.DrawString(p.Key.Name, new Font("Times New Roman", 8, FontStyle.Bold), Brushes.Black, new PointF(x, y));
                backrectangle.Add(new Rectangle(new Point(x + 75, y), new Size(475, 20)));
                
                int x2 = p.Key.Value * 475 / p.Key.MaxValue;
                if(p.Value < 0)
                {
                    initValrectangle.Add(new Rectangle(new Point(x + 75, y), new Size(p.Key.Value * 475 / p.Key.MaxValue, 10)));
                    Rectangle r = new Rectangle(new Point(x2+85- (int)(p.Value * -1000), y+10), new Size((int)(p.Value * -1000), 10));
                    g.DrawRectangle(new Pen(Color.Black), r);
                    g.FillRectangle(Brushes.Red, r);
                } else
                {
                    initValrectangle.Add(new Rectangle(new Point(x + 75, y), new Size(p.Key.Value * 475 / p.Key.MaxValue, 20)));
                    Rectangle r = new Rectangle(new Point(x2 + 85, y ), new Size((int)(p.Value * 1000), 20));
                    g.DrawRectangle(new Pen(Color.Black), r);
                    g.FillRectangle(Brushes.Lime, r);
                }
                y += 25;
            }
            foreach (Rectangle r in backrectangle)
            {
                g.DrawRectangle(new Pen(Color.Black), r);
                g.FillRectangle(Brushes.Transparent, r);
            }

            foreach (Rectangle r in initValrectangle)
            {
                g.DrawRectangle(new Pen(Color.Black), r);
                g.FillRectangle(Brushes.Gray, r);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            val = Convert.ToInt32(numericUpDown1.Value);
            int mCost;
            if (val != selection.theValue.Value)
            {
                selection.theValue.PreviewPolicyChange(ref val, out mCost, out gcost);
                Refresh();
            }
        }
    }
 }


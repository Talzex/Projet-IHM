using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSim2021
{
    public class IndexedValueView
    {
        public IndexedValue theValue;
        public Point Coordonnées { get; set; }
        public String Type { get; set; }
        public String Texte { get; set; }
        public String Valeur { get; set; }
        public Brush Col { get; set; }

        public IndexedValueView(IndexedValue theValue, Point Coordonnées, String Type, String Nom, String Valeur, Brush Col)
        {
            this.theValue = theValue;
            this.Coordonnées = Coordonnées;
            this.Type = Type;
            Texte = Nom;
            this.Valeur = Valeur;
            this.Col = Col;
        }
        public void Dessine(Graphics g)
        {
           
            StringFormat stringFormatType = new StringFormat();
            StringFormat stringFormatNom = new StringFormat();
            StringFormat stringFormatValeur = new StringFormat();
            StringFormat stringFormatMin = new StringFormat();
            StringFormat stringFormatMax = new StringFormat();

            Rectangle r = new Rectangle(Coordonnées, new Size(80,80));
            Pen p = new Pen(Color.Black, 3);
            g.DrawRectangle(p, r);
            g.FillRectangle(Col, r);

            stringFormatType.Alignment = StringAlignment.Center;
            stringFormatType.LineAlignment = StringAlignment.Near;
            g.DrawString(Type, new Font("Times New Roman", 12, FontStyle.Bold), Brushes.Black, r, stringFormatType); ;
            
            
            stringFormatNom.Alignment = StringAlignment.Center;
            stringFormatNom.LineAlignment = StringAlignment.Center;
            g.DrawString(Texte, new Font("Times New Roman", 10, FontStyle.Bold), Brushes.Black,r, stringFormatNom);

            stringFormatValeur.Alignment = StringAlignment.Center;
            stringFormatValeur.LineAlignment = StringAlignment.Far;
            g.DrawString(Valeur, new Font("Times New Roman", 8, FontStyle.Bold), Brushes.Black, r, stringFormatValeur);

            stringFormatMin.Alignment = StringAlignment.Near;
            stringFormatMin.LineAlignment = StringAlignment.Far;
            g.DrawString(theValue.MinValue.ToString(), new Font("Times New Roman", 8, FontStyle.Italic), Brushes.Black, r, stringFormatMin);

            stringFormatMax.Alignment = StringAlignment.Far;
            stringFormatMax.LineAlignment = StringAlignment.Far;
            g.DrawString(theValue.MaxValue.ToString(), new Font("Times New Roman", 8, FontStyle.Italic), Brushes.Black, r, stringFormatMax);


        }

        public bool Contient(Point p)
        {
            Rectangle r = new Rectangle(Coordonnées, new Size(80,80));
            return r.Contains(p);
        }

        public Point Centre
        {
            get
            {
                return new Point(Coordonnées.X + 80 / 2,
           Coordonnées.Y + 80 / 2);
            }
        }

    }
}

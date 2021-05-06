using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSim2021
{
    class IndexedValueView
    {
        public readonly IndexedValue theValue;
        
        public Size Taille { get; set; }
        public Color Couleur { get; set; }
        public Point Coordonnées { get; set; }
        public String Type { get; set; }
        public String Texte { get; set; }
        public String Valeur { get; set; }

        public IndexedValueView(IndexedValue theValue, Size Taille, Color Couleur, Point Coordonnées, String Type, String Nom, String Valeur)
        {
            this.theValue = theValue;
            this.Taille = Taille;
            this.Couleur = Couleur;
            this.Coordonnées = Coordonnées;
            this.Type = Type;
            this.Texte = Nom;
            this.Valeur = Valeur;
        }
        public void Dessine(Graphics g)
        {
           
            StringFormat stringFormatType = new StringFormat();
            StringFormat stringFormatNom = new StringFormat();
            StringFormat stringFormatValeur = new StringFormat();


            Rectangle r = new Rectangle(Coordonnées, Taille);
            Pen p = new Pen(Couleur);
            g.DrawRectangle(p, r);
            
            stringFormatType.Alignment = StringAlignment.Center;
            stringFormatType.LineAlignment = StringAlignment.Near;
            g.DrawString(Type, new Font("Times New Roman", 12, FontStyle.Bold), Brushes.Black, r, stringFormatType); ;
            
            
            stringFormatNom.Alignment = StringAlignment.Center;
            stringFormatNom.LineAlignment = StringAlignment.Center;
            g.DrawString(Texte, new Font("Times New Roman", 8, FontStyle.Regular), Brushes.Black,r, stringFormatNom);

            stringFormatValeur.Alignment = StringAlignment.Center;
            stringFormatValeur.LineAlignment = StringAlignment.Far;
            g.DrawString(Valeur, new Font("Times New Roman", 8, FontStyle.Italic), Brushes.Black, r, stringFormatValeur);
        }
        
    }
}

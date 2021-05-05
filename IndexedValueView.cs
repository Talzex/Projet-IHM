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
        public String Texte { get; set; }

        

        public IndexedValueView(IndexedValue theValue,Size Taille, Color Couleur, Point Coordonnées, String Texte)
        {
            this.theValue = theValue;
            this.Taille = Taille;
            this.Couleur = Couleur;
            this.Coordonnées = Coordonnées;
            this.Texte = Texte;
        }

        public void Dessine(Graphics g)
        {
            Rectangle r = new Rectangle(Coordonnées, Taille);
            Pen p = new Pen(Couleur);
            g.DrawRectangle(p, r);
        }
        
    }
}

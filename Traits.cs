using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BaseSim2021
{
    public class Traits
    {
        public Color color;
        public IndexedValueView Source { get; set; }
        public IndexedValueView Destination { get; set; }

        public void Dessine(Graphics g)
        {
            Pen p = new Pen(color, 3);
            g.DrawLine(p, Source.Centre,
            Destination.Centre);
        }
    }
}

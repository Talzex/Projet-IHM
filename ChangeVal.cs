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
    public partial class ChangeVal : Form
    {
        public int Valeur { get { return (int)numericUpDown1.Value; } }
        public ChangeVal(int v)
        {
            InitializeComponent();
            numericUpDown1.Value = v;
        }
    }
}

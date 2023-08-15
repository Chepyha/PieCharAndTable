using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Test
    {
        public double values { get; set; }
        public Color color { get; set; } = new Color();
        public Test( double values=5, Color color=default) {  this.values = values; this.color = color;  }
    }
}

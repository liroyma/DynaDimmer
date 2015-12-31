using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynadimmer.Models.Messages
{
    public abstract class GaneralMessage
    {

        public DateTime Time { get; private set; }
        public Brush MessageColor { get; set; }
        public string Info { get; private set; }

        public GaneralMessage(string info)
        {
            Info = info;
            MessageColor = Brushes.Black;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynadimmer.Models.Messages
{
    class ConnectionMessage:GaneralMessage
    {
        public ConnectionMessage(string info):base(info)
        {
            MessageColor = Brushes.Black;
        }

        public ConnectionMessage(string info, Brush color) : base(info)
        {
            MessageColor = color;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynadimmer.Models.Messages
{
    class NotificationMessage:GaneralMessage
    {
        public NotificationMessage(string info, Brush color) :base(info)
        {
            MessageColor = color;
        }
    }
}

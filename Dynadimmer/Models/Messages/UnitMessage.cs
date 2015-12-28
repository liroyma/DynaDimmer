using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynadimmer.Models.Messages
{
    public class UnitMessage
    {
        public const byte StartOutMessage = 0x02;
        public const byte StartInMessage = 0x01;
        public const byte EndOutMessage = 0x03;

        public DateTime Time { get; private set; }
        public string Info { get; private set; }
        public Brush MessageColor { get; set; }

        public string DecimalFormatString { get; set; }
        public string AsciiFormatString { get; set; }
        public string HexFormatString { get; set; }


        public byte[] DataAscii { get; set; }
        public byte[] DecimalData { get; set; }
        public string[] HexData { get; set; }


        public UnitMessage()
        {
            Time = DateTime.Now;
            MessageColor = Brushes.Black;
        }

        public UnitMessage(string info)
        {
            Info = info;
            Time = DateTime.Now;
            MessageColor = Brushes.Black;
        }


    }
}

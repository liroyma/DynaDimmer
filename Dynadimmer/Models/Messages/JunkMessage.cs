using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynadimmer.Models.Messages
{
    class JunkMessage:UnitMessage
    {
        private List<byte> answer;
        
        public JunkMessage(List<byte> answer):base("Unknown data")
        {
            MessageColor = Brushes.Brown;
            DataAscii = answer.ToArray();
            //DecimalData = Createmessage(false, header, data);

           // DecimalFormatString = string.Join(" ", DecimalData);
           // HexFormatString = string.Join(" ", ConvertDecToHex(DecimalData));
            AsciiFormatString = string.Join(" ", DataAscii);
        }
    }
}

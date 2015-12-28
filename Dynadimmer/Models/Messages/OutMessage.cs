using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynadimmer.Models.Messages
{
    public class OutMessage: UnitMessage
    {
        public OutMessage(string info,byte header, params byte[] data):base(info)
        {
            MessageColor = Brushes.Blue;
            DataAscii = Createmessage(true, header, data);
            DecimalData = Createmessage(false, header, data);

            DecimalFormatString = string.Join(" ", DecimalData);
            HexFormatString = string.Join(" ", ConvertDecToHex(DecimalData));
            AsciiFormatString = string.Join(" ", DataAscii);
        }

        private string[] ConvertDecToHex(byte[] decimalarray)
        {
            List<string> list = new List<string>();
            foreach (var item in decimalarray)
            {
                list.Add(item.ToString("x2").ToUpper());
            }
            return list.ToArray();
        }

        private byte[] Createmessage(bool isAscii, byte header, params byte[] data)
        {
            List<byte> fulldata = new List<byte>();
            fulldata.Add(StartOutMessage);
            fulldata.AddRange(Message(isAscii, header, data));
            fulldata.Add(EndOutMessage);
            return fulldata.ToArray();

        }

        private byte[] Message(bool isAscii, byte Header, params byte[] data)
        {
            List<byte> list = new List<byte>();
            list.Add(Header);
            if (data != null)
            {
                list.AddRange(data);
            }
            return CRCManager.AddCRC(list.ToArray(), isAscii);
        }
    }
}

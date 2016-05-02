using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynadimmer.Models.Messages
{
    public class OutMessage : UnitMessage
    {
        public byte Header { get; set; }

        public OutMessage(string info, byte[] unitid, byte header, params byte[] data) : base(info)
        {
            Header = header;
            MessageColor = Brushes.Blue;
            DataAscii = Createmessage(true, unitid, header, data);
            DecimalData = Createmessage(false, unitid, header, data);

            DecimalFormatString = string.Join(" ", DecimalData);
            HexFormatString = string.Join(" ", ConvertDecToHex(DecimalData));
            AsciiFormatString = string.Join(" ", DataAscii);
            //once = true;
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

        private byte[] Createmessage(bool isAscii, byte[] unitid, byte header, params byte[] data)
        {
            List<byte> fulldata = new List<byte>();
            fulldata.Add(StartOutMessage);
            fulldata.AddRange(Message(isAscii, unitid, header, data));
            fulldata.Add(EndOutMessage);
            return fulldata.ToArray();

        }
        //static bool once = false;
        private byte[] Message(bool isAscii, byte[] unitid, byte Header, params byte[] data)
        {
            List<byte> list = new List<byte>();
           // if (!once)
                list.AddRange(unitid);
            list.Add(Header);
            if (data != null)
            {
                list.AddRange(data);
            }
            return CRCManager.AddCRC(list.ToArray(), isAscii);
        }
    }
}

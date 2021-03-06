﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynadimmer.Models.Messages
{
    public class IncomeMessage : UnitMessage
    {
        public byte Header { get; private set; }
        public byte[] OnlyData { get; private set; }

        public IncomeMessage(string info,List<byte> answer):base(info)
        {
            MessageColor = Brushes.Green;
            List<byte> data = new List<byte>();
            data.Add(1);
            data.AddRange(CRCManager.CheckCRC(answer));
            data.Add(3);

            Header = data[1];

            DataAscii =  answer.ToArray();
            DecimalData = data.ToArray();
            OnlyData = DecimalData.ToList().GetRange(2, DecimalData.Length - 3).ToArray();

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

    }
}

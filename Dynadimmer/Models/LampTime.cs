using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynadimmer.Views.Schedulers
{
    class LampTime
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Precentage { get; set; }
        public string TimeString { get; set; }

        public LampTime(int hour ,int minute, int precentage)
        {
            Hour = hour;
            Minute = minute;
            TimeString = string.Format("{0}:{1}", Hour.ToString("D2"), Minute.ToString("D2"));
            Precentage = precentage;
        }

        public LampTime(int time, int precentage)
        {
            Hour = time / 60;
            Minute = time % 60;
            TimeString = string.Format("{0}:{1}", Hour.ToString("D2"), Minute.ToString("D2"));
            Precentage = precentage;
        }

        public LampTime(string timestring, int precentage)
        {
            TimeString = timestring;
            string[] temp = TimeString.Split(':');
            Hour = int.Parse(temp[0]);
            Minute = int.Parse(temp[1]);
            Precentage = precentage;
        }

    }
}

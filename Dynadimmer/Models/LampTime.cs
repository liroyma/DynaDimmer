using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynadimmer.Views.Schedulers
{
    public class LampTime
    {
        public const int STARTHOUR = 15;
        public const int STARTMINUTE = 00;

        public int Precentage { get; set; }

        private string timestring;
        public string TimeString
        {
            get { return timestring; }
            private set
            {
                timestring = value;
                date = System.DateTime.Parse(TimeString);
                if (date.Hour < STARTHOUR || date.Hour > 23)
                    date = date.AddDays(1);
            }
        }

        public System.DateTime date { get; private set; }

        public LampTime(int hour, int minute, int precentage)
        {
            TimeString = string.Format("{0}:{1}", hour.ToString("D2"), minute.ToString("D2"));
            Precentage = precentage;
        }

        public LampTime(int time, int precentage)
        {
            if (time == 1440)
                TimeString = "00:00";
            else
            {
                int hour = time / 60;
                int minute = time % 60;
                TimeString = string.Format("{0}:{1}", hour.ToString("D2"), minute.ToString("D2"));
            }
            Precentage = precentage;
        }

        public LampTime(string timestring, int precentage)
        {
            TimeString = timestring;
            Precentage = precentage;
        }

        internal void UpdateTime(string timestring)
        {
            if (timestring == "24:0")
                TimeString = "07:00";
            else
                TimeString = timestring;
        }

        internal void UpdateTime(int time)
        {
            if (time == 1440)
                TimeString = "00:00";
            else
            {
                int hour = time / 60;
                int minute = time % 60;
                TimeString = string.Format("{0}:{1}", hour.ToString("D2"), minute.ToString("D2"));
            }
        }
    }
}

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
        public System.DateTime Date { get; private set; }
        
        public static System.DateTime GetRightTime(System.DateTime time)
        {
            System.DateTime dd = System.DateTime.Today.AddHours(time.Hour).AddMinutes(time.Minute);

            return (dd.Hour < STARTHOUR || dd.Hour > 23) ? dd.AddDays(1) : dd;
        }

        public static double CalcTotalHoursSpan(LampTime one, LampTime two)
        {
            return (two.Date - one.Date).TotalHours;
        }

        public LampTime(int hour, int minute, int precentage)
        {
            Date = GetRightTime(System.DateTime.Parse(string.Format("{0}:{1}", hour.ToString("D2"), minute.ToString("D2"))));
            Precentage = precentage;
        }

        public LampTime(int time, int precentage)
        {
            if (time == 1440)
                Date = GetRightTime(System.DateTime.Parse("00:00"));
                //TimeString = "00:00";
            else
            {
                int hour = time / 60;
                int minute = time % 60;
                Date = GetRightTime(System.DateTime.Parse(string.Format("{0}:{1}", hour.ToString("D2"), minute.ToString("D2"))));
                //TimeString = string.Format("{0}:{1}", hour.ToString("D2"), minute.ToString("D2"));
            }
            Precentage = precentage;
        }
        
        public LampTime(System.DateTime time, int precentage)
        {
            Date = GetRightTime(time);
            Precentage = precentage;
        }

      /*  internal void UpdateTime(string timestring)
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
        }*/
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynadimmer.Models;
using Dynadimmer.Models.Messages;

namespace Dynadimmer.Views.SummerWinnter
{
    class UnitSummerWinnterClockModel : UnitProperty
    {
        private const int Header = 2;

        #region Properties
        private System.DateTime winterdate;
        public System.DateTime WinterDate
        {
            get { return winterdate; }
            set
            {
                winterdate = value;
                NotifyPropertyChanged("WinterDate");
            }
        }

        private System.DateTime summerdate;
        public System.DateTime SummerDate
        {
            get { return summerdate; }
            set
            {
                summerdate = value;
                NotifyPropertyChanged("SummerDate");
            }
        }
        #endregion

        public UnitSummerWinnterClockModel()
        {
            Title = "Summer & Winter Clock";
        }

        public override void SendDownLoad(object sender)
        {
            List<byte> message = new List<byte>();
            message.Add((byte)SummerDate.Day);
            message.Add((byte)SummerDate.Month);
            message.Add(0);
            message.Add((byte)WinterDate.Day);
            message.Add((byte)WinterDate.Month);
            message.Add(0);
            CreateAndSendMessage(this, Header, message.ToArray());
        }

        public override void SendUpload(object sender)
        {
            CreateAndSendMessage(this, Header);
        }

        public override void GotAnswer(IncomeMessage messase)
        {
            byte[] data = messase.DecimalData;
            string dateString1 = String.Format("{0}/{1}/{2}", data[2], data[3], System.DateTime.Now.Year);
            string dateString2 = String.Format("{0}/{1}/{2}", data[5], data[6], System.DateTime.Now.Year);
            SummerDate = System.DateTime.Parse(dateString1);
            WinterDate = System.DateTime.Parse(dateString2);
            base.SetView();
        }

        public override void DidntGotAnswer()
        {
        }
    }
}

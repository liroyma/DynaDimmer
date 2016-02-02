using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynadimmer.Models;
using Dynadimmer.Models.Messages;
using System.Xml;
using Xceed.Wpf.Toolkit;
using Dynadimmer.Views.Information;

namespace Dynadimmer.Views.SummerWinnter
{
    class UnitSummerWinnterClockModel : UnitProperty
    {
        public const int Header = 2;

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
            if (SummerDate.Month > WinterDate.Month || (SummerDate.Month == WinterDate.Month && SummerDate.Day > WinterDate.Day))
            {
                MessageBox.Show("Winter clock change can't be before Summer clock change.");
                return;
            }
            List<byte> message = new List<byte>();
            message.Add((byte)SummerDate.Day);
            message.Add((byte)SummerDate.Month);
            message.Add(0);
            message.Add((byte)WinterDate.Day);
            message.Add((byte)WinterDate.Month);
            message.Add(0);
            CreateAndSendMessage(SendMessageType.Download, Header, message.ToArray());
        }

        public override void SendUpload(object sender)
        {
            CreateAndSendMessage(SendMessageType.Upload, Header);
        }

        public override string GotAnswer(IncomeMessage messase)
        {
            byte[] data = messase.DecimalData;
            string dateString1 = String.Format("{0}/{1}/{2}", data[2], data[3], System.DateTime.Now.Year);
            string dateString2 = String.Format("{0}/{1}/{2}", data[5], data[6], System.DateTime.Now.Year);
            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("he-IL");
            SummerDate = System.DateTime.Parse(dateString1,cultureinfo);
            WinterDate = System.DateTime.Parse(dateString2,cultureinfo);
            base.SetView();
            return Title;
        }

        public override void DidntGotAnswer()
        {
        }

        public override void SaveData(System.Xml.XmlWriter writer, object extra)
        {
        }

        public override void UpdateData(UnitInfo info)
        {
        }

        protected override void OnGotData(UnitInfo info)
        {
            IsLoaded = true;
            base.OnGotData(info);
        }
    }
}

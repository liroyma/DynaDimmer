using Dynadimmer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynadimmer.Models.Messages;
using System.Xml;

namespace Dynadimmer.Views.Config
{
    public class ConfigModel : UnitProperty
    {
        public const int Header = 3;

        public event EventHandler GotData;

        #region Properties
        private string unittime;
        public string UnitTime
        {
            get { return unittime; }
            set
            {
                unittime = value;
                NotifyPropertyChanged("UnitTime");
            }
        }

        private int unitlampcount;
        public int UnitLampCount
        {
            get { return unitlampcount; }
            set
            {
                unitlampcount = value;
                NotifyPropertyChanged("UnitLampCount");
            }
        }
        #endregion

        public ConfigModel() : base()
        {
            Title = "Unit Configuration";
        }

        public override void DidntGotAnswer()
        {
        }

        public override string GotAnswer(IncomeMessage messase)
        {
            byte[] data = messase.DecimalData;
            string dateString = String.Format("{0}/{1}/{2} {3}:{4}:{5}", data[3], data[4], data[5], data[6], data[7], data[8]);
            System.DateTime date = System.DateTime.Parse(dateString);
            UnitTime = date.DayOfWeek + " - " + date.ToString("dd/MM/yy HH:mm:ss");
            UnitLampCount = messase.DecimalData[9];
            if(Notify)
                GotData(null, null);
            Notify = true;
            base.SetView();
            return Title;
        }
        private bool Notify = true;

        public override void SendDownLoad(object sender)
        {
            List<byte> DATA = new List<byte>();
            if (sender == null)
                DATA.Add((byte)UnitLampCount);
            else
            {
                Notify = false;
                DATA.Add((byte)sender);
            }
            CreateAndSendMessage(SendMessageType.Download, Header, DATA.ToArray());
        }

        public override void SendUpload(object sender)
        {
            CreateAndSendMessage(SendMessageType.Upalod, Header);
        }

        public override void SaveData(XmlWriter writer, object extra)
        {
            writer.WriteStartElement("Configutarion");
            writer.WriteAttributeString("LampCount", UnitLampCount.ToString());
            writer.WriteEndElement();
        }
    }
}

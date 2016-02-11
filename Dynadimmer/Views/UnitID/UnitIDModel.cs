using Dynadimmer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynadimmer.Models.Messages;
using System.Xml;
using Dynadimmer.Views.Information;
using System.Windows;

namespace Dynadimmer.Views.UnitID
{
    public class UnitIDModel : UnitProperty
    {
        public const int Header = 200;

        private uint unitid;
        public uint UnitID
        {
            get { return unitid; }
            set
            {
                unitid = value;
                NotifyPropertyChanged("UnitID");
            }
        }

        public Visibility downloadvisibility;
        public Visibility DownloadVisibility
        {
            get { return downloadvisibility; }
            set
            {
                downloadvisibility = value;
                NotifyPropertyChanged("DownloadVisibility");
            }
        }

        public UnitIDModel()
        {
            Title = "Unit ID";
            DownloadVisibility = Visibility.Collapsed;
        }

        public override void DidntGotAnswer()
        {

        }

        public override string GotAnswer(IncomeMessage messase)
        {
            byte[] data = messase.DecimalData;
            byte[] bytes = { data[5], data[4], data[3], data[2] };
            UnitID = BitConverter.ToUInt32(bytes, 0);
            base.OnGotData(new UnitInfo() { UnitID = UnitID });
            base.SetView();
            return Title;
        }

        public override void SaveData(XmlWriter writer, object extra)
        {

        }

        public override void SendDownLoad(object sender)
        {
            /* if (BroadCast)
             {
                 byte[] bytes = { 255, 255, 255, 255 };
                 CreateAndSendMessage(SendMessageType.Download, Header, bytes);
             }
             else*/
            CreateAndSendMessage(SendMessageType.Download, Header, BitConverter.GetBytes(UnitID).Reverse().ToArray());
        }

        public override void SendUpload(object sender)
        {
            CreateAndSendMessage(SendMessageType.Upload, Header);
        }

        public override void UpdateData(UnitInfo info)
        {
            UnitID = info.UnitID;
            IsLoaded = true;
        }

        protected override void OnGotData(UnitInfo info)
        {
            UnitID = info.UnitID;
            IsLoaded = true;
            base.OnGotData(info);
        }

    }
}

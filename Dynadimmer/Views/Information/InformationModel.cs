using Dynadimmer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynadimmer.Models.Messages;
using System.Xml;
using System.ComponentModel;
using System.Windows;

namespace Dynadimmer.Views.Information
{
    public class InformationModel : UnitProperty
    {
        public const int Header = 199;

        private UnitInfo info;
        public UnitInfo Info
        {
            get { return info; }
            set
            {
                info = value;
                NotifyPropertyChanged("Info");
            }
        }

        private Visibility datavisibility;
        public Visibility DataVisibility
        {
            get { return datavisibility; }
            set
            {
                datavisibility = value;
                NotifyPropertyChanged("DataVisibility");
            }
        }

        private Visibility nodatavisibility;
        public Visibility NoDataVisibility
        {
            get { return nodatavisibility; }
            set
            {
                nodatavisibility = value;
                DataVisibility = NoDataVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                NotifyPropertyChanged("NoDataVisibility");
            }
        }

        public InformationModel()
        {
            Info = new UnitInfo();
            Title = "Unit Infornation";
            NoDataVisibility = Visibility.Visible;
        }

        public override void DidntGotAnswer()
        {
        }

        public override string GotAnswer(IncomeMessage messase)
        {
            byte[] data = messase.DecimalData;
            Info.UnitID = BitConverter.ToUInt32(new byte[] { data[5], data[4], data[3], data[2], }, 0);
            Info.SoftwareVersion = data[6];
            Info.HardwareVersion = data[7];
            string dateString = String.Format("{0}/{1}/{2} {3}:{4}:{5}", data[8], data[9], data[10], data[11], data[12], data[13]);
            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("he-IL");
            System.DateTime date = System.DateTime.Parse(dateString, cultureinfo);
            Info.UnitClock = date.DayOfWeek + " - " + date.ToString("dd/MM/yy HH:mm:ss");
            Info.LampsCount = data[14];
            Info.Lamp1Power = BitConverter.ToUInt16(new byte[] { data[16], data[15] }, 0);
            Info.Lamp2Power = BitConverter.ToUInt16(new byte[] { data[18], data[17] }, 0);
            NoDataVisibility = Visibility.Collapsed;
             OnGotData(info);
            return Title;
        }

        public override void SaveData(XmlWriter writer, object extra)
        {
            writer.WriteStartElement("Configutarion");
            writer.WriteAttributeString("LampCount", Info.LampsCount.ToString());
            writer.WriteAttributeString("SoftwareVersion", Info.SoftwareVersion.ToString());
            writer.WriteAttributeString("HardwareVersion", Info.HardwareVersion.ToString());
            writer.WriteEndElement();
        }

        public override void SendDownLoad(object sender)
        {

        }

        public override void SendUpload(object sender)
        {
            CreateAndSendMessage(SendMessageType.Upload, Header);
        }

        public override void UpdateData(UnitInfo info)
        {
            if (info.UnitID != int.MaxValue)
                Info.UnitID = info.UnitID;
            if (info.UnitClock != string.Empty)
                Info.UnitClock = info.UnitClock;
            if (info.LampsCount != int.MaxValue)
                Info.LampsCount = info.LampsCount;
            if (info.Lamp1Power != int.MaxValue)
                Info.Lamp1Power = info.Lamp1Power;
            if (info.Lamp2Power != int.MaxValue)
                Info.Lamp2Power = info.Lamp2Power;
        }

        protected override void OnGotData(UnitInfo info)
        {
            Info = info;
            IsLoaded = true;
            base.OnGotData(info);
        }
    }

    public class UnitInfo : MyUIHandler
    {
        private Visibility lamp1visibility;
        public Visibility Lamp1Visibility
        {
            get { return lamp1visibility; }
            set
            {
                lamp1visibility = value;
                NotifyPropertyChanged("Lamp1Visibility");
            }
        }

        private Visibility lamp2visibility;
        public Visibility Lamp2Visibility
        {
            get { return lamp2visibility; }
            set
            {
                lamp2visibility = value;
                NotifyPropertyChanged("Lamp2Visibility");
            }
        }

        private uint unitid = int.MaxValue;
        public uint UnitID
        {
            get { return unitid; }
            set
            {
                unitid = value;
                NotifyPropertyChanged("UnitID");
            }
        }

        private string unitclock = string.Empty;
        public string UnitClock
        {
            get { return unitclock; }
            set
            {
                unitclock = value;
                NotifyPropertyChanged("UnitClock");
            }
        }

        private int lamp2power = int.MaxValue;
        public int Lamp2Power
        {
            get { return lamp2power; }
            set
            {
                lamp2power = value;
                NotifyPropertyChanged("Lamp2Power");
            }
        }

        private int lamp1power = int.MaxValue;
        public int Lamp1Power
        {
            get { return lamp1power; }
            set
            {
                lamp1power = value;
                NotifyPropertyChanged("Lamp1Power");
            }
        }

        private int lampscount = int.MaxValue;
        public int LampsCount
        {
            get { return lampscount; }
            set
            {
                lampscount = value;
                switch (value)
                {
                    case 0:
                        Lamp1Visibility = Lamp2Visibility = Visibility.Collapsed;
                        Lamp1Power = Lamp2Power = 0;
                        break;
                    case 1:
                        Lamp1Visibility = Visibility.Visible;
                        Lamp2Visibility = Visibility.Collapsed;
                        Lamp2Power = 0;
                        break;
                    case 2:
                        Lamp1Visibility = Lamp2Visibility = Visibility.Visible;
                        break;
                    default:
                        Lamp1Visibility = Lamp2Visibility = Visibility.Collapsed;
                        Lamp1Power = Lamp2Power = 0;
                        break;
                }
                NotifyPropertyChanged("LampsCount");
            }
        }

        private int hardwareversion = int.MaxValue;
        public int HardwareVersion
        {
            get { return hardwareversion; }
            set
            {
                hardwareversion = value;
                NotifyPropertyChanged("HardwareVersion");
            }
        }

        private int softwareversion = int.MaxValue;
        public int SoftwareVersion
        {
            get { return softwareversion; }
            set
            {
                softwareversion = value;
                NotifyPropertyChanged("SoftwareVersion");
            }
        }
    }
}

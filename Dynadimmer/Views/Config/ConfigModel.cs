using Dynadimmer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynadimmer.Models.Messages;
using System.Xml;
using System.Windows.Controls;
using System.Globalization;
using System.Windows;
using Dynadimmer.Views.Information;

namespace Dynadimmer.Views.Config
{
    public class ConfigModel : UnitProperty
    {
        public const int Header = 3;
        private bool Notify = true;

        #region Properties
        private int unitlampcount;
        public int UnitLampCount
        {
            get { return unitlampcount; }
            set
            {
                unitlampcount = value;
                CountValid = true;
                IsValid = CountValid && Lamp1Valid && Lamp2Valid;
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
                NotifyPropertyChanged("UnitLampCount");
            }
        }

        private int lamp1power;
        public int Lamp1Power
        {
            get { return lamp1power; }
            set
            {
                lamp1power = value;
                Lamp1Valid = true;
                IsValid = CountValid && Lamp1Valid && Lamp2Valid;
                NotifyPropertyChanged("Lamp1Power");
            }
        }

        private int lamp2power;
        public int Lamp2Power
        {
            get { return lamp2power; }
            set
            {
                lamp2power = value;
                Lamp2Valid = true;
                IsValid = CountValid && Lamp1Valid && Lamp2Valid;
                NotifyPropertyChanged("Lamp2Power");
            }
        }

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


        private bool isloadedandvalid;
        public bool IsLoadedAndValid
        {
            get { return isloadedandvalid; }
            set
            {
                isloadedandvalid = value;
                NotifyPropertyChanged("IsLoadedAndValid");
            }
        }

        private bool isvalid;
        public bool IsValid
        {
            get { return isvalid; }
            set
            {
                isvalid = value;
                IsLoadedAndValid = IsValid && IsLoaded;
                NotifyPropertyChanged("IsValid");
            }
        }


        public bool CountValid { get; set; }
        public bool Lamp1Valid { get; set; }
        public bool Lamp2Valid { get; set; }

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
            UnitLampCount = messase.DecimalData[2];
            Lamp1Power = BitConverter.ToUInt16(new byte[] { data[4], data[3] }, 0);
            Lamp2Power = BitConverter.ToUInt16(new byte[] { data[6], data[5] }, 0);
            if (Notify)
                OnGotData(new Information.UnitInfo() { LampsCount = UnitLampCount, Lamp1Power = Lamp1Power, Lamp2Power = Lamp2Power });
            Notify = true;
            base.SetView();
            IsValid = true;
            return Title;
        }

        protected override void OnGotData(UnitInfo info)
        {
            UnitLampCount = info.LampsCount;
            Lamp1Power = info.Lamp1Power;
            Lamp2Power = info.Lamp2Power;
            IsLoaded = true;
            IsValid = true;
            base.OnGotData(info);
        }
        
        public override void SendDownLoad(object sender)
        {
            List<byte> DATA = new List<byte>();
            if (sender is byte[])
            {
                Notify = false;
                DATA.AddRange((byte[])sender);
            }
            else
            {
                DATA.Add((byte)UnitLampCount);
                DATA.AddRange(BitConverter.GetBytes(Lamp1Power).Reverse().ToList().GetRange(2, 2));
                DATA.AddRange(BitConverter.GetBytes(Lamp2Power).Reverse().ToList().GetRange(2, 2));
            }
            CreateAndSendMessage(SendMessageType.Download, Header, DATA.ToArray());
        }

        public override void UpdateData(Information.UnitInfo info)
        {
            UnitLampCount = info.LampsCount;
            Lamp1Power = info.Lamp1Power;
            Lamp2Power = info.Lamp2Power;
            IsLoaded = true;
            IsValid = true;
        }

        public override void SendUpload(object sender)
        {
            CreateAndSendMessage(SendMessageType.Upload, Header);
        }

        public override void SaveData(XmlWriter writer, object extra)
        {
            
        }
    }
}

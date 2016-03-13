using Dynadimmer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynadimmer.Models.Messages;
using Dynadimmer.Views.Information;
using System.Xml;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Dynadimmer.Views.OnlineSaving
{
    public class OnlineSavingModel : UnitProperty
    {
        public const int Header = 15;
        public const int V1_10_Header = 16;
        public const int DaliHeader = 17;
        public const int Timer_Interval = 3;
        DispatcherTimer _timer = new DispatcherTimer();

        #region Properties


        private int tempHeader;
        public int TempHeader
        {
            get { return tempHeader; }
            set
            {
                tempHeader = value;
                NotifyPropertyChanged("TempHeader");
            }
        }



        private System.DateTime unittime;
        public System.DateTime UnitTime
        {
            get { return unittime; }
            set
            {
                unittime = value;
                NotifyPropertyChanged("UnitTime");
            }
        }

        private double luminare1Current;
        public double Luminare1Current
        {
            get { return luminare1Current; }
            set
            {
                luminare1Current = value;
                NotifyPropertyChanged("Luminare1Current");
            }
        }

        private double luminare1Planned;
        public double Luminare1Planned
        {
            get { return luminare1Planned;}
            set
            {
                luminare1Planned = value;
                NotifyPropertyChanged("Luminare1Planned");
            }
        }

        private double luminare2Current;
        public double Luminare2Current
        {
            get { return luminare2Current; }
            set
            {
                luminare2Current = value;
                NotifyPropertyChanged("Luminare2Current");
            }
        }

        private double luminare2Planned;
        public double Luminare2Planned
        {
            get { return luminare2Planned; }
            set
            {
                luminare2Planned = value;
                NotifyPropertyChanged("Luminare2Planned");
            }
        }

        private int baseCalc;
        public int BaseCalc
        {
            get { return baseCalc; }
            set
            {
                baseCalc = value;
                NotifyPropertyChanged("BaseCalc");
            }
        }

        private int unitLampCount;
        public int UnitLampCount
        {
            get { return unitLampCount; }
            set
            {
                unitLampCount = value >= 0 && value <=2 ? value : unitLampCount;
                NotifyPropertyChanged("UnitLampCount");
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

        #endregion


        public OnlineSavingModel () : base()
        {
            Title = "Unit Online Saving";
            SetTimer();
            _timer.Start();
            IsLoaded = true; 
        }

        public override void DidntGotAnswer()
        {
            throw new NotImplementedException();
        }

        public override string GotAnswer(IncomeMessage message)
        {
            byte[] data = message.DecimalData;

            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("he-IL");
            UnitTime = System.DateTime.Parse(String.Format("{0}/{1}/{2} {3}:{4}:{5}", data[2], data[3], data[4], data[5], data[6], data[7]), cultureinfo);
            
            BaseCalc = BitConverter.ToUInt16(new byte[] { data[9], data[8] }, 0);
            int lamp1current = BitConverter.ToUInt16(new byte[] { data[11], data[10] }, 0);
            int lamp1planned = BitConverter.ToUInt16(new byte[] { data[13], data[12] }, 0);
            int lamp2current = BitConverter.ToUInt16(new byte[] { data[15], data[14] }, 0);
            int lamp2planned = BitConverter.ToUInt16(new byte[] { data[17], data[16] }, 0);
            
            Luminare1Current = Math.Round((double)(BaseCalc - lamp1current) * 100 / BaseCalc);
            Luminare1Planned = Math.Round((double)(BaseCalc - lamp1planned) * 100 / BaseCalc);
            Luminare2Current = Math.Round((double)(BaseCalc - lamp2current) * 100 / BaseCalc);
            Luminare2Planned = Math.Round((double)(BaseCalc - lamp2planned) * 100 / BaseCalc);

            OnGotData(new UnitInfo() { LampsCount = UnitLampCount, UnitClock = UnitTime });

            return Title;
        }

      

        public override void SaveData(XmlWriter writer, object extra)
        {
            throw new NotImplementedException();
        }

        public override void SendDownLoad(object sender)
        {
            throw new NotImplementedException();
        }

        public override void SendUpload(object sender)
        {  
            CreateAndSendMessage(SendMessageType.Upload, (byte)TempHeader);
        }

        public override void UpdateData(Information.UnitInfo info)
        {
            UnitTime = info.UnitClock > UnitTime ? info.UnitClock : UnitTime;
            UnitLampCount = info.LampsCount;
            Lamp1Visibility = UnitLampCount == 1 || UnitLampCount == 2 ? Visibility.Visible : Visibility.Collapsed;
            Lamp2Visibility = UnitLampCount == 2 ? Visibility.Visible : Visibility.Collapsed;
            
        }

        protected override void OnGotData(UnitInfo info)
        {
            UnitTime = info.UnitClock;
            UnitLampCount = info.LampsCount;
            Lamp1Visibility = UnitLampCount == 1 || UnitLampCount == 2 ? Visibility.Visible : Visibility.Collapsed;
            Lamp2Visibility = UnitLampCount == 2 ? Visibility.Visible : Visibility.Collapsed;
            IsLoaded = true;
            base.OnGotData(info);
        }


        private void _timer_Tick(object sender, EventArgs e)
        {
            SendUpload(sender);
        }

        private void SetTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(Timer_Interval);
            _timer.Tick += _timer_Tick;
           
        }

        public void SetTimerState(bool timerState)
        {
            
            if (timerState)
                _timer.Start();
            else
                _timer.Stop();
        }

      
    }
}

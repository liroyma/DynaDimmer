﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Dynadimmer.Models;
using Dynadimmer.Models.Messages;
using System.Xml;
using Xceed.Wpf.Toolkit;
using Dynadimmer.Views.Information;

namespace Dynadimmer.Views.DateTime
{
    public class UnitDateTimeModel : UnitProperty
    {
        public const int Header = 1;

        DispatcherTimer _timer = new DispatcherTimer();
        DispatcherTimer _sendtimer = new DispatcherTimer();
        System.DateTime _date;

        #region Properties
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

        private string computertime;
        public string ComputerTime
        {
            get { return computertime; }
            set
            {
                computertime = value;
                NotifyPropertyChanged("ComputerTime");
            }
        }
        #endregion

        public UnitDateTimeModel()
        {
            Title = "Unit Time";
            SetTimers();
            _timer.Start();
            IsLoaded = true;
        }

        public override void SendDownLoad(object sender)
        {
            List<byte> DATA = new List<byte>();
            DATA.Add((byte)((Int32)_date.DayOfWeek + 1));
            DATA.Add((byte)_date.Day);
            DATA.Add((byte)_date.Month);
            int zz = int.Parse(_date.ToString("yy"));
            if (zz == 0)
            {
                MessageBox.Show("Year can be equal to 0");
                return;
            }

            DATA.Add((byte)(zz));
            DATA.Add((byte)_date.Hour);
            DATA.Add((byte)_date.Minute);
            DATA.Add((byte)_date.Second);
            CreateAndSendMessage(SendMessageType.Download, Header, DATA.ToArray());
        }

        public override void SendUpload(object sender)
        {
            CreateAndSendMessage(SendMessageType.Upload, Header);
        }

        public override string GotAnswer(IncomeMessage messase)
        {
            byte[] data = messase.DecimalData;
            string dateString = String.Format("{0}/{1}/{2} {3}:{4}:{5}", data[3], data[4], data[5], data[6], data[7], data[8]);
            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("he-IL");
            UnitTime = System.DateTime.Parse(dateString, cultureinfo);
            base.SetView();
            OnGotData(new UnitInfo() { UnitClock = UnitTime });
            return Title;
        }

        public override void DidntGotAnswer()
        {
        }

        public override void SaveData(XmlWriter writer, object extra)
        {
        }

        public override void UpdateData(UnitInfo info)
        {
            UnitTime = info.UnitClock;
        }

        protected override void OnGotData(UnitInfo info)
        {
            UnitTime = info.UnitClock;
            IsLoaded = true;

            base.OnGotData(info);
        }



        #region Timers
        private void SetTimers()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += _timer_Tick;
            _sendtimer.Interval = TimeSpan.FromSeconds(30);
            _sendtimer.Tick += _sendtimer_Tick;
        }

        private void _sendtimer_Tick(object sender, EventArgs e)
        {
            CreateAndSendMessage(SendMessageType.Upload, Header);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _date = System.DateTime.Now;
            ComputerTime = _date.DayOfWeek + " - " + _date.ToString("dd/MM/yy HH:mm:ss");
        }

        internal void UpdateData(System.DateTime unitClock)
        {
            UnitTime = unitClock;
            IsLoaded = true;
        }

        public void SendingClock(bool tosend)
        {
            if (tosend)
                _sendtimer.Start();
            else
                _sendtimer.Stop();
        }
        #endregion

    }
}

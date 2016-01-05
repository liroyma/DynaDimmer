using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Dynadimmer.Models;
using Dynadimmer.Models.Messages;
using System.Xml;

namespace Dynadimmer.Views.DateTime
{
    class UnitDateTimeModel : UnitProperty
    {
        public const int Header = 1;

        DispatcherTimer _timer = new DispatcherTimer();
        System.DateTime _date;

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
            SetTimer();
            _timer.Start();
            IsLoaded = true;
        }

        public override void SendDownLoad(object sender)
        {
            List<byte> DATA = new List<byte>();
            DATA.Add((byte)((Int32)_date.DayOfWeek + 1));
            DATA.Add((byte)_date.Day);
            DATA.Add((byte)_date.Month);
            DATA.Add((byte)(_date.Year % 2000));
            DATA.Add((byte)_date.Hour);
            DATA.Add((byte)_date.Minute);
            DATA.Add((byte)_date.Second);
            CreateAndSendMessage(SendMessageType.Download,Header, DATA.ToArray());
        }

        public override void SendUpload(object sender)
        {
            CreateAndSendMessage(SendMessageType.Upalod,Header);
        }

        public override string GotAnswer(IncomeMessage messase)
        {
            byte[] data = messase.DecimalData;
            string dateString = String.Format("{0}/{1}/{2} {3}:{4}:{5}", data[3], data[4], data[5], data[6], data[7], data[8]);
            System.DateTime date = System.DateTime.Parse(dateString);
            UnitTime = date.DayOfWeek + " - " + date.ToString("dd/MM/yy HH:mm:ss");
            base.SetView();
            return Title;
        }

        public override void DidntGotAnswer()
        {
        }

        public override void SaveData(XmlWriter writer, object extra)
        {
        }

        #region Timer
        private void SetTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += _timer_Tick;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _date = System.DateTime.Now;
            ComputerTime = _date.DayOfWeek + " - " + _date.ToString("dd/MM/yy HH:mm:ss");
        }

        #endregion

    }
}

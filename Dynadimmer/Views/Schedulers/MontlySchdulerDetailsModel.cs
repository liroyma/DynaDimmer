using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Dynadimmer.Models;
using Dynadimmer.Models.Messages;
using Dynadimmer.Views.SchdularSelection;
using Xceed.Wpf.Toolkit;

namespace Dynadimmer.Views.Schedulers.Inner
{
    class MontlySchdulerDetailsModel : UnitProperty
    {
        private const int UploadHeader = 12;
        private const int DownloadHeader = 11;
        private const int ENDVALUE = 1440;

        private const int STARTHOUR = 15;
        private const int STARTMINUTE = 00;

        public event EventHandler UpdateView;


        #region Commands
        public MyCommand Add { get; set; }
        public MyCommand Remove { get; set; }
        public MyCommand Close { get; set; }
        #endregion

        #region Properties
        private int lamp;
        public int Lamp
        {
            get { return lamp; }
            set
            {
                lamp = value;
                NotifyPropertyChanged("Lamp");
            }
        }

        private int month;
        public int Month
        {
            get { return month; }
            set
            {
                month = value;
                NotifyPropertyChanged("Month");
            }
        }

        private int illuminance;
        public int Illuminance
        {
            get { return illuminance; }
            set
            {
                illuminance = value;
                NotifyPropertyChanged("Illuminance");
            }
        }

        private string monthstring;
        public string MonthString
        {
            get { return monthstring; }
            set
            {
                monthstring = value;
                NotifyPropertyChanged("MonthString");
            }
        }

        private Visibility showremovenutton;
        public Visibility ShowRemoveButton
        {
            get { return showremovenutton; }
            set
            {
                showremovenutton = value;
                NotifyPropertyChanged("ShowRemoveButton");
            }
        }


        private LampTime selectedlamptime;
        public LampTime SelectedLampTime
        {
            get { return selectedlamptime; }
            set
            {
                selectedlamptime = value;
                ShowRemoveButton = value != null ? Visibility.Visible : Visibility.Hidden;
                NotifyPropertyChanged("SelectedLampTime");
            }
        }

        private ObservableCollection<LampTime> lamptimes;
        public ObservableCollection<LampTime> LampTimes
        {
            get { return lamptimes; }
            set
            {
                lamptimes = value;
                NotifyPropertyChanged("LampTimes");
            }
        }

        private System.Windows.Media.Brush bordercolor;
        public System.Windows.Media.Brush BorderColor
        {
            get { return bordercolor; }
            set
            {
                bordercolor = value;
                NotifyPropertyChanged("BorderColor");
            }
        }

        public List<LampTime> BeforeStart = new List<LampTime>();
        public List<LampTime> AfterStart = new List<LampTime>();


        private Visibility itemvisablility;
        public Visibility ItemVisablility
        {
            get { return itemvisablility; }
            set
            {
                itemvisablility = value;
                NotifyPropertyChanged("ItemVisablility");
            }
        }

        #endregion

        public MontlySchdulerDetailsModel(Lamp lamp, Month month) : base()
        {
            Lamp = (int)lamp;
            Month = (int)month;
            MonthString = month.ToString();
            BorderColor = GetBorderColor(Lamp);
            Title = string.Format("Lamp {0} - {1}", Lamp + 1, MonthString);
            Add = new MyCommand();
            Add.CommandSent += Add_CommandSent;
            Close = new MyCommand();
            Close.CommandSent += Close_CommandSent;
            Remove = new MyCommand();
            Remove.CommandSent += Remove_CommandSent;
            SelectedLampTime = null;
            Illuminance = 100;
            lamptimes = new ObservableCollection<LampTime>();
            ItemVisablility = Visibility.Collapsed;
        }

        private void Close_CommandSent(object sender, EventArgs e)
        {
            ItemVisablility = Visibility.Collapsed;
        }

        private Brush GetBorderColor(int lamp)
        {
            switch (lamp)
            {
                case 0:
                case 2:
                    return Brushes.LightBlue;
                case 1:
                case 3:
                    return Brushes.LightCoral;
                default:
                    return Brushes.LightCyan;
            }
        }

        private void Remove_CommandSent(object sender, EventArgs e)
        {
            if (sender is LampTime)
            {
                LampTime lt = (LampTime)sender;
                if (BeforeStart.Contains(lt))
                    BeforeStart.Remove(lt);
                if (AfterStart.Contains(lt))
                    AfterStart.Remove(lt);
                UpadteList();
            }
        }

        private void UpadteList()
        {
            LampTimes.Clear();
            foreach (var item in AfterStart.OrderBy(x => x.Minute).OrderBy(x => x.Hour))
            {
                LampTimes.Add(item);
            }
            foreach (var item in BeforeStart.OrderBy(x => x.Minute).OrderBy(x => x.Hour))
            {
                LampTimes.Add(item);
            }
            ItemVisablility = Visibility.Visible;
            UpdateView(null, null);
        }

        private void Add_CommandSent(object sender, EventArgs e)
        {
            SelectedLampTime = null;

            if (LampTimes.Count >= 10)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Max items: 10", "", System.Windows.MessageBoxButton.OK);
                return;
            }
            if (sender == null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please select a time", "", System.Windows.MessageBoxButton.OK);
                return;
            }
            LampTime lt = new LampTime((string)sender, Illuminance);
            LampTime temp = LampTimes.Where(x => x.TimeString == lt.TimeString).FirstOrDefault();
            if (temp != null)
            {
                if (temp.Precentage == lt.Precentage)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Time Alredy Exsists.", "", System.Windows.MessageBoxButton.OK);
                    return;
                }
                System.Windows.MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Time Alredy Exsists.\n Do tou want to update?", "", System.Windows.MessageBoxButton.YesNo);
                if (result != System.Windows.MessageBoxResult.Yes)
                {
                    return;
                }
                if (BeforeStart.Contains(temp))
                    BeforeStart.Remove(temp);
                if (AfterStart.Contains(temp))
                    AfterStart.Remove(temp);
            }

            if (lt.Hour == STARTHOUR)
            {
                if (lt.Minute > STARTMINUTE)
                {
                    AfterStart.Add(lt);
                }
                else
                {
                    BeforeStart.Add(lt);
                }
            }
            else if (lt.Hour > STARTHOUR)
            {
                AfterStart.Add(lt);
            }
            else
            {
                BeforeStart.Add(lt);
            }
            UpadteList();
        }

        public override void SendDownLoad(object sender)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)Lamp);
            data.Add((byte)Month);
            foreach (var item in LampTimes)
            {
                int x = item.Hour * 60 + item.Minute;
                byte[] bytes = BitConverter.GetBytes(x).ToList().GetRange(0, 2).ToArray();
                data.Add(bytes[1]);
                data.Add(bytes[0]);
                //data.Add((byte)item.Hour);
                //data.Add((byte)item.Minute);
                data.Add((byte)item.Precentage);
            }
            if (LampTimes.Count < 10)
            {
                int x = ENDVALUE;// 24 * 60 + 60;
                byte[] bytes = BitConverter.GetBytes(x).ToList().GetRange(0, 2).ToArray();
                data.Add(bytes[1]);
                data.Add(bytes[0]);
                //data.Add(24);
                //data.Add(60);
                data.Add(100);
            }
            CreateAndSendMessage(this, DownloadHeader, data.ToArray());
        }

        public override void SendUpload(object sender)
        {
            CreateAndSendMessage(this, UploadHeader, new byte[] { (byte)Lamp, (byte)Month });
        }

        public override void GotAnswer(IncomeMessage messase)
        {
            AfterStart.Clear();
            BeforeStart.Clear();
            byte[] data = messase.DecimalData;

            if (data[1] == UploadHeader)
            {
                bool ended = false;
                int index = 4;
                while (!ended)
                {
                    byte[] bytes = { data[index + 1], data[index] };
                    int time = BitConverter.ToInt16(bytes, 0);
                    int pre = data[index + 2];
                    index += 3;
                    if (time == ENDVALUE)
                        break;
                    if(index > data.Length - 3)
                        ended = true;
                    //LampTime lt = new LampTime(data[index], data[index + 1], data[index + 2]);
                    LampTime lt = new LampTime(time, pre);
                    if (lt.Hour == STARTHOUR)
                    {
                        if (lt.Minute > STARTMINUTE)
                        {
                            AfterStart.Add(lt);
                        }
                        else
                        {
                            BeforeStart.Add(lt);
                        }
                    }
                    else if (lt.Hour > STARTHOUR)
                    {
                        AfterStart.Add(lt);
                    }
                    else
                    {
                        BeforeStart.Add(lt);
                    }
                }
                UpadteList();
                base.SetView();
            }

        }

        public override void DidntGotAnswer()
        {
            IsLoaded = true;
        }


    }
}

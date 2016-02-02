using Dynadimmer.Models;
using Dynadimmer.Views.LampItem;
using Dynadimmer.Views.NewSchdularSelection;
using Dynadimmer.Views.Schedulers;
using Dynadimmer.Views.Schedulers.Inner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace Dynadimmer.Views.MonthItem
{
    public class MonthModel : MyUIHandler
    {
        public static UnitProperty Perent { get; set; }

        public event EventHandler itemchanged;
        public event EventHandler<Visibility> IsVisibleChanged;
        public event EventHandler Loaded;

        public int MonthDays { get; set; }

        private Canvas GraghCanvas;
        private byte[] MessageConstData = new byte[2];

        private const int ENDVALUE = 1440;

        bool addorremove = false;

        #region Commands
        public MyCommand Upload { get; set; }
        public MyCommand Download { get; set; }
        public MyCommand Add { get; set; }
        public MyCommand Remove { get; set; }
        public MyCommand Close { get; set; }
        public MyCommand Copy { get; set; }
        public MyCommand Paste { get; set; }
        #endregion

        #region Properties
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private string monthstring;
        public string MonthString
        {
            get { return monthstring; }
            set
            {
                monthstring = value;
                Title = string.Format("{0} - {1}", LampName, MonthString);
                NotifyPropertyChanged("MonthString");
            }
        }

        private string _lampname;
        public string LampName
        {
            get { return _lampname; }
            set
            {
                _lampname = value;
                Title = string.Format("{0} - {1}", LampName, MonthString);
                NotifyPropertyChanged("LampName");
            }
        }

        private bool _fromfile;
        public bool FromFile
        {
            get { return _fromfile; }
            set
            {
                _fromfile = value;
                ButtonsColumnWidth = value ? 0 : 100;
                NotifyPropertyChanged("FromFile");
            }
        }

        private double _buttonscolumnwidth;
        public double ButtonsColumnWidth
        {
            get { return _buttonscolumnwidth; }
            set
            {
                _buttonscolumnwidth = value;
                NotifyPropertyChanged("ButtonsColumnWidth");
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

        public List<LampTime> BeforeStart = new List<LampTime>();
        public List<LampTime> AfterStart = new List<LampTime>();

        private Visibility itemvisablility;
        public Visibility ItemVisablility
        {
            get { return itemvisablility; }
            set
            {
                if (IsVisibleChanged != null && itemvisablility != value)
                    IsVisibleChanged(null, value);
                itemvisablility = value;
                NotifyPropertyChanged("ItemVisablility");
            }
        }

        private bool _CanPaste;
        public bool CanPaste
        {
            get { return _CanPaste; }
            set
            {
                _CanPaste = value;
                NotifyPropertyChanged("CanPaste");
            }
        }

        private bool _itemupdated;
        public bool ItemUpdated
        {
            get { return _itemupdated; }
            set
            {
                _itemupdated = value;
                StarVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                if (addorremove && itemchanged != null)
                {
                    itemchanged(null, null);
                    addorremove = false;
                }
                NotifyPropertyChanged("ItemUpdated");
            }
        }

        private Visibility _starvisibility;
        public Visibility StarVisibility
        {
            get { return _starvisibility; }
            set
            {
                _starvisibility = value;
                NotifyPropertyChanged("StarVisibility");
            }
        }

        private bool _isloaded;
        public bool IsLoaded
        {
            get { return _isloaded; }
            set
            {
                _isloaded = value;
                if (Loaded != null)
                    Loaded(null, null);
                NotifyPropertyChanged("IsLoaded");
            }
        }

        private LampTime _endtime;
        public LampTime EndTime
        {
            get { return _endtime; }
            set
            {
                _endtime = value;
                NotifyPropertyChanged("EndTime");
            }
        }


        private string _endtimestring;
        public string EndTimeString
        {
            get { return _endtimestring; }
            set
            {
                string oldvalue = _endtimestring;
                string[] splited = value.Split(':');
                if (splited.Length == 2 && splited[0] != string.Empty && splited[1] != string.Empty)
                {
                    if (EndTime == null)
                        EndTime = new LampTime(value, 100);
                    else
                        EndTime.UpdateTime(value);
                    if (LampTimes.Count > 0 && EndTime.date <= LampTimes.Last().date)
                    {
                        EndTime.UpdateTime(oldvalue);
                        return;
                    }
                    UpdateView();
                }
                _endtimestring = value;
                NotifyPropertyChanged("EndTimeString");
            }
        }


        #endregion

        public MonthModel()
        {
            Add = new MyCommand();
            Add.CommandSent += Add_CommandSent;
            Close = new MyCommand();
            Close.CommandSent += Close_CommandSent;
            Remove = new MyCommand();
            Remove.CommandSent += Remove_CommandSent;
            Copy = new MyCommand();
            Copy.CommandSent += Copy_CommandSent;
            Paste = new MyCommand();
            Paste.CommandSent += Paste_CommandSent;
            Upload = new MyCommand();
            Upload.CommandSent += Upload_CommandSent;
            Download = new MyCommand();
            Download.CommandSent += Download_CommandSent;
            SelectedLampTime = null;
            Illuminance = 100;
            LampTimes = new ObservableCollection<LampTime>();
            FromFile = false;
            ItemVisablility = Visibility.Collapsed;
            EndTimeString = "08:00";
        }

        internal void Init(Month month, Canvas gragh)
        {
            MessageConstData[1] = (byte)month;
            MonthString = month.ToString();
            MonthDays = GetDays(month);
            GraghCanvas = gragh;
        }

        private int GetDays(Month month)
        {
            switch (month)
            {
                case Month.February:
                    return 28;
                case Month.April:
                case Month.June:
                case Month.September:
                case Month.November:
                    return 30;
                case Month.January:
                case Month.March:
                case Month.May:
                case Month.July:
                case Month.August:
                case Month.October:
                case Month.December:
                    return 31;
                default:
                    return 31;
            }
        }

        internal void SetLamp(LampModel lamp)
        {
            MessageConstData[0] = (byte)lamp.Index;
            LampName = lamp.Name;
        }

        internal byte[] GetUploadData()
        {
            return MessageConstData;
        }

        public void SetData(List<byte> data)
        {
            data.RemoveRange(0, 2);
            FromFile = false;
            AfterStart.Clear();
            BeforeStart.Clear();
            for (int i = 0; i < data.Count; i += 3)
            {
                byte[] bytes = { data[i + 1], data[i] };
                int time = BitConverter.ToInt16(bytes, 0);
                if (i + 2 > data.Count - 1)
                {
                    if (time == ENDVALUE)
                         EndTimeString = string.Format("{0}:{1}", 7.ToString("D2"), 0.ToString("D2"));
                        //EndTimeString = string.Format("{0}:{1}", 7, 0);
                    else
                        EndTimeString = string.Format("{0:D2}:{1:D2}", time / 60, time % 60);
                    break;
                }
                int pre = data[i + 2];
                LampTime lt = new LampTime(time, pre);
                if (lt.date.Hour == LampTime.STARTHOUR)
                {
                    if (lt.date.Minute >= LampTime.STARTMINUTE)
                    {
                        AfterStart.Add(lt);
                    }
                    else
                    {
                        BeforeStart.Add(lt);
                    }
                }
                else if (lt.date.Hour > LampTime.STARTHOUR)
                {
                    AfterStart.Add(lt);
                }
                else
                {
                    BeforeStart.Add(lt);
                }
            }
            UpadteList();
            IsLoaded = true;
            ItemUpdated = false;
        }

        private void UpadteList()
        {
            LampTimes.Clear();
            foreach (var item in AfterStart.OrderBy(x => x.date))
            {
                LampTimes.Add(item);
            }
            foreach (var item in BeforeStart.OrderBy(x => x.date))
            {
                LampTimes.Add(item);
            }
            ItemVisablility = Visibility.Visible;
            ItemUpdated = true;
            UpdateView();
        }

        internal void UpdateView()
        {
            if (GraghCanvas == null)
                return;
            GraghCanvas.Children.Clear();

            double width = GraghCanvas.ActualWidth;
            double height = GraghCanvas.ActualHeight;
            if (width <= 0 || height <= 0)
                return;

            double graphwidth = width - 40;

            double currentLeft = 20;

            double firstandlaststep = graphwidth / (LampTimes.Count + 2);
            double oneminutewidth = 0;


            TextBlock starttextBlock = new TextBlock();
            starttextBlock.Text = "On";
            Canvas.SetBottom(starttextBlock, 0);
            Canvas.SetLeft(starttextBlock, currentLeft - 10);

            TextBlock endtextBlock = new TextBlock();
            endtextBlock.Text = "Off";
            Canvas.SetBottom(endtextBlock, 0);
            Canvas.SetLeft(endtextBlock, width - 30);

            BarView startbar = new BarView();
            startbar.Height = height - 40;
            startbar.Width = firstandlaststep;
            startbar.Precentage = 100;
            Canvas.SetBottom(startbar, 20);
            Canvas.SetLeft(startbar, currentLeft);


            GraghCanvas.Children.Add(startbar);
            GraghCanvas.Children.Add(starttextBlock);
            GraghCanvas.Children.Add(endtextBlock);
            if (LampTimes.Count == 0)
            {
                startbar.Width = firstandlaststep * 2;
                return;
            }
            oneminutewidth = (graphwidth - (firstandlaststep * 2)) / CalcTimeSpan(LampTimes.First(), EndTime);

            currentLeft += startbar.Width;
            for (int i = 0; i < LampTimes.Count; i++)
            {
                double step;
                if (LampTimes.Last() == LampTimes[i])
                    step = CalcTimeSpan(LampTimes[i], EndTime) * oneminutewidth;
                else
                    step = CalcTimeSpan(LampTimes[i], LampTimes[i + 1]) * oneminutewidth;
                if (step < 40)
                {
                    firstandlaststep -= (40 - step);
                    step = 40;
                }
                TextBlock textBlock = new TextBlock();
                textBlock.Text = LampTimes[i].TimeString;
                Canvas.SetLeft(textBlock, currentLeft - 10);
                Canvas.SetBottom(textBlock, 0);

                BarView bar1 = new BarView();
                bar1.Height = height - 40;
                bar1.Width = step;
                bar1.Precentage = LampTimes[i].Precentage;
                Canvas.SetBottom(bar1, 20);
                Canvas.SetLeft(bar1, currentLeft);

                GraghCanvas.Children.Add(bar1);
                GraghCanvas.Children.Add(textBlock);
                currentLeft += bar1.Width;
            }

            TextBlock lasttextBlock = new TextBlock();
            lasttextBlock.Text = EndTime.TimeString;
            Canvas.SetLeft(lasttextBlock, currentLeft - 10);
            Canvas.SetBottom(lasttextBlock, 0);

            BarView endbar = new BarView();
            endbar.Height = height - 40;
            endbar.Width = firstandlaststep > 0 ? firstandlaststep : 0;
            endbar.Precentage = EndTime.Precentage;
            Canvas.SetBottom(endbar, 20);
            Canvas.SetLeft(endbar, currentLeft);

            GraghCanvas.Children.Add(endbar);
            GraghCanvas.Children.Add(lasttextBlock);
        }

        private double CalcTimeSpan(LampTime one, LampTime two)
        {
            TimeSpan span = two.date - one.date;
            return (two.date - one.date).TotalMinutes;
            /* if (two.Hour < one.Hour)
                 return (((24 + two.Hour) - one.Hour) * 60) + (two.Minute - one.Minute);
             return ((two.Hour - one.Hour) * 60) + (two.Minute - one.Minute);*/
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

            if (lt.date.Hour == LampTime.STARTHOUR)
            {
                if (lt.date.Minute >= LampTime.STARTMINUTE)
                {
                    AfterStart.Add(lt);
                }
                else
                {
                    BeforeStart.Add(lt);
                }
            }
            else if (lt.date.Hour > LampTime.STARTHOUR)
            {
                AfterStart.Add(lt);
            }
            else
            {
                BeforeStart.Add(lt);
            }
            addorremove = true;
            UpadteList();
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
                if (itemchanged != null)
                    itemchanged(null, null);
                addorremove = true;
                UpadteList();
            }
        }

        private void Paste_CommandSent(object sender, EventArgs e)
        {
            if (!(Perent is NewSchedularSelectionModel))
                return;
            AfterStart.Clear();
            BeforeStart.Clear();
            foreach (var lt in ((NewSchedularSelectionModel)Perent).CopiedList)
            {
                if (lt.date.Hour == LampTime.STARTHOUR)
                {
                    if (lt.date.Minute >= LampTime.STARTMINUTE)
                    {
                        AfterStart.Add(lt);
                    }
                    else
                    {
                        BeforeStart.Add(lt);
                    }
                }
                else if (lt.date.Hour > LampTime.STARTHOUR)
                {
                    AfterStart.Add(lt);
                }
                else
                {
                    BeforeStart.Add(lt);
                }
            }
            UpadteList();
        }

        public byte[] GetData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(MessageConstData);
            foreach (var item in LampTimes)
            {
                int x = item.date.Hour * 60 + item.date.Minute;
                byte[] bytes = BitConverter.GetBytes(x).ToList().GetRange(0, 2).ToArray();
                data.Add(bytes[1]);
                data.Add(bytes[0]);
                data.Add((byte)item.Precentage);
            }
            int endint = EndTime.date.Hour * 60 + EndTime.date.Minute;
            byte[] endbytes = BitConverter.GetBytes(endint).ToList().GetRange(0, 2).ToArray();
            data.Add(endbytes[1]);
            data.Add(endbytes[0]);
            return data.ToArray();
        }

        private void Copy_CommandSent(object sender, EventArgs e)
        {
            if (!(Perent is NewSchedularSelectionModel))
                return;
            ((NewSchedularSelectionModel)Perent).Copy(this);
        }

        private void Close_CommandSent(object sender, EventArgs e)
        {
            ItemVisablility = Visibility.Collapsed;
        }

        private void Upload_CommandSent(object sender, EventArgs e)
        {
            Perent.Title = this.Title;
            Perent.SendUpload(MessageConstData);
        }

        public void Download_CommandSent(object sender, EventArgs e)
        {
            Perent.Title = this.Title;
            Perent.SendDownLoad(GetData());
        }

        internal void SetTimes(XmlNodeList childNodes)
        {
            FromFile = true;
            AfterStart.Clear();
            BeforeStart.Clear();
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "EndTime")
                {
                    EndTimeString = node.Attributes["Time"].Value;
                    break;
                }
                string time = node.Attributes["Time"].Value;
                int pre = int.Parse(node.Attributes["Precentage"].Value);
                LampTime lt = new LampTime(time, pre);
                if (lt.date.Hour == LampTime.STARTHOUR)
                {
                    if (lt.date.Minute >= LampTime.STARTMINUTE)
                    {
                        AfterStart.Add(lt);
                    }
                    else
                    {
                        BeforeStart.Add(lt);
                    }
                }
                else if (lt.date.Hour > LampTime.STARTHOUR)
                {
                    AfterStart.Add(lt);
                }
                else
                {
                    BeforeStart.Add(lt);
                }
            }
            UpadteList();
            IsLoaded = true;
            ItemUpdated = false;
        }
    }
}

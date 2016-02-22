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
                IlluminanceError = false;
                NotifyPropertyChanged("Illuminance");
            }
        }

        private bool illuminanceerror;
        public bool IlluminanceError
        {
            get { return illuminanceerror; }
            set
            {
                illuminanceerror = value;
                CanAdd = EndTimeValue > StartTimeValue && !IlluminanceError;
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
                ItemUpdated = addorremove = true;
                NotifyPropertyChanged("EndTime");
            }
        }

        private System.DateTime endtimevalue;
        public System.DateTime EndTimeValue
        {
            get { return endtimevalue; }
            set
            {
                if ((LampTimes.Count != 0 && LampTimes.Last().Date >= LampTime.GetRightTime(value)) || (value.Hour == 15 && value.Minute == 00))
                {
                    return;
                }
                endtimevalue = LampTime.GetRightTime(value);
                EndTime = new LampTime(value, 100);
                CanAdd = EndTimeValue > StartTimeValue && !IlluminanceError;
                UpdateView();
                NotifyPropertyChanged("EndTimeValue");
            }
        }

        private System.DateTime starttimevalue;
        public System.DateTime StartTimeValue
        {
            get { return starttimevalue; }
            set
            {
                starttimevalue = LampTime.GetRightTime(value);
                CanAdd = EndTimeValue > StartTimeValue && !IlluminanceError;
                NotifyPropertyChanged("StartTimeValue");
            }
        }

        private bool canadd;
        public bool CanAdd
        {
            get { return canadd; }
            set
            {
                canadd = value;
                NotifyPropertyChanged("CanAdd");
            }
        }


        private bool detailsxpended;
        public bool DetailsExpended
        {
            get { return detailsxpended; }
            set
            {
                detailsxpended = value;
                NotifyPropertyChanged("DetailsExpended");
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
            EndTime = new LampTime(8, 0, 100);
            StartTimeValue = System.DateTime.Parse("15:00");
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
                        EndTimeValue = System.DateTime.Parse(string.Format("{0}:{1}", 7.ToString("D2"), 0.ToString("D2")));
                    //EndTimeString = string.Format("{0}:{1}", 7, 0);
                    else
                        EndTimeValue = System.DateTime.Parse(string.Format(string.Format("{0:D2}:{1:D2}", time / 60, time % 60)));
                    break;
                }
                int pre = data[i + 2];
                LampTime lt = new LampTime(time, pre);
                if (lt.Date.Hour == LampTime.STARTHOUR)
                {
                    if (lt.Date.Minute >= LampTime.STARTMINUTE)
                    {
                        AfterStart.Add(lt);
                    }
                    else
                    {
                        BeforeStart.Add(lt);
                    }
                }
                else if (lt.Date.Hour > LampTime.STARTHOUR)
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
            foreach (var item in AfterStart.OrderBy(x => x.Date))
            {
                LampTimes.Add(item);
            }
            foreach (var item in BeforeStart.OrderBy(x => x.Date))
            {
                LampTimes.Add(item);
            }

            ItemVisablility = Visibility.Visible;
            ItemUpdated = true;
            UpdateView();
        }

        internal void UpdateView()
        {
            List<UIElement> Elements = new List<UIElement>();
            if (GraghCanvas == null)
                return;
            GraghCanvas.Children.Clear();

            double width = GraghCanvas.ActualWidth;
            double height = GraghCanvas.ActualHeight;
            if (width <= 0 || height <= 0)
                return;
            double graphwidth = width - 40;

            double currentLeft = 20;

            double firstandlaststep = 40;
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

            Elements.Add(startbar);
            Elements.Add(starttextBlock);
            Elements.Add(endtextBlock);

            if (LampTimes.Count == 0)
            {
                startbar.Width = graphwidth;
                foreach (var item in Elements)
                {
                    GraghCanvas.Children.Add(item);
                }
                return;
            }

            BarView endbar = new BarView();
            endbar.Height = height - 40;
            endbar.Width = firstandlaststep;
            endbar.Precentage = EndTime.Precentage;
            Canvas.SetBottom(endbar, 20);
            Canvas.SetLeft(endbar, graphwidth - 20);


            TextBlock lasttextBlock = new TextBlock();
            lasttextBlock.Text = EndTime.Date.ToString("HH:mm");
            Canvas.SetLeft(lasttextBlock, Canvas.GetLeft(endbar) - 10);
            Canvas.SetBottom(lasttextBlock, 0);

            Elements.Add(endbar);
            Elements.Add(lasttextBlock);
            currentLeft += startbar.Width;
            graphwidth -= (firstandlaststep * 2);
            oneminutewidth = graphwidth / LampTime.CalcTotalHoursSpan(LampTimes.First(), EndTime);
            List<BarView> dynamibars = new List<BarView>();
            for (int i = 0; i < LampTimes.Count; i++)
            {
                BarView bar = new BarView();
                bar.SetTimes(LampTimes[i].Date, LampTimes[i] == LampTimes.Last() ? EndTime.Date : LampTimes[i + 1].Date, LampTimes[i].Precentage);
                bar.SetSize(height - 40, oneminutewidth);
                dynamibars.Add(bar);
                Canvas.SetBottom(bar, 20);
                Elements.Add(bar);
            }

            graphwidth -= (40 * dynamibars.Where(x => x.isDefultWidth == false).Count());

            oneminutewidth = graphwidth / LampTime.CalcTotalHoursSpan(LampTimes.First(), EndTime);

            foreach (var item in dynamibars.Where(x => x.isDefultWidth == true))
            {
                item.SetSize(height - 40, oneminutewidth);
            }

            foreach (var item in dynamibars)
            {
                Canvas.SetLeft(item, currentLeft);
                TextBlock textBlock = new TextBlock();
                textBlock.Text = item.StartTime.ToString("HH:mm");
                Canvas.SetLeft(textBlock, Canvas.GetLeft(item) - 10);
                Canvas.SetBottom(textBlock, 0);
                Elements.Add(textBlock);
                currentLeft += item.Width;
            }

            dynamibars.Last().Width = Canvas.GetLeft(endbar) - Canvas.GetLeft(dynamibars.Last());

            foreach (var item in Elements)
            {
                GraghCanvas.Children.Add(item);
            }
        }

        private void Add_CommandSent(object sender, EventArgs e)
        {
            SelectedLampTime = null;

            System.DateTime tempdate = LampTime.GetRightTime(StartTimeValue);
            LampTime temp = LampTimes.Where(x => x.Date == tempdate).FirstOrDefault();
            if (temp != null)
            {
                if (temp.Precentage == Illuminance)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Time Alredy Exsists.", "", System.Windows.MessageBoxButton.OK);
                    return;
                }
                System.Windows.MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Time Alredy Exsists.\n Do tou want to update?", "", System.Windows.MessageBoxButton.YesNo);
                if (result != System.Windows.MessageBoxResult.Yes)
                {
                    return;
                }
                if (temp != null)
                {
                    if (BeforeStart.Contains(temp))
                        BeforeStart.Remove(temp);
                    if (AfterStart.Contains(temp))
                        AfterStart.Remove(temp);
                }
            }

            if (BeforeStart.Count  + AfterStart.Count >= 10)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Max items: 10", "Add time error", System.Windows.MessageBoxButton.OK);
                return;
            }
            if (tempdate >= EndTime.Date)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("End Time must be after selected time", "Add time error", System.Windows.MessageBoxButton.OK);
                return;
            }


            LampTime lt = new LampTime(tempdate, Illuminance);
            if (lt.Date.Hour == LampTime.STARTHOUR)
            {
                if (lt.Date.Minute >= LampTime.STARTMINUTE)
                {
                    AfterStart.Add(lt);
                }
                else
                {
                    BeforeStart.Add(lt);
                }
            }
            else if (lt.Date.Hour > LampTime.STARTHOUR)
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
            EndTimeValue = ((NewSchedularSelectionModel)Perent).CopiedEndTime.Date;
            foreach (var lt in ((NewSchedularSelectionModel)Perent).CopiedList)
            {
                if (lt.Date.Hour == LampTime.STARTHOUR)
                {
                    if (lt.Date.Minute >= LampTime.STARTMINUTE)
                    {
                        AfterStart.Add(lt);
                    }
                    else
                    {
                        BeforeStart.Add(lt);
                    }
                }
                else if (lt.Date.Hour > LampTime.STARTHOUR)
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
                int x = item.Date.Hour * 60 + item.Date.Minute;
                byte[] bytes = BitConverter.GetBytes(x).ToList().GetRange(0, 2).ToArray();
                data.Add(bytes[1]);
                data.Add(bytes[0]);
                data.Add((byte)item.Precentage);
            }
            int endint = EndTime.Date.Hour * 60 + EndTime.Date.Minute;
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
            DetailsExpended = false;
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
                    EndTimeValue = System.DateTime.Parse(node.Attributes["Time"].Value);
                    break;
                }
                System.DateTime time = System.DateTime.Parse(node.Attributes["Time"].Value);
                int pre = int.Parse(node.Attributes["Precentage"].Value);
                LampTime lt = new LampTime(time, pre);
                if (lt.Date.Hour == LampTime.STARTHOUR)
                {
                    if (lt.Date.Minute >= LampTime.STARTMINUTE)
                    {
                        AfterStart.Add(lt);
                    }
                    else
                    {
                        BeforeStart.Add(lt);
                    }
                }
                else if (lt.Date.Hour > LampTime.STARTHOUR)
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

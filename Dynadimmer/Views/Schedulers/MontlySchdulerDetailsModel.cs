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
using Xceed.Wpf.Toolkit;
using Dynadimmer.Views.NewSchdularSelection;
using System.Windows.Controls;
using System.Xml;

namespace Dynadimmer.Views.Schedulers.Inner
{
    public class MontlySchdulerDetailsModel : MyUIHandler
    {
        public NewSchedularSelectionModel Perent { get; set; }
        private Canvas GraghCanvas;

        private const int ENDVALUE = 1440;

        private const int STARTHOUR = 15;
        private const int STARTMINUTE = 00;

        private byte[] MessageConstData;

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

        private bool _fromfile;
        public bool FromFile
        {
            get { return _fromfile; }
            set
            {
                _fromfile = value;
                ButtonsColumnWidth = value ? 0 : Double.NaN;
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

        #endregion

        public MontlySchdulerDetailsModel(NewSchedularSelectionModel perent, Canvas canvas, Lamp lamp, Month month) : base()
        {
            Perent = perent;
            GraghCanvas = canvas;
            MessageConstData = new byte[] { (byte)lamp.Index, (byte)month };
            MonthString = month.ToString();
            BorderColor = GetBorderColor(lamp.Index);
            Title = string.Format("{0} - {1}", lamp.Name, MonthString);
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
            lamptimes = new ObservableCollection<LampTime>();
            ItemVisablility = Visibility.Collapsed;
        }

        public void SetData(byte[] data)
        {
            FromFile =false;
            AfterStart.Clear();
            BeforeStart.Clear();
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
                if (index > data.Length - 3)
                    ended = true;
                //LampTime lt = new LampTime(data[index], data[index + 1], data[index + 2]);
                LampTime lt = new LampTime(time, pre);
                if (lt.Hour == STARTHOUR)
                {
                    if (lt.Minute >= STARTMINUTE)
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
        }

        internal byte[] GetUploadData()
        {
            return MessageConstData;
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
            UpdateView();
        }

        internal void UpdateView()
        {
            GraghCanvas.Children.Clear();

            double width = GraghCanvas.ActualWidth;
            double height = GraghCanvas.ActualHeight;
            if (width <= 0 || height <= 0)
                return;

            double graphwidth = width - 40;

            double currentLeft = 20;

            double firstandlaststep = graphwidth / (LampTimes.Count + 1);
            double oneminutewidth = 0;


            TextBlock starttextBlock = new TextBlock();
            starttextBlock.Text = "On";
            Canvas.SetBottom(starttextBlock, 0);
            Canvas.SetLeft(starttextBlock, currentLeft - 10);

            TextBlock endtextBlock = new TextBlock();
            endtextBlock.Text = "Off";
            Canvas.SetBottom(endtextBlock, 0);
            Canvas.SetLeft(endtextBlock, width - 30);

            BarView bar = new BarView();
            bar.Height = height - 40;
            bar.Width = firstandlaststep;
            bar.Precentage = 100;
            Canvas.SetBottom(bar, 20);
            Canvas.SetLeft(bar, currentLeft);


            GraghCanvas.Children.Add(bar);
            GraghCanvas.Children.Add(starttextBlock);
            GraghCanvas.Children.Add(endtextBlock);

            if (LampTimes.Count >= 1)
                oneminutewidth = (graphwidth - (firstandlaststep * 2)) / CalcTimeSpan(LampTimes.First(), LampTimes.Last());
            else
                return;

            currentLeft += bar.Width;
            for (int i = 0; i < LampTimes.Count - 1; i++)
            {
                double step = CalcTimeSpan(LampTimes[i], LampTimes[i + 1]) * oneminutewidth;
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
            lasttextBlock.Text = LampTimes.Last().TimeString;
            Canvas.SetLeft(lasttextBlock, currentLeft - 10);
            Canvas.SetBottom(lasttextBlock, 0);

            BarView endbar = new BarView();
            endbar.Height = height - 40;
            endbar.Width = firstandlaststep > 0 ? firstandlaststep : 0;
            endbar.Precentage = LampTimes.Last().Precentage;
            Canvas.SetBottom(endbar, 20);
            Canvas.SetLeft(endbar, currentLeft);

            GraghCanvas.Children.Add(endbar);
            GraghCanvas.Children.Add(lasttextBlock);
        }

        internal void SetTimes(XmlNodeList childNodes)
        {
            FromFile = true;
            AfterStart.Clear();
            BeforeStart.Clear();
            foreach (XmlNode node in childNodes)
            {
                string time = node.Attributes["Time"].Value;
                int pre = int.Parse(node.Attributes["Precentage"].Value);
                LampTime lt = new LampTime(time, pre);
                if (lt.Hour == STARTHOUR)
                {
                    if (lt.Minute >= STARTMINUTE)
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
        }

        private double CalcTimeSpan(LampTime one, LampTime two)
        {
            if (two.Hour < one.Hour)
                return (((24 + two.Hour) - one.Hour) * 60) + (two.Minute - one.Minute);
            return ((two.Hour - one.Hour) * 60) + (two.Minute - one.Minute);
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
                if (lt.Minute >= STARTMINUTE)
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

        private void Paste_CommandSent(object sender, EventArgs e)
        {
            AfterStart.Clear();
            BeforeStart.Clear();
            foreach (var lt in Perent.CopiedList)
            {
                if (lt.Hour == STARTHOUR)
                {
                    if (lt.Minute >= STARTMINUTE)
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
        }

        private void Copy_CommandSent(object sender, EventArgs e)
        {
            Perent.Copy(this);
        }

        private void Close_CommandSent(object sender, EventArgs e)
        {
            ItemVisablility = Visibility.Collapsed;
        }

        public void Download_CommandSent(object sender, EventArgs e)
        {
            Perent.Title = this.Title;
            Perent.SendDownLoad(GetData());
        }

        public byte[] GetData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(MessageConstData);
            foreach (var item in LampTimes)
            {
                int x = item.Hour * 60 + item.Minute;
                byte[] bytes = BitConverter.GetBytes(x).ToList().GetRange(0, 2).ToArray();
                data.Add(bytes[1]);
                data.Add(bytes[0]);
                data.Add((byte)item.Precentage);
            }
            if (LampTimes.Count < 10)
            {
                int x = ENDVALUE;
                byte[] bytes = BitConverter.GetBytes(x).ToList().GetRange(0, 2).ToArray();
                data.Add(bytes[1]);
                data.Add(bytes[0]);
                data.Add(100);
            }
            return data.ToArray();
        }

        private void Upload_CommandSent(object sender, EventArgs e)
        {
            Perent.Title = this.Title;
            Perent.SendUpload(MessageConstData);
        }
    }
}

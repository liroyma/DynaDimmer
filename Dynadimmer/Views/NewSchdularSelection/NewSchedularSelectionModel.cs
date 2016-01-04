using Dynadimmer.Models;
using Dynadimmer.Views.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Dynadimmer.Models.Messages;
using Dynadimmer.Views.Schedulers.Inner;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows.Media;

namespace Dynadimmer.Views.NewSchdularSelection
{
    public class NewSchedularSelectionModel : UnitProperty
    {
        public const int UploadHeader = 12;
        public const int DownloadHeader = 11;
        private const int NumberOfTotalLamp = 2;

        #region Commands
        public MyCommand DownLoadAll { get; set; }
        public MyCommand Close { get; set; }

        private void Close_CommandSent(object sender, EventArgs e)
        {
            FileSelectionVisibility = Visibility.Collapsed;
            foreach (var lamp in Lamps)
            {
                foreach (var month in lamp.monthsModels)
                {
                    month.ItemVisablility = Visibility.Collapsed;
                }
            }
        }

        private void DownLoadAll_CommandSent(object sender, EventArgs e)
        {
            foreach (var lamp in LampsList)
            {
                foreach (var month in lamp.monthsModels)
                {
                    month.Download_CommandSent(null, null);
                }
            }
        }
        #endregion

        List<Lamp> Lamps = new List<Lamp>();
        StackPanel Container;
        bool LoadAll = false;
        public List<LampTime> CopiedList { get; private set; }

        public Month[] MonthsList { get; set; }

        public Month SelectedMonth { get; set; }
        
        private Visibility _FileSelectionVisibility;
        public Visibility FileSelectionVisibility
        {
            get { return _FileSelectionVisibility; }
            set
            {
                _FileSelectionVisibility = value;
                LampSelectionVisibility = value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                NotifyPropertyChanged("FileSelectionVisibility");
            }
        }

        private Visibility _LampSelectionVisibility;
        public Visibility LampSelectionVisibility
        {
            get { return _LampSelectionVisibility; }
            set
            {
                _LampSelectionVisibility = value;
                NotifyPropertyChanged("LampSelectionVisibility");
            }
        }

        private Lamp selectedlamp;
        public Lamp SelectedLamp
        {
            get { return selectedlamp; }
            set
            {
                selectedlamp = value;
                NotifyPropertyChanged("SelectedLamp");
            }
        }

        private ObservableCollection<Lamp> _lamps = new ObservableCollection<Lamp>();
        public ObservableCollection<Lamp> LampsList
        {
            get { return _lamps; }
            set
            {
                _lamps = value;
                NotifyPropertyChanged("LampsList");
            }
        }

        private Visibility isvisibility;
        public Visibility WinVisibility
        {
            get { return isvisibility; }
            set
            {
                isvisibility = value;
                NotifyPropertyChanged("WinVisibility");
            }
        }

        public NewSchedularSelectionModel()
        {
            DownLoadAll = new MyCommand();
            DownLoadAll.CommandSent += DownLoadAll_CommandSent;
            Close = new MyCommand();
            Close.CommandSent += Close_CommandSent;
            MonthsList = (Month[])Enum.GetValues(typeof(Month));
            SelectedMonth = Month.January;
            WinVisibility = Visibility.Collapsed;
            FileSelectionVisibility = Visibility.Collapsed;
            for (int i = 0; i < NumberOfTotalLamp; i++)
            {
                Lamp temp = new Lamp(this, i);
                Lamps.Add(temp);
            }
        }

        public void SetContainer(StackPanel container)
        {
            Container = container;
            if (container != null)
            {
                foreach (Lamp lamp in Lamps)
                {
                    lamp.AddToView(Container);
                }
            }
        }

        public void SetNumberOfLamps(int unitLampCount)
        {
            LampsList.Clear();
            foreach (var item in Lamps)
            {
                if (item.Index < unitLampCount)
                {
                    item.isConfig = true;
                    LampsList.Add(item);
                }
                else
                {
                    item.isConfig = false;
                }
            }
            if (LampsList.Count == 0)
            {
                WinVisibility = Visibility.Collapsed;
            }
            else
            {
                SelectedLamp = LampsList[0];
                WinVisibility = Visibility.Visible;
            }
        }

        public override void DidntGotAnswer()
        {
        }

        public override string GotAnswer(IncomeMessage messase)
        {
            FileSelectionVisibility = Visibility.Collapsed;
            byte[] data = messase.DecimalData;
            Lamp templamp = Lamps.Where(x => x.Index == data[2]).FirstOrDefault();
            MontlySchdulerDetailsModel zx = templamp.FindMonthView((Month)data[3]);
            zx.SetData(data);
            if (LoadAll) zx.ItemVisablility = Visibility.Collapsed;
            return zx.Title;
        }

        public override void SendDownLoad(object sender)
        {
            CreateAndSendMessage(this, DownloadHeader, (byte[])sender);
        }

        public override void SendUpload(object sender)
        {
            LoadAll = false;
            if (sender is byte[])
                CreateAndSendMessage(this, UploadHeader, (byte[])sender);
            else if (sender is Lamp)
                StartAll((Lamp)sender);
            else
            {
                MontlySchdulerDetailsModel model = FindSchduler(SelectedLamp, SelectedMonth);
                Title = model.Title;
                CreateAndSendMessage(this, UploadHeader, model.GetUploadData());
            }
        }

        public override void SaveData(System.Xml.XmlWriter writer, object extra)
        {
            Lamp lamp = (Lamp)extra;
            writer.WriteStartElement("Lamp");
            writer.WriteAttributeString("LampName", lamp.Name);
            writer.WriteAttributeString("LampIndex", lamp.Index.ToString());
            foreach (var item in lamp.monthsModels)
            {
                writer.WriteStartElement("Month");
                writer.WriteAttributeString("Month", item.MonthString);
                foreach (var time in item.LampTimes)
                {
                    writer.WriteStartElement("Time");
                    writer.WriteAttributeString("Precentage", time.Precentage.ToString());
                    writer.WriteAttributeString("Time", time.TimeString);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        internal void AddLamp(XmlNode node)
        {
            FileSelectionVisibility = Visibility.Visible;
            int z = int.Parse(node.Attributes["LampIndex"].Value);
            Lamp templamp = Lamps.Where(x => x.Index == z).FirstOrDefault();
            templamp.ReadData(node.ChildNodes);
        }

        private void StartAll(Lamp lamp)
        {
            LoadAll = true;
            Title = lamp.Name + " - All";
            CreateAndSendMessage(this, UploadHeader, new byte[] { (byte)lamp.Index, 13 });
        }

        private MontlySchdulerDetailsModel FindSchduler(Lamp lamp, Month month)
        {
            return lamp.FindMonthView(month);
        }

        public void Copy(MontlySchdulerDetailsModel model)
        {
            CopiedList = model.LampTimes.ToList();
            foreach (Lamp lamp in Lamps)
            {
                foreach (var month in lamp.monthsModels)
                {
                    month.CanPaste = true;
                }
            }
            model.CanPaste = false;
        }
    }

    public class Lamp
    {
        public string Name { get; private set; }
        public int Index { get; private set; }
        public bool isConfig { get; set; }

        List<UserControl> views = new List<UserControl>();
        public List<MontlySchdulerDetailsModel> monthsModels = new List<MontlySchdulerDetailsModel>();

        public Lamp(NewSchedularSelectionModel perent, int index)
        {
            Name = "Lamp " + (index + 1);
            Index = index;
            isConfig = false;
            foreach (Month month in Enum.GetValues(typeof(Month)))
            {
                MontlySchdulerDetails temp = new MontlySchdulerDetails(perent, this, month);
                views.Add(temp);
                monthsModels.Add(temp.Model);
            }
        }

        public void AddToView(StackPanel stack)
        {
            foreach (var item in views)
            {
                stack.Children.Add(item);
            }
        }

        internal MontlySchdulerDetailsModel FindMonthView(Month month)
        {
            return monthsModels.Where(x => x.MonthString == month.ToString()).FirstOrDefault();
        }

        public override string ToString()
        {
            return Name;
        }

        internal void ReadData(XmlNodeList childNodes)
        {
            foreach (XmlNode node in childNodes)
            {
                Month month = (Month)Enum.Parse(typeof(Month), node.Attributes["Month"].Value);
                MontlySchdulerDetailsModel model = FindMonthView(month);
                model.SetTimes(node.ChildNodes);
            }
        }
    }

    public enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12,
    }
}

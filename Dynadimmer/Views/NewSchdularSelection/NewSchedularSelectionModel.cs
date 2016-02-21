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
using Dynadimmer.Views.MainContainer;
using Dynadimmer.Views.LampItem;
using Dynadimmer.Views.MonthItem;

namespace Dynadimmer.Views.NewSchdularSelection
{
    public class NewSchedularSelectionModel : UnitProperty
    {
        public const int UploadHeader = 12;
        public const int DownloadHeader = 11;
        public const int ResetHeader = 14;

        public string ResetTitle = "Reset Programs";

        MainContainerView Container;

        public MyCommand ResetAll { get; set; }

        public List<LampTime> CopiedList { get; private set; }
        public LampTime CopiedEndTime { get; private set; }

        public Month[] MonthsList { get; set; }
        public Month SelectedMonth { get; set; }

        private LampModel selectedlamp;
        public LampModel SelectedLamp
        {
            get { return selectedlamp; }
            set
            {
                selectedlamp = value;
                NotifyPropertyChanged("SelectedLamp");
            }
        }

        private ObservableCollection<LampModel> _lamps = new ObservableCollection<LampModel>();
        public ObservableCollection<LampModel> LampsList
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
            ResetAll = new MyCommand();
            ResetAll.CommandSent += ResetAll_CommandSent;
            MonthsList = (Month[])Enum.GetValues(typeof(Month));
            SelectedMonth = Month.January;
            WinVisibility = Visibility.Collapsed;
        }

        private void ResetAll_CommandSent(object sender, EventArgs e)
        {
            byte[] data = new byte[] { (byte)SelectedLamp.Index };
            Title = "Reset " + SelectedLamp.Name;
            CreateAndSendMessage(SendMessageType.Download, ResetHeader, data);
        }

        public void SetContainer(MainContainerView container)
        {
            Container = container;
        }

        public override void DidntGotAnswer()
        {

        }

        public string GotGaneralAnswer(IncomeMessage message)
        {
            byte[] data = message.OnlyData;
            if (data[0] == NewSchedularSelectionModel.DownloadHeader)
            {
                LampView templamp = Container.FindLamp(data[1]);
                MonthView tempmonth = templamp.FindMonth((Month)data[2]);
                tempmonth.Model.ItemUpdated = false;
                return "Changes in " + tempmonth.Model.Title + " saved.";
            }
            else if (data[0] == NewSchedularSelectionModel.ResetHeader)
            {
                LampView templamp = Container.FindLamp(data[1]);
                return "Reset All programs in " + templamp.Model.Name + " succsed.";
            }
            return "";
        }

        public override string GotAnswer(IncomeMessage messase)
        {
            byte[] data = messase.OnlyData;
            LampView templamp = Container.FindLamp(data[0]);
            MonthView tempmonth = templamp.FindMonth((Month)data[1]);
            tempmonth.Model.SetData(data.ToList());
            return tempmonth.Model.Title;

        }

        public override void SendDownLoad(object sender)
        {
            if (sender is MonthModel)
            {
                MonthModel tempmonth = sender as MonthModel;
                Title = tempmonth.Title;
                CreateAndSendMessage(SendMessageType.Download, DownloadHeader, tempmonth.GetData());
            }
            else
                CreateAndSendMessage(SendMessageType.Download, DownloadHeader, (byte[])sender);
        }

        public override void SendUpload(object sender)
        {
            
            if (sender is byte[])
                CreateAndSendMessage(SendMessageType.Upload, UploadHeader, (byte[])sender);
            else if (sender is LampModel)
                StartAll((LampModel)sender);
            else
            {
                LampView templamp = Container.FindLamp(SelectedLamp);
                templamp.ExpendedAll();
                MonthView tempmonth = templamp.FindMonth(SelectedMonth);
                Title = tempmonth.Model.Title;
                tempmonth.Model.DetailsExpended = true;
                CreateAndSendMessage(SendMessageType.Upload, UploadHeader, tempmonth.Model.GetUploadData());
            }
        }

        protected override void OnGotData(Information.UnitInfo info)
        {
            IsLoaded = true;
            base.OnGotData(info);
        }

        public override void UpdateData(Information.UnitInfo info)
        {
            LampsList.Clear();
            foreach (LampModel item in Container.GetLampsModels())
            {
                if (item.Index < info.LampsCount)
                {
                    item.isConfig = true;
                    switch (item.Index)
                    {
                        case 0:
                            item.LampPower = info.Lamp1Power;
                            break;
                        case 1:
                            item.LampPower = info.Lamp2Power;
                            break;
                    }


                    LampsList.Add(item);
                }
                else
                {
                    item.isConfig = false;
                    foreach (MonthModel month in item.GetMonths())
                    {
                        month.ItemVisablility = Visibility.Collapsed;
                    }
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

        public override void SaveData(System.Xml.XmlWriter writer, object extra)
        {
            LampModel lamp = (LampModel)extra;
            writer.WriteStartElement("Lamp");
            writer.WriteAttributeString("LampName", lamp.Name);
            writer.WriteAttributeString("LampIndex", lamp.Index.ToString());
            writer.WriteAttributeString("LampPower", lamp.LampPower.ToString());
            foreach (var item in lamp.GetMonths())
            {
                writer.WriteStartElement("Month");
                writer.WriteAttributeString("Month", item.MonthString);
                foreach (var time in item.LampTimes)
                {
                    writer.WriteStartElement("Time");
                    writer.WriteAttributeString("Precentage", time.Precentage.ToString());
                    writer.WriteAttributeString("Time", time.Date.ToString("HH:mm"));
                    writer.WriteEndElement();
                }
                writer.WriteStartElement("EndTime");
                writer.WriteAttributeString("Time", item.EndTime.Date.ToString("HH:mm"));
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void StartAll(LampModel lamp)
        {
            Title = lamp.Name + " - All";
            CreateAndSendMessage(SendMessageType.Upload, UploadHeader, new byte[] { (byte)lamp.Index, 13 });
        }

        public void Copy(MonthModel model)
        {
            CopiedList = model.LampTimes.ToList();
            CopiedEndTime = model.EndTime;
            foreach (LampView lamp in Container.GetLampsViews())
            {
                lamp.SetAllPatse(true);
            }
            model.CanPaste = false;
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

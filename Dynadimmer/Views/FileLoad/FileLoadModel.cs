using Dynadimmer.Models;
using Dynadimmer.Models.Actions;
using Dynadimmer.Models.Messages;
using Dynadimmer.Views.Config;
using Dynadimmer.Views.LampItem;
using Dynadimmer.Views.MainContainer;
using Dynadimmer.Views.MonthItem;
using Dynadimmer.Views.NewSchdularSelection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml;

namespace Dynadimmer.Views.FileLoad
{
    public class FileLoadModel : MyUIHandler
    {
        public const int DownloadHeader = 11;

        public event EventHandler<byte[]> ClickDownload;


        public event EventHandler<Visibility> WinVisibilityChanged;

        private MainContainerView Container;

        private Visibility isvisibility;
        public Visibility WinVisibility
        {
            get { return isvisibility; }
            set
            {
                isvisibility = value;
                if (WinVisibilityChanged != null)
                    WinVisibilityChanged(null, value);
                NotifyPropertyChanged("WinVisibility");
            }
        }


        private bool _downLoadenable;
        public bool DownLoadEnable
        {
            get { return _downLoadenable; }
            set
            {
                _downLoadenable = value;
                NotifyPropertyChanged("DownLoadEnable");
            }
        }


        private string _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                _FilePath = value;
                NotifyPropertyChanged("FilePath");
            }
        }

        private bool _madechanges;
        public bool MadeChanges
        {
            get { return _madechanges; }
            set
            {
                _madechanges = value;
                NotifyPropertyChanged("MadeChanges");
            }
        }

        #region Commands
        public MyCommand DownLoadAll { get; set; }
        public MyCommand Save { get; set; }
        public MyCommand Close { get; set; }

        private void Close_CommandSent(object sender, EventArgs e)
        {
            Container.Reset();
            WinVisibility = Visibility.Collapsed;
        }

        private void Save_CommandSent(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Dimmer documents (.dxml)|*.dxml";
            if (saveFileDialog.ShowDialog() != true)
                return;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(saveFileDialog.FileName, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("Dimmer");

            writer.WriteStartElement("Configutarion");
            writer.WriteAttributeString("LampCount", UnitLampCount.ToString());
            writer.WriteEndElement();

            foreach (var lampitem in Container.GetLampsViews())
            {
                if (lampitem.Model.isConfig)
                {
                    SaveData(writer, lampitem.Model);
                }
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            writer.Dispose();
            MadeChanges = false;
        }

        private void DownLoadAll_CommandSent(object sender, EventArgs e)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)UnitLampCount);
            data.AddRange(BitConverter.GetBytes(Container.Model.Lamp1Power).Reverse().ToList().GetRange(2, 2));
            data.AddRange(BitConverter.GetBytes(Container.Model.Lamp2Power).Reverse().ToList().GetRange(2, 2));
            if (ClickDownload != null)
                ClickDownload(null, data.ToArray());
        }
        #endregion

        public FileLoadModel()
        {
            DownLoadAll = new MyCommand();
            DownLoadAll.CommandSent += DownLoadAll_CommandSent;
            Save = new MyCommand();
            Save.CommandSent += Save_CommandSent;
            Close = new MyCommand();
            Close.CommandSent += Close_CommandSent;
            WinVisibility = Visibility.Collapsed;
        }

        internal void SetContainer(MainContainerView container)
        {
            Container = container;
        }

        public void ReadFromFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Filter = "Dimmer documents (.dxml)|*.dxml";
            dlg.InitialDirectory = Properties.Settings.Default.FilesPath;

            if (dlg.ShowDialog() != true)
                return;

            WinVisibility = Visibility.Visible;
            XmlDocument doc = new XmlDocument();
            doc.Load(dlg.FileName);
            FilePath = dlg.FileName;

            Container.Reset();

            XmlNodeList ConfigutarionNodes = doc.DocumentElement.SelectNodes("/Dimmer/Configutarion");
            UnitLampCount = int.Parse(ConfigutarionNodes.Item(0).Attributes["LampCount"].Value);
            Container.Model.LampCount = UnitLampCount;
            for (int i = 0; i < Container.GetLampsModels().Count; i++)
            {
                Container.GetLampsModels()[i].isConfig = i < UnitLampCount;
            }

            XmlNodeList LampNodes = doc.DocumentElement.SelectNodes("/Dimmer/Lamp");
            foreach (XmlNode lampitem in LampNodes)
            {
                int z = int.Parse(lampitem.Attributes["LampIndex"].Value);
                LampView templamp = Container.FindLamp(z);
                templamp.Model.LampPower = int.Parse(lampitem.Attributes["LampPower"].Value);
                foreach (XmlNode monthitem in lampitem.ChildNodes)
                {
                    Month month = (Month)Enum.Parse(typeof(Month), monthitem.Attributes["Month"].Value);
                    MonthView tempmonth = templamp.FindMonth(month);
                    tempmonth.Model.SetTimes(monthitem.ChildNodes);
                    tempmonth.Model.itemchanged += MonthItem_Changd;
                }
            }
            Container.Model.Lamp1Power = Container.FindLamp(0).Model.isConfig ? Container.FindLamp(0).Model.LampPower : 0;
            Container.Model.Lamp2Power = Container.FindLamp(1).Model.isConfig ? Container.FindLamp(1).Model.LampPower : 0;
        }

        private void MonthItem_Changd(object sender, EventArgs e)
        {
            MadeChanges = true;
        }

        int UnitLampCount;

        public void SaveData(XmlWriter writer, object extra)
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
                item.ItemUpdated = false;
            }
            writer.WriteEndElement();
        }


    }
}

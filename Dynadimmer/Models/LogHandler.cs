using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynadimmer.Models.Messages;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Dynadimmer.Models
{
    public class LogHandler:MyUIHandler
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public event EventHandler DoneSaving;

        List<UnitMessage> UnitMessages = new List<UnitMessage>();
        List<NotificationMessage> Notifications = new List<NotificationMessage>();
        List<ConnectionMessage> ConnectionMessages = new List<ConnectionMessage>();

        #region Commands
        #region Clear Log
        public MyCommand ClearLog { get; set; }
        
        private void ClearLog_CommandSent(object sender, EventArgs e)
        {
            //Messages.Clear();
            Save();
        }
        #endregion
        #endregion
        #region Ui Elements

        private ObservableCollection<UnitMessage> messages;
        public ObservableCollection<UnitMessage> Messages
        {
            get { return messages; }
            set
            {
                messages = value;
                NotifyPropertyChanged("Messages");
            }
        }

        private string connectioninfo;
        public string ConnectionInfo
        {
            get { return connectioninfo; }
            set
            {
                connectioninfo = value;
                NotifyPropertyChanged("ConnectionInfo");
            }
        }

        private Brush connectioninfocolor;
        public Brush ConnectionInfoColor
        {
            get { return connectioninfocolor; }
            set
            {
                connectioninfocolor = value;
                NotifyPropertyChanged("ConnectionInfoColor");
            }
        }

        private string messageInfo;
        public string MessageInfo
        {
            get { return messageInfo; }
            set
            {
                messageInfo = value;
                NotifyPropertyChanged("MessageInfo");
            }
        }

        private Brush messageinfocolor;
        public Brush MessageInfoColor
        {
            get { return messageinfocolor; }
            set
            {
                messageinfocolor = value;
                NotifyPropertyChanged("MessageInfoColor");
            }
        }


        private string _savingproccess;
        public string SavingProccess
        {
            get { return _savingproccess; }
            set
            {
                _savingproccess = value;
                NotifyPropertyChanged("SavingProccess");
            }
        }

        private HorizontalAlignment _SavingProccessTextPosition;
        public HorizontalAlignment SavingProccessTextPosition
        {
            get { return _SavingProccessTextPosition; }
            set
            {
                _SavingProccessTextPosition = value;
                NotifyPropertyChanged("SavingProccessTextPosition");
            }
        }

        private Visibility _LogVisibility;
        public Visibility LogVisibility
        {
            get { return _LogVisibility; }
            set
            {
                _LogVisibility = value;
                NotifyPropertyChanged("LogVisibility");
            }
        }

        private bool _LogChecked;
        public bool LogChecked
        {
            get { return _LogChecked; }
            set
            {
                _LogChecked = value;
                Properties.Settings.Default.ViewLog = value;
                Properties.Settings.Default.Save();
                LogVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("LogChecked");
            }
        }

        private bool _reset;
        public bool Reset
        {
            get { return _reset; }
            set
            {
                _reset = value;
                NotifyPropertyChanged("Reset");
            }
        }

        private Visibility _ProgressBarVisibility;
        public Visibility ProgressBarVisibility
        {
            get { return _ProgressBarVisibility; }
            set
            {
                _ProgressBarVisibility = value;
                NotifyPropertyChanged("ProgressBarVisibility");
            }
        }

        private int _TotalMessages;
        public int TotalMessages
        {
            get { return _TotalMessages; }
            set
            {
                _TotalMessages = value;
                ProgressBarVisibility = value == 0 ? Visibility.Collapsed : Visibility.Visible;
                SavingProccessTextPosition = value == 0 ? HorizontalAlignment.Left : HorizontalAlignment.Center;
                NotifyPropertyChanged("TotalMessages");
            }
        }

        private int _CurrentIndex;
        public int CurrentIndex
        {
            get { return _CurrentIndex; }
            set
            {
                _CurrentIndex = value;
                NotifyPropertyChanged("CurrentIndex");
            }
        }
        #endregion

        public LogHandler()
        {
            SavingProccess = "";
            TotalMessages = 0;
            LogChecked = Properties.Settings.Default.ViewLog;
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            Messages = new ObservableCollection<UnitMessage>();
            Reset = true;
            ClearLog = new MyCommand();
            ClearLog.CommandSent += ClearLog_CommandSent;
        }

        #region Save in Background
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DoneSaving(null, null);
            TotalMessages = 0;
            SavingProccess = "Log Last Save in " + DateTime.Now.ToShortTimeString();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentIndex = e.ProgressPercentage;
            SavingProccess = string.Format("Saving log, {1} of {0}.", TotalMessages, e.ProgressPercentage);
        }
        
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<UnitMessage> CopyUnitMessages = new List<UnitMessage>();
            CopyUnitMessages.AddRange(UnitMessages);
            List<NotificationMessage> CopyNotifications = new List<NotificationMessage>();
            CopyNotifications.AddRange(Notifications);
            List<ConnectionMessage> CopyConnectionMessages = new List<ConnectionMessage>();
            CopyConnectionMessages.AddRange(ConnectionMessages);
            TotalMessages = CopyUnitMessages.Count + CopyNotifications.Count + CopyConnectionMessages.Count;
            int index = 0;
            foreach (var item in CopyUnitMessages)
            {
                worker.ReportProgress(++index);
                //Thread.Sleep(200);
            }
            foreach (var item in CopyNotifications)
            {
                worker.ReportProgress(++index);
                //Thread.Sleep(200);
            }
            foreach (var item in CopyConnectionMessages)
            {
                worker.ReportProgress(++index);
                //Thread.Sleep(200);
            }
        }
              
        public void Save()
        {
            if(!worker.IsBusy)
            worker.RunWorkerAsync();
        }
        #endregion

        public void AddMessage(GaneralMessage message)
        {
            if(message is UnitMessage)
            {
                UnitMessages.Add((UnitMessage)message);
                Messages.Insert(0, (UnitMessage)message);
            }
            else if(message is NotificationMessage)
            {
                Notifications.Add((NotificationMessage)message);
                MessageInfo = message.Info;
                MessageInfoColor = message.MessageColor;
            }
            else if (message is ConnectionMessage)
            {
                ConnectionMessages.Add((ConnectionMessage)message);
                ConnectionInfo = message.Info;
                ConnectionInfoColor = message.MessageColor;
            }

        }
    }
    
}

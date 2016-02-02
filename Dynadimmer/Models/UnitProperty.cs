using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Dynadimmer.Models.Messages;
using Dynadimmer.Views.Schedulers.Inner;
using Dynadimmer.Views.Information;

namespace Dynadimmer.Models
{
    public abstract class UnitProperty : INotifyPropertyChanged
    {
        private static IRDAHandler connection;
        private static WindowHandler viewer;

        public static void SetConnection(IRDAHandler con, WindowHandler vi)
        {
            connection = con;
            viewer = vi;
        }

        public MyCommand Download { get; set; }
        public MyCommand Upload { get; set; }

        public event EventHandler<UnitInfo> GotData;

        protected virtual void OnGotData(UnitInfo info)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            if (GotData != null && info != null)
            {
                GotData(this, info);
            }
        }

        public string Title { get; set; }

        private bool isloaded;
        public bool IsLoaded
        {
            get { return isloaded; }
            set
            {
                isloaded = value;
                NotifyPropertyChanged("IsLoaded");
            }
        }

        public UnitProperty()
        {
            IsLoaded = false;

            Download = new MyCommand();
            Download.CommandSent += Download_CommandSent;

            Upload = new MyCommand();
            Upload.CommandSent += Upload_CommandSent;
        }

        public virtual void Download_CommandSent(object sender, EventArgs e)
        {
            SendDownLoad(sender);
        }

        public virtual void Upload_CommandSent(object sender, EventArgs e)
        {
            SendUpload(sender);
        }

        public abstract void SendDownLoad(object sender);
        public abstract void SendUpload(object sender);
        public abstract void UpdateData(UnitInfo info);

        public abstract string GotAnswer(IncomeMessage messase);

        public void SetView()
        {
            IsLoaded = true;
        }

        public abstract void SaveData(System.Xml.XmlWriter writer, object extra);

        public abstract void DidntGotAnswer();

        #region Event Handler
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public void CreateAndSendMessage(SendMessageType type, byte header, params byte[] data)
        {
            connection.Write(new OutMessage(string.Format("Sent {0} {1}.", type.ToString(), Title), viewer.GetUnitID(), header, data));
        }
    }

    public enum SendMessageType
    {
        Download, Upload
    }


}

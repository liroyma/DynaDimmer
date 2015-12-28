using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Dynadimmer.Models.Messages;

namespace Dynadimmer.Models
{
    public abstract class UnitProperty : INotifyPropertyChanged
    {
        private static IRDACummunication connection;

        public static void SetConnection(IRDACummunication con)
        {
            connection = con;
        }

        public event EventHandler AllDone;

        public MyCommand Download { get; set; }
        public MyCommand Upload { get; set; }

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

        private void Download_CommandSent(object sender, EventArgs e)
        {
            SendDownLoad(sender);
        }
        
        private void Upload_CommandSent(object sender, EventArgs e)
        {
            SendUpload(sender);
        }

        public abstract void SendDownLoad(object sender);
        public abstract void SendUpload(object sender);

        public abstract void GotAnswer(IncomeMessage messase);

        public void SetView()
        {
            IsLoaded = true;
            if(AllDone!=null)
                AllDone(null,null);
        }
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

        public void CreateAndSendMessage(UnitProperty prop,byte header, params byte[] data)
        {
            connection.Write(prop, new OutMessage(string.Format("Sent {0}",prop.Title), header, data));
        }
    }
    

}

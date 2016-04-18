using Dynadimmer.Models.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynadimmer.Models
{
    public  class ConnectionHand: INotifyPropertyChanged
    {


         public string connectionbuttontext { get; set; }    
         public Color connectionbuttoncolor { get; set; }

        public MyCommand Connect { get; set; }

        public bool isConnected { get; set; }

        public LogHandler Log;
        public WindowHandler Viewer;


        #region Event Handler
        public  event EventHandler<bool> Connected;
        public  event EventHandler<List<GaneralMessage>> Answered;
        #endregion
        public  bool IsInit { get;  set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Write(OutMessage outMessage)
        {

        }
        public void Init()
        {

        }
        public void SetHandlers(LogHandler log, WindowHandler win)
        {

        }
        public void CheckStatus()
        {

        }
        public void Dispose()
        {

        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
    }
}

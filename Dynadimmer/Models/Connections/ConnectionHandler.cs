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
    public abstract class ConnectionHandler: INotifyPropertyChanged
    {


         public string connectionbuttontext { get; set; }    
         public Color connectionbuttoncolor { get; set; }

        public MyCommand Connect { get; set; }

        public bool isConnected { get; set; }

        public LogHandler Log;
        public WindowHandler Viewer;


        #region Event Handler
        public abstract event EventHandler<bool> Connected;
        public abstract event EventHandler<List<GaneralMessage>> Answered;
        #endregion
        public abstract bool IsInit { get;  set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public abstract void Write(OutMessage outMessage);
        public abstract void Init();
        public abstract void SetHandlers(LogHandler log, WindowHandler win);
        public abstract void CheckStatus();
        public abstract void Dispose();

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
    }
}

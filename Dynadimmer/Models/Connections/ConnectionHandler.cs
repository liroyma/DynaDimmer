using Dynadimmer.Models.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Dynadimmer.Models
{
    public abstract class ConnectionHandler : MyUIHandler
    {

        private string connectionbuttontext;
        public string ConnectionButtonText
        {
            get { return connectionbuttontext; }
            set
            {
                connectionbuttontext = value;
                NotifyPropertyChanged("ConnectionButtonText");
            }
        }

        private Color connectionbuttoncolor;
        public Color ConnectionButtonColor
        {
            get { return connectionbuttoncolor; }
            set
            {
                connectionbuttoncolor = value;
                NotifyPropertyChanged("ConnectionButtonColor");
            }
        }

        private Visibility connectionbuttonvisibility;
        public Visibility ConnectionButtonVisibility
        {
            get { return connectionbuttonvisibility; }
            set
            {
                connectionbuttonvisibility = value;
                NotifyPropertyChanged("ConnectionButtonVisibility");
            }
        }

        public MyCommand Connect { get; set; }
        public bool IsInit { get; set; }

        public bool isConnected { get; set; }
        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                ConnectionButtonColor = IsConnected ? Colors.LightCoral : (this is IRDAHandler? Colors.LightGreen : Colors.LightBlue);
                ConnectionButtonText = IsConnected ? "Disconnect" : "Connect";
                OnConnected(value);
                NotifyPropertyChanged("IsConnected");
            }
        }

        public static LogHandler Log;
        public static WindowHandler Viewer;
        public static void SetHandlers(LogHandler log, WindowHandler win)
        {
            Log = log;
            Viewer = win;
        }


        #region Event Handler
        public event EventHandler<bool> Connected;
        public event EventHandler<List<GaneralMessage>> Answered;

        protected virtual void OnConnected(bool e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<bool> handler = Connected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnAnswered(List<GaneralMessage> e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<List<GaneralMessage>> handler = Answered;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        public abstract void Write(OutMessage outMessage);
        public abstract void Init();
        public abstract void CheckStatus(bool todisconnect);
        public abstract void Dispose();

    }
    public enum CinnectionType
    {
        IRDA,
        TCP
    }
}

using Dynadimmer.Models.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace Dynadimmer.Models
{
    public  class ConnectionHand: INotifyPropertyChanged
    {


        protected string connectionbuttontext { get; set; }
        protected Color connectionbuttoncolor { get; set; }

        protected MyCommand Connect { get; set; }

        public bool isConnected { get; set; }
        public bool isInit { get; set; }

        protected LogHandler Log;
        protected WindowHandler Viewer;

        protected List<byte> answer = new List<byte>();
        protected DispatcherTimer FillAnswerTimer = new DispatcherTimer();

        #region Event Handler
        public  event EventHandler<bool> Connected;
        public  event EventHandler<List<GaneralMessage>> Answered;
        #endregion
        

        public string ConnectionButtonText
        {
            get { return connectionbuttontext; }
            set
            {
                connectionbuttontext = value;
                NotifyPropertyChanged("ConnectionButtonText");
            }
        }

        public Color ConnectionButtonColor
        {
            get { return connectionbuttoncolor; }
            set
            {
                connectionbuttoncolor = value;
                NotifyPropertyChanged("ConnectionButtonColor");
            }
        }

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                ConnectionButtonColor = IsConnected ? Colors.LightCoral : Colors.LightGreen;
                ConnectionButtonText = IsConnected ? "Disconnect" : "Connect";
                if (Connected != null)
                    Connected(null, value);
                NotifyPropertyChanged("IsConnected");
            }
        }

        public bool IsInit
        {
            get { return isInit; }
            set
            {
                isInit = value;
                NotifyPropertyChanged("IsInit");
            }
        }

        protected void FillAnswerTimer_Elapsed(object sender, EventArgs e)
        {
            FillAnswerTimer.Stop();
            List<GaneralMessage> mm = new List<GaneralMessage>();

            while (answer.Contains(1))
            {
                int startindex = answer.FindIndex(x => x == 1);
                if (startindex != 0)
                {
                    mm.Add(new JunkMessage(answer.GetRange(0, startindex)));
                    answer.RemoveRange(0, startindex);
                }
                if (answer.Contains(3))
                {
                    int endindex = answer.FindIndex(x => x == 3);
                    byte[] answer1 = answer.GetRange(0, endindex + 1).ToArray();
                    try
                    {
                        IncomeMessage mess = new IncomeMessage(string.Format("Recived {0}", ""), answer1.ToList());
                        mm.Add(mess);
                    }
                    catch
                    {
                        mm.Add(new JunkMessage(answer1.ToList()));
                        mm.Add(new NotificationMessage("Error getting message.", Brushes.Red));
                    }
                    answer.RemoveRange(0, endindex + 1);
                }
                else
                    break;
            }
            if (answer.Count > 0)
            {
                mm.Add(new JunkMessage(answer));
                answer.Clear();
            }
            Answered(null, mm);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void Write(OutMessage outMessage)
        {
            if (outMessage.Header != Views.DateTime.UnitDateTimeModel.Header && outMessage.Header != Views.OnlineSaving.OnlineSavingModel.Header && outMessage.Header != Views.OnlineSaving.OnlineSavingModel.DaliHeader && outMessage.Header != Views.OnlineSaving.OnlineSavingModel.V1_10_Header)
                Viewer.WindowEnable = false;
            FillAnswerTimer.Start();
            Log.AddMessage(outMessage);
            Log.AddMessage(new NotificationMessage(outMessage.Info, Brushes.Blue));
            //stream.Write(outMessage.DataAscii, 0, outMessage.DataAscii.Length);
        }

        public virtual void Init()
        {
            
        }

        public void SetHandlers(LogHandler log, WindowHandler win)
        {
            Log = log;
            Viewer = win;
            isConnected = false;
        }

        public virtual void CheckStatus()
        {

        }

        public virtual void Dispose()
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

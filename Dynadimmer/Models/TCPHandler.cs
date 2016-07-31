using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Dynadimmer.Models.Messages;
using System.Windows.Media;
using System.Windows.Threading;
using wcl;

namespace Dynadimmer.Models
{
    public class TCPHandler : ConnectionHand
    {
        private IPAddress ip;
        private int port;
        private TcpClient tcpclient;
        private NetworkStream stream;
       // List<byte> answer = new List<byte>();
       // DispatcherTimer FillAnswerTimer = new DispatcherTimer();


        public TCPHandler()
        {
            Connect = new MyCommand();
            Connect.CommandSent += Connect_CommandSent;
            FillAnswerTimer.Tick += FillAnswerTimer_Elapsed;
            FillAnswerTimer.Interval = TimeSpan.FromMilliseconds(1200);
            this.ip = IPAddress.Parse("192.168.4.1");
            this.port = 23;
            tcpclient = new TcpClient();
        }

        #region Commands
       // public MyCommand Connect { get; set; }
        #endregion

        #region Events

        public new event EventHandler<bool> Connected;
        public new event EventHandler<List<GaneralMessage>> Answered;
        #endregion

        #region UI Propeerties
      //  private string connectionbuttontext;
        public new string ConnectionButtonText
        {
            get { return connectionbuttontext; }
            set
            {
                connectionbuttontext = value;
                NotifyPropertyChanged("ConnectionButtonText");
            }
        }

      //  private Color connectionbuttoncolor;
        public new Color ConnectionButtonColor
        {
            get { return connectionbuttoncolor; }
            set
            {
                connectionbuttoncolor = value;
                NotifyPropertyChanged("ConnectionButtonColor");
            }
        }

        //public bool IsConnected
        //{
        //    get { return isConnected; }
        //    set
        //    {
        //        isConnected = value;
        //        ConnectionButtonColor = IsConnected ? Colors.LightCoral : Colors.LightGreen;
        //        ConnectionButtonText = IsConnected ? "Disconnect" : "Connect";
        //        if (Connected != null)
        //            Connected(null, value);
        //        NotifyPropertyChanged("IsConnected");
        //    }
        //}

        //public bool IsInit
        //{
        //    get { return isInit; }
        //    set
        //    {
        //        isInit = value;
        //        NotifyPropertyChanged("IsInit");
        //    }
        //}

        #endregion



        public override void Init()
        {
            try
            {
                tcpclient.Client.Connect(ip, port);
                stream = tcpclient.GetStream(); 
                IsInit = true;   
            }
         
        catch
            {
                IsInit = false;
            }

        }
      
        private void Connect_CommandSent(object sender, EventArgs e)
        {
            if (IsInit)
                CheckStatus();
            else
            {
                if (IsConnected)
                {
                    Dispose();
                    Log.AddMessage(new ConnectionMessage("DisConnected."));
                    IsConnected = false;
                    return;

                }
                else
                {
                    tcpclient = new TcpClient();
                    tcpclient.Client.Connect(ip, port);
                    stream = tcpclient.GetStream();
                    Log.AddMessage(new ConnectionMessage("Connected."));
                    IsConnected = true;
                    IsInit = false;
                    return;
                }
            }
        }

        private new void FillAnswerTimer_Elapsed(object sender, EventArgs e)
        {
            byte[] data = new byte[500];

            base.FillAnswerTimer_Elapsed(sender, e);

            //int answerLength = stream.Read(data, 0, data.Length);
            //for (int i = 0; i < answerLength; i++)
            //{
            //    answer.Add(data[i]);
            //}

            //FillAnswerTimer.Stop();
            //List<GaneralMessage> mm = new List<GaneralMessage>();

            //while (answer.Contains(1))
            //{
            //    int startindex = answer.FindIndex(x => x == 1);
            //    if (startindex != 0)
            //    {
            //        mm.Add(new JunkMessage(answer.GetRange(0, startindex)));
            //        answer.RemoveRange(0, startindex);
            //    }
            //    if (answer.Contains(3))
            //    {
            //        int endindex = answer.FindIndex(x => x == 3);
            //        byte[] answer1 = answer.GetRange(0, endindex + 1).ToArray();
            //        try
            //        {
            //            IncomeMessage mess = new IncomeMessage(string.Format("Recived {0}", ""), answer1.ToList());
            //            mm.Add(mess);
            //        }
            //        catch
            //        {
            //            mm.Add(new JunkMessage(answer1.ToList()));
            //            mm.Add(new NotificationMessage("Error getting message.", Brushes.Red));
            //        }
            //        answer.RemoveRange(0, endindex + 1);
            //    }
            //    else
            //        break;
            //}
            //if (answer.Count > 0)
            //{
            //    mm.Add(new JunkMessage(answer));
            //    answer.Clear();
            //}
            //Answered(null, mm);
        }

        public new void Write(OutMessage outMessage)
        {
            //if (outMessage.Header != Views.DateTime.UnitDateTimeModel.Header && outMessage.Header != Views.OnlineSaving.OnlineSavingModel.Header && outMessage.Header != Views.OnlineSaving.OnlineSavingModel.DaliHeader && outMessage.Header != Views.OnlineSaving.OnlineSavingModel.V1_10_Header)
            //    Viewer.WindowEnable = false;
            //FillAnswerTimer.Start();
            //Log.AddMessage(outMessage);
            //Log.AddMessage(new NotificationMessage(outMessage.Info, Brushes.Blue));
            base.Write(outMessage);
            stream.Write(outMessage.DataAscii, 0, outMessage.DataAscii.Length);
 
        }

        public new void SetHandlers(LogHandler log, WindowHandler win) 
        {
            base.SetHandlers(log, win);
            //Log = log;
            //Viewer = win;
            //IsConnected = false;
        }

        public new void CheckStatus()
        {
            if (tcpclient.Client.Connected)
            {
                Log.AddMessage(new ConnectionMessage("Connected."));
                IsConnected = true;
                IsInit = false;
                return;
            }
        }

        public new void Dispose()
        {
            stream.Close();
            tcpclient.Close();

        }
    }
}

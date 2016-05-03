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
using System.Windows;
using System.Threading;

namespace Dynadimmer.Models
{
    public class TCPHandler : ConnectionHandler
    {
        private IPAddress ip;
        private int port;
        private TcpClient tcpclient;
        private NetworkStream stream;
        List<byte> answer = new List<byte>();
        DispatcherTimer FillAnswerTimer = new DispatcherTimer();


        public bool GetStatus { get; private set; }

        DispatcherTimer ConnectionTimer;
        System.Net.NetworkInformation.Ping ping;
        private bool startping;

        public TCPHandler()
        {
            Connect = new MyCommand();
            Connect.CommandSent += Connect_CommandSent;
            FillAnswerTimer.Tick += FillAnswerTimer_Elapsed;
            FillAnswerTimer.Interval = TimeSpan.FromMilliseconds(1200);
            ConnectionButtonVisibility = Visibility.Collapsed;
        }

        public override void Init()
        {
            this.ip = IPAddress.Parse(Properties.Settings.Default.UnitIPAddress);
            this.port = 23;
            ping = new System.Net.NetworkInformation.Ping();
            ping.PingCompleted += Ping_PingCompleted;
            ping.SendAsync(ip, 800, null);
            ConnectionTimer = new DispatcherTimer();
            ConnectionTimer.Interval = TimeSpan.FromMilliseconds(1000);
            ConnectionTimer.IsEnabled = true;
            ConnectionTimer.Tick += Timer_Tick;
            ConnectionTimer.Start();
            tcpclient = new TcpClient();
            IsInit = true;
            IsConnected = false;
        }

        private void FillAnswerTimer_Elapsed(object sender, EventArgs e)
        {
            byte[] data = new byte[500];
            int answerLength = 0;
            try
            {
                answerLength = stream.Read(data, 0, data.Length);

            }
            catch
            {
                FillAnswerTimer.Stop();
                Viewer.WindowEnable = true;
                return;
            }
            for (int i = 0; i < answerLength; i++)
            {
                answer.Add(data[i]);
            }

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
            OnAnswered(mm);
        }

        public override void Write(OutMessage outMessage)
        {
            if (GetStatus || !startping)
            {
                if (outMessage.Header != Views.DateTime.UnitDateTimeModel.Header && outMessage.Header != Views.OnlineSaving.OnlineSavingModel.Header && outMessage.Header != Views.OnlineSaving.OnlineSavingModel.DaliHeader && outMessage.Header != Views.OnlineSaving.OnlineSavingModel.V1_10_Header)
                    Viewer.WindowEnable = false;

                FillAnswerTimer.Start();
                Log.AddMessage(outMessage);
                Log.AddMessage(new NotificationMessage(outMessage.Info, Brushes.Blue));
                try
                {
                    stream.Write(outMessage.DataAscii, 0, outMessage.DataAscii.Length);
                }
                catch
                {
                    Log.AddMessage(new ConnectionMessage("Unable to connect to " + this.ip + "."));
                    Dispose();
                    IsConnected = false;
                    Viewer.WindowEnable = true;
                }
            }
            else
            {
                Log.AddMessage(new ConnectionMessage("Unable to connect to " + this.ip + "."));
                Dispose();
                IsConnected = false;
                Viewer.WindowEnable = true;
            }

        }

        internal void UpdateIP()
        {
            ip = IPAddress.Parse(Properties.Settings.Default.UnitIPAddress);
        }

        private void Connect_CommandSent(object sender, EventArgs e)
        {
            if (IsInit)
                CheckStatus(false);
        }

        public override void CheckStatus(bool todisconnect)
        {
            if ((IsConnected && tcpclient.Client.Connected) || todisconnect)
            {
                Dispose();
                Log.AddMessage(new ConnectionMessage("DisConnected."));
                IsConnected = false;
                return;

            }

            ConnectionButtonVisibility = Visibility.Visible;
            tcpclient = new TcpClient();
            Log.AddMessage(new ConnectionMessage("Conneting to " + this.ip + "."));
            var result = tcpclient.BeginConnect(ip, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));

            if (!success || !tcpclient.Connected)
            {
                Log.AddMessage(new ConnectionMessage("Unable to connect to " + this.ip + "."));
                IsConnected = false;
            }
            else
            {
                stream = tcpclient.GetStream();
                Log.AddMessage(new ConnectionMessage("Connected TCP."));
                IsConnected = true;
                startping = true;
            }
        }

        public override void Dispose()
        {
            if(stream!=null)
                stream.Close();
            if(tcpclient!=null)
                tcpclient.Close();
            startping = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (startping)
                ping.SendAsync(ip, 800, null);
        }

        private void Ping_PingCompleted(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
        {
            GetStatus = e.Reply.Status == System.Net.NetworkInformation.IPStatus.Success;
        }
    }
}

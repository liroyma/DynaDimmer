using Dynadimmer.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using wcl;

namespace Dynadimmer.Models
{
    public class IRDAHandler : ConnectionHand
    {
        public new bool IsInit { get;  set; }

        public IRDAHandler()
        {
            Connect = new MyCommand();
            Connect.CommandSent += Connect_CommandSent;
            FillAnswerTimer.Tick += FillAnswerTimer_Elapsed;
            FillAnswerTimer.Interval = TimeSpan.FromMilliseconds(1200);

        }

        public new void Init()
        {
            try
            {
                _wclAPI = new wclAPI();
                _wclClient = new wclClient();
                _wclIrDADiscovery = new wclIrDADiscovery();

                /* _wclAPI.AfterLoad += new EventHandler(AfterLoad);
                 _wclAPI.AfterUnload += new EventHandler(AfterUnload);
                 _wclAPI.OnChanged += new EventHandler(OnChanged);*/

                _wclIrDADiscovery.OnComplete += new wcl.wclIrDACompleteEventHandler(OnComplete);
                _wclIrDADiscovery.OnStarted += new System.EventHandler(OnStarted);

                _wclClient.OnDisconnect += new System.EventHandler(OnDisconnect);
                _wclClient.OnData += new wcl.wclDataEventHandler(OnData);
                _wclClient.OnConnect += new wcl.wclConnectEventHandler(OnConnect);
                _wclClient.ConnectTimeout = 60000;

                _wclAPI.Load();
                IsInit = true;
            }
            catch
            {
                IsInit = false;
            }
        }

        #region Handlers
        //LogHandler Log;
        //WindowHandler Viewer;

        public new void SetHandlers(LogHandler log, WindowHandler win)
        {
            //Log = log;
            //Viewer = win;
            //IsConnected = false;
            base.SetHandlers(log, win);
        }

        #endregion

        #region Private Properties
        wclAPI _wclAPI;
        wclClient _wclClient;
        wclIrDADiscovery _wclIrDADiscovery;
        List<wclIrDADevice> Devices;

        //List<byte> answer = new List<byte>();
        //DispatcherTimer FillAnswerTimer = new DispatcherTimer();
        #endregion

        #region Commands
       // public MyCommand Connect { get; set; }
        #endregion

        #region Events
        public new event EventHandler<bool> Connected;
        public new event EventHandler<List<GaneralMessage>> Answered;
        #endregion

        #region UI Propeerties
       // private string connectionbuttontext;
        public  new string ConnectionButtonText
        {
            get { return connectionbuttontext; }
            set
            {
                connectionbuttontext = value;
                NotifyPropertyChanged("ConnectionButtonText");
            }
        }

    //    private Color connectionbuttoncolor;
        public new Color ConnectionButtonColor
        {
            get { return connectionbuttoncolor; }
            set
            {
                connectionbuttoncolor = value;
                NotifyPropertyChanged("ConnectionButtonColor");
            }
        }

        public new  bool IsConnected
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
        #endregion

        #region Send and Recieve

        private new void FillAnswerTimer_Elapsed(object sender, EventArgs e)
        {
            base.FillAnswerTimer_Elapsed(sender, e);
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
            _wclClient.Write(outMessage.DataAscii, (uint)outMessage.DataAscii.Length);
        }

        #endregion

        #region Client
        private void OnConnect(object sender, wclConnectEventArgs e)
        {
            if (e.Error != wcl.wclErrors.WCL_E_SUCCESS)
            {
                IsConnected = false;
                Log.AddMessage(new ConnectionMessage("Unable connect: " + wclErrors.wclGetErrorMessage(e.Error)));
            }
            else
            {
                IsConnected = true;
                Log.AddMessage(new ConnectionMessage("Connected."));
            }
        }

        private void OnData(object sender, wclDataEventArgs e)
        {
            FillAnswerTimer.Stop();
            FillAnswerTimer.Start();
            answer.AddRange(e.Data);
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            IsConnected = false;
            Log.AddMessage(new ConnectionMessage("DisConnected."));
        }
        #endregion

        #region Discovery
        private void OnStarted(object sender, EventArgs e)
        {
            Log.AddMessage(new ConnectionMessage("Looking for Devices..."));
        }

        private void OnComplete(object sender, wclIrDACompleteEventArgs e)
        {
            if (e.Devices == null)
            {
                Log.AddMessage(new ConnectionMessage("Complete with error!", Brushes.Red));
                return;
            }
            if (e.Devices.Count == 0)
            {
                Log.AddMessage(new ConnectionMessage("Nothing found!", Brushes.Red));
                return;
            }
            Devices = new List<wclIrDADevice>();
            for (UInt32 i = 0; i < e.Devices.Count; i++)
            {
                Devices.Add(e.Devices[i]);
            }
            Log.AddMessage(new ConnectionMessage("Connecting..."));
            _wclClient.IrDAParams.Address = Devices[0].Address;
            _wclClient.Transport = wclTransport.trIrDA;
            wclErrors.wclShowError(_wclClient.Connect()).ToString();
        }
        #endregion

        #region API
        private void AfterUnload(object sender, EventArgs e)
        {
        }

        private void AfterLoad(object sender, EventArgs e)
        {
            _wclIrDADiscovery.Discovery();
        }

        private void OnChanged(object sender, EventArgs e)
        {
            if (_wclAPI.Active)
            {
                Log.AddMessage(new ConnectionMessage("APi is Active"));
            }
            else
            {
                Log.AddMessage(new ConnectionMessage("API is not active", Brushes.Red));
            }
        }
        #endregion

        #region Connection

        private void Connect_CommandSent(object sender, EventArgs e)
        {
            if (IsInit)
                CheckStatus();
        }

        public new void CheckStatus()
        {
            if (_wclClient == null)
                return;
            if (_wclClient.State == wclClientState.csConnecting || _wclClient.State == wclClientState.csConnected)
            {
                _wclClient.Disconnect();
            }
            else
            {
                IsConnected = false;
                string x = wclErrors.wclGetErrorMessage(_wclIrDADiscovery.Discovery());
                if (x != String.Empty) Log.AddMessage(new ConnectionMessage("USB is not connected", Brushes.Red));
                // Log.AddMessage(new ConnectionMessage(x,Brushes.Red));
            }
        }

        public new void Dispose()
        {
            if (_wclClient != null)
                _wclClient.Dispose();
            if (_wclIrDADiscovery != null)
                _wclIrDADiscovery.Dispose();
            if (_wclAPI != null)
                _wclAPI.Dispose();
        }

        #endregion

    }
}

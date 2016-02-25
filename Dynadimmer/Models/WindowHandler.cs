using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dynadimmer.Models
{
    public class WindowHandler : MyUIHandler
    {
        private const string _AppName = "Menorah Programmable Dimmer";

        public bool haveinforamtion;
        public bool HaveInformation
        {
            get { return haveinforamtion; }
            set
            {
                haveinforamtion = value;
                NotifyPropertyChanged("HaveInformation");
            }
        }

        private bool isbroadcast;
        public bool IsBroadCast
        {
            get { return isbroadcast; }
            set
            {
                isbroadcast = value;
                RemoteIDString = value ? "Broadcast" : RemoteID.ToString();
                NotifyPropertyChanged("IsBroadCast");
            }
        }
        
        private uint _RemoteID;
        public uint RemoteID
        {
            get { return _RemoteID; }
            set
            {
                _RemoteID = value;
                RemoteIDString = RemoteID.ToString();
                NotifyPropertyChanged("RemoteID");
            }
        }

        private string _RemoteIDstring;
        public string RemoteIDString
        {
            get { return _RemoteIDstring; }
            set
            {
                _RemoteIDstring = value;
                AppTitle = string.Format("{0} {1} - Remote ID: {2}", _AppName, AppVersion, RemoteIDString);
                NotifyPropertyChanged("RemoteID");
            }
        }

        private WindowState _windowState;
        public System.Windows.WindowState WindowState
        {
            get { return _windowState; }
            set
            {
                _windowState = value;
                Properties.Settings.Default.WindowState = value;
                Properties.Settings.Default.Save();
                NotifyPropertyChanged("WindowState");
            }
        }

        private string _AppTitle;
        public string AppTitle
        {
            get { return _AppTitle; }
            set
            {
                _AppTitle = value;
                NotifyPropertyChanged("AppTitle");
            }
        }

        private string _AppVersion;
        public string AppVersion
        {
            get { return _AppVersion; }
            set
            {
                _AppVersion = value;
                AppTitle = string.Format("{0} {1} {2}", _AppName, AppVersion, RemoteIDString);
                NotifyPropertyChanged("AppVersion");
            }
        }

        private bool isconnectedandnotfromfile;
        public bool IsConnectedAndNotFromFile
        {
            get { return isconnectedandnotfromfile; }
            set
            {
                isconnectedandnotfromfile = value;
                NotifyPropertyChanged("IsConnectedAndNotFromFile");
            }
        }

        private bool windowenable;
        public bool WindowEnable
        {
            get { return windowenable; }
            set
            {
                windowenable = value;
                NotifyPropertyChanged("WindowEnable");
            }
        }

        private bool _UnitInfoChecked;
        public bool UnitInfoChecked
        {
            get { return _UnitInfoChecked; }
            set
            {
                _UnitInfoChecked = value;
                UnitInfoVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                if (value)
                {
                    SummerWinterChecked = false;
                    ConfigChecked = false;
                    UnitClockChecked = false;
                    UnitIDChecked = false;
                }
                NotifyPropertyChanged("UnitInfoChecked");
            }
        }

        private bool _UnitIDChecked;
        public bool UnitIDChecked
        {
            get { return _UnitIDChecked; }
            set
            {
                _UnitIDChecked = value;
                UnitIDVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                if (value)
                {
                    SummerWinterChecked = false;
                    ConfigChecked = false;
                    UnitClockChecked = false;
                    UnitInfoChecked = false;
                }
                NotifyPropertyChanged("UnitIDChecked");
            }
        }

        internal byte[] GetUnitID()
        {

            return IsBroadCast ? new byte[] { 255, 255, 255, 255 } : BitConverter.GetBytes(RemoteID).Reverse().ToArray();
        }

        private bool _UnitClockChecked;
        public bool UnitClockChecked
        {
            get { return _UnitClockChecked; }
            set
            {
                _UnitClockChecked = value;
                UnitTimeVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                if (value)
                {
                    SummerWinterChecked = false;
                    ConfigChecked = false;
                    UnitIDChecked = false;
                    UnitInfoChecked = false;
                }
                NotifyPropertyChanged("UnitClockChecked");
            }
        }

        private bool _SummerWinterChecked;
        public bool SummerWinterChecked
        {
            get { return _SummerWinterChecked; }
            set
            {
                _SummerWinterChecked = value;
                SummerWinterVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                if (value)
                {
                    UnitClockChecked = false;
                    ConfigChecked = false;
                    UnitIDChecked = false;
                    UnitInfoChecked = false;
                }
                NotifyPropertyChanged("SummerWinterChecked");
            }
        }

        private bool _ConfigChecked;
        public bool ConfigChecked
        {
            get { return _ConfigChecked; }
            set
            {
                _ConfigChecked = value;
                ConfigVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                if (value)
                {
                    UnitClockChecked = false;
                    SummerWinterChecked = false;
                    UnitIDChecked = false;
                    UnitInfoChecked = false;
                }
                NotifyPropertyChanged("ConfigChecked");
            }
        }

        private Visibility _UnitInfoVisibility;
        public Visibility UnitInfoVisibility
        {
            get { return _UnitInfoVisibility; }
            set
            {
                _UnitInfoVisibility = value;
                NotifyPropertyChanged("UnitInfoVisibility");
            }
        }

        private Visibility _UnitIDVisibility;
        public Visibility UnitIDVisibility
        {
            get { return _UnitIDVisibility; }
            set
            {
                _UnitIDVisibility = value;
                NotifyPropertyChanged("UnitIDVisibility");
            }
        }


        private Visibility _UnitTimeVisibility;
        public Visibility UnitTimeVisibility
        {
            get { return _UnitTimeVisibility; }
            set
            {
                _UnitTimeVisibility = value;
                NotifyPropertyChanged("UnitTimeVisibility");
            }
        }

        private Visibility _SummerWinterVisibility;
        public Visibility SummerWinterVisibility
        {
            get { return _SummerWinterVisibility; }
            set
            {
                _SummerWinterVisibility = value;
                NotifyPropertyChanged("SummerWinterVisibility");
            }
        }

        private Visibility _ConfigVisibility;
        public Visibility ConfigVisibility
        {
            get { return _ConfigVisibility; }
            set
            {
                _ConfigVisibility = value;
                NotifyPropertyChanged("ConfigVisibility");
            }
        }


        public WindowHandler()
        {
            RemoteID = 0;
            UnitInfoChecked = true;
            HaveInformation = false;
            WindowState = Properties.Settings.Default.WindowState;
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                AppVersion = "V" + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
        }
    }
}

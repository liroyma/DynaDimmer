using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dynadimmer.Models
{
    public class WindowHandler: MyUIHandler
    {
        public string AppVersion { get; private set; }
        public string AppTitle { get; private set; }

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
                }
                NotifyPropertyChanged("ConfigChecked");
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
            ConfigChecked = true;
            WindowState = Properties.Settings.Default.WindowState;
            AppTitle = "Menorah Programmable Dimmer";
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                AppTitle += "  - V" + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
        }
    }
}

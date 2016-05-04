using Dynadimmer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Dynadimmer.Models.Messages;
using System.Windows.Data;

namespace Dynadimmer.Views.Setttings
{
    public class AppSettingsModel : MyUIHandler
    {
        #region Price
        private List<double> _pl = new List<double>();

        private ObservableCollection<double> _pricelist = new ObservableCollection<double>();
        public ObservableCollection<double> PriceList
        {
            get { return _pricelist; }
            set
            {
                _pricelist = value;
                NotifyPropertyChanged("PriceList");
            }
        }

        private string _pricetext = "0";
        public string PriceText
        {
            get { return _pricetext; }
            set
            {
                _pricetext = value;
                NotifyPropertyChanged("PriceText");
            }
        }

        private string _selectedprice;
        public string SelectedPrice
        {
            get { return _selectedprice; }
            set
            {
                _selectedprice = value;
                NotifyPropertyChanged("SelectedPrice");
            }
        }

        #endregion

        #region Hours
        private List<int> _hl = new List<int>();

        private ObservableCollection<int> _hourslist = new ObservableCollection<int>();
        public ObservableCollection<int> HoursList
        {
            get { return _hourslist; }
            set
            {
                _hourslist = value;
                NotifyPropertyChanged("HoursList");
            }
        }

        private string _hourtext;
        public string HourText
        {
            get { return _hourtext; }
            set
            {
                _hourtext = value;
                NotifyPropertyChanged("HourText");
            }
        }

        private string _selectedhour;
        public string SelectedHour
        {
            get { return _selectedhour; }
            set
            {
                _selectedhour = value;
                NotifyPropertyChanged("SelectedHour");
            }
        }
        
        #endregion

        #region Commands
        public MyCommand Remove { get; set; }
        public MyCommand Add { get; set; }
        public MyCommand Save { get; set; }
        public MyCommand Close { get; set; }
        public MyCommand Browse { get; set; }
        public MyCommand LogIn { get; set; }

        private void Close_CommandSent(object sender, EventArgs e)
        {
            if (sender is Window)
            {
                if (startcontype != Properties.Settings.Default.ConType && TypeChanged != null)
                    TypeChanged(null, Properties.Settings.Default.ConType);
                if (startipadd != Properties.Settings.Default.UnitIPAddress && UnitIPChanged != null)
                    UnitIPChanged(null, Properties.Settings.Default.UnitIPAddress);
                ((Window)sender).Close();
            }
        }

        private void Save_CommandSent(object sender, EventArgs e)
        {
            Properties.Settings.Default.HoursList = String.Join(";", _hl.ToArray());
            Properties.Settings.Default.PricesList = String.Join(";", _pl.ToArray());
            Properties.Settings.Default.ConType = ConType;
            Properties.Settings.Default.FilesPath = FilesPath;
            if(ConType==ConnectionType.TCP)
                Properties.Settings.Default.UnitIPAddress = string.Format("{0}.{1}.{2}.{3}", ClassA, ClassB, ClassC, ClassD); ;
            Properties.Settings.Default.Save();

        }

        private void UpdateViewLists()
        {
            PriceList.Clear();
            foreach (var item in _pl)
            {
                PriceList.Add(item);
            }
            HoursList.Clear();
            foreach (var item in _hl)
            {
                HoursList.Add(item);
            }
        }

        private void Remove_CommandSent(object sender, EventArgs e)
        {

            switch (sender as string)
            {
                case "Price":
                    if (SelectedPrice != double.Parse("0.0").ToString())
                    {
                        _pl.Remove(double.Parse(SelectedPrice));
                        UpdateViewLists();
                        SelectedPrice = "0.0";
                    }
                    break;
                case "Hour":
                    if (SelectedHour != int.Parse("0").ToString())
                    {
                        _hl.Remove(int.Parse(SelectedHour));
                        SelectedHour = "0";
                    }
                    break;
            }
            UpdateViewLists();
        }

        private void Add_CommandSent(object sender, EventArgs e)
        {
            switch (sender as string)
            {
                case "Price":
                    if (PriceText != double.Parse("0.0").ToString() && !_pl.Contains(double.Parse(PriceText)))
                    {
                        _pl.Add(double.Parse(PriceText));
                        SelectedPrice = "0.0";
                        PriceText = "0.0";
                        _pl.Sort();
                    }
                    break;
                case "Hour":
                    if (HourText != int.Parse("0").ToString() && !_hl.Contains(int.Parse(HourText)))
                    {
                        _hl.Add(int.Parse(HourText));
                        SelectedHour = "0";
                        HourText = "0";
                        _hl.Sort();
                    }
                    break;
            }
            UpdateViewLists();
        }

        private void Browse_CommandSent(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
                FilesPath = folderBrowserDialog1.SelectedPath;
        }

        private void LogIn_CommandSent(object sender, EventArgs e)
        {
            if (LoginWin.Model.IsLogged)
            {
                LoginWin.Model.Logout();
            }
            else
            {
                LoginWin.Model.ErrorVisibility = Visibility.Hidden;
                LoginWin.PassTxt.Clear();
                LoginWin.ShowDialog();
            }
            LoginText = LoginWin.Model.IsLogged ? "Logout" : "Login";
        }
        #endregion

        #region IP Address
        string startipadd;

        public event EventHandler<ConnectionType> TypeChanged;
        public event EventHandler<string> UnitIPChanged;

        private Visibility _ipaddressvisability;
        public Visibility IPAddressVisability
        {
            get { return _ipaddressvisability; }
            set
            {
                _ipaddressvisability = value;
                NotifyPropertyChanged("IPAddressVisability");
            }
        }

        private int _classa;
        public int ClassA
        {
            get { return _classa; }
            set
            {
                _classa = value;
                NotifyPropertyChanged("ClassA");
            }
        }

        private int _classb;
        public int ClassB
        {
            get { return _classb; }
            set
            {
                _classb = value;
                NotifyPropertyChanged("ClassB");
            }
        }

        private int _classc;
        public int ClassC
        {
            get { return _classc; }
            set
            {
                _classc = value;
                NotifyPropertyChanged("ClassC");
            }
        }

        private int _classd;
        public int ClassD
        {
            get { return _classd; }
            set
            {
                _classd = value;
                NotifyPropertyChanged("ClassD");
            }
        }
        #endregion

        #region Login
        public Login.LogInWindow LoginWin { get; set; }

        private string _logintext;
        public string LoginText
        {
            get { return _logintext; }
            set
            {
                _logintext = value;
                NotifyPropertyChanged("LoginText");
            }
        }

        #endregion

        #region Files Path
        private string _filespath;
        public string FilesPath
        {
            get { return _filespath; }
            set
            {
                _filespath = value;
                NotifyPropertyChanged("FilesPath");
            }
        }
        #endregion

        #region Connection Type
        ConnectionType startcontype;

        private ConnectionType _contype;
        public ConnectionType ConType
        {
            get { return _contype; }
            set
            {
                if (onstart)
                {
                    if (once)
                        _contype = value;
                    once = false;
                }
                else
                    _contype = value;
                IPAddressVisability = _contype == ConnectionType.TCP ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("ConType");
            }
        }
        #endregion

        bool onstart = false;
        bool once = false;

        public AppSettingsModel()
        {
            LoginWin = new Login.LogInWindow();
            Save = new MyCommand();
            Save.CommandSent += Save_CommandSent;
            LogIn = new MyCommand();
            LogIn.CommandSent += LogIn_CommandSent;
            Browse = new MyCommand();
            Browse.CommandSent += Browse_CommandSent;
            Close = new MyCommand();
            Close.CommandSent += Close_CommandSent;
            Add = new MyCommand();
            Add.CommandSent += Add_CommandSent;
            Remove = new MyCommand();
            Remove.CommandSent += Remove_CommandSent;
            SelectedPrice = "0.0";
            SelectedHour = "0";
            LoginText = "Login";
            ReadFromSettings();
            Console.WriteLine("SelectedHour: " + SelectedHour);
        }

        public void ReadFromSettings()
        {
            onstart = true;
            once = true;
            ConType = Properties.Settings.Default.ConType;
            startcontype = Properties.Settings.Default.ConType;
            FilesPath = Properties.Settings.Default.FilesPath;
            string IPAddress = Properties.Settings.Default.UnitIPAddress;
            string[] classes = IPAddress.Split('.');
            ClassA = int.Parse(classes[0]);
            ClassB = int.Parse(classes[1]);
            ClassC = int.Parse(classes[2]);
            ClassD = int.Parse(classes[3]);
            startipadd = Properties.Settings.Default.UnitIPAddress;
            if (FilesPath == string.Empty)
                FilesPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string hoursstring = Properties.Settings.Default.HoursList;
            _hl.Clear();
            if (hoursstring != string.Empty)
            {
                foreach (var item in hoursstring.Split(';'))
                {
                    if(!_hl.Contains(int.Parse(item)))
                        _hl.Add(int.Parse(item));
                }
            }
            string pricestring = Properties.Settings.Default.PricesList;
            _pl.Clear();
            if (pricestring != string.Empty)
            {
                foreach (var item in pricestring.Split(';'))
                {
                    if (!_pl.Contains(double.Parse(item)))
                        _pl.Add(double.Parse(item));
                }
            }
            UpdateViewLists();
            onstart = false;
        }
    }

}

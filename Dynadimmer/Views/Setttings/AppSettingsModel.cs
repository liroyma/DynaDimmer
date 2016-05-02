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
        private ObservableCollection<string> _pricelist = new ObservableCollection<string>();
        public ObservableCollection<string> PriceList
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
                PriceAddEnable = (value != "0" && value != "0." && value != "0.0") && !PriceList.Contains(value);
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
                PriceRemoveVisability = value != "0" ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("SelectedPrice");
            }
        }

        private bool _priceaddenable;
        public bool PriceAddEnable
        {
            get { return _priceaddenable; }
            set
            {
                _priceaddenable = value;
                NotifyPropertyChanged("PriceAddEnable");
            }
        }

        private Visibility _priceremovevisability;
        public Visibility PriceRemoveVisability
        {
            get { return _priceremovevisability; }
            set
            {
                _priceremovevisability = value;
                NotifyPropertyChanged("PriceRemoveVisability");
            }
        }

        #endregion

        #region Huers
        private ObservableCollection<string> _hourslist = new ObservableCollection<string>();
        public ObservableCollection<string> HoursList
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
                HourAddEnable = (value != "0" && value != "0." && value != "0.0") && !HoursList.Contains(value);
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

        private bool _houraddenable;
        public bool HourAddEnable
        {
            get { return _houraddenable; }
            set
            {
                _houraddenable = value;
                NotifyPropertyChanged("HourAddEnable");
            }
        }

        private Visibility _hourremovevisability;
        public Visibility HourRemoveVisability
        {
            get { return _hourremovevisability; }
            set
            {
                _hourremovevisability = value;
                NotifyPropertyChanged("HourRemoveVisability");
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
                ((Window)sender).Close();
            }
        }

        private void Save_CommandSent(object sender, EventArgs e)
        {
            Properties.Settings.Default.HoursList = String.Join(";", HoursList.ToArray());
            Properties.Settings.Default.PricesList = String.Join(";", PriceList.ToArray());
            Properties.Settings.Default.ConType = ConType;
            Properties.Settings.Default.FilesPath = FilesPath;
            Properties.Settings.Default.Save();

        }

        private void Remove_CommandSent(object sender, EventArgs e)
        {

            switch (sender as string)
            {
                case "Price":
                    if (SelectedPrice != double.Parse("0.0").ToString())
                    {
                        PriceList.Remove(SelectedPrice);
                        SelectedPrice = "0.0";
                    }
                    break;
                case "Hour":
                    if (SelectedHour != int.Parse("0").ToString())
                    {
                        HoursList.Remove(SelectedHour);
                        SelectedHour = "0";
                    }
                    break;
            }
        }

        private void Add_CommandSent(object sender, EventArgs e)
        {
            switch (sender as string)
            {
                case "Price":
                    if (PriceText != double.Parse("0.0").ToString())
                    {
                        PriceList.Add(PriceText);
                        PriceText = "0.0";
                    }
                    break;
                case "Hour":
                    if (HourText != int.Parse("0").ToString())
                    {
                        HoursList.Add(HourText);
                        HourText = "0";
                    }
                    break;
            }
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

        public Login.LogInWindow LoginWin { get; set; }

        public event EventHandler<CinnectionType> TypeChanged;

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

        CinnectionType startcontype;

        private CinnectionType _contype;
        public CinnectionType ConType
        {
            get { return _contype; }
            set
            {
                if (onstart)
                {
                    if(once)
                        _contype = value;
                    once = false;
                }
                else
                    _contype = value;
                NotifyPropertyChanged("ConType");
            }
        }

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
        }

        public void ReadFromSettings()
        {
            onstart = true;
            once = true;
            ConType = Properties.Settings.Default.ConType;
            startcontype = Properties.Settings.Default.ConType;
            FilesPath = Properties.Settings.Default.FilesPath;
            if (FilesPath == string.Empty)
                FilesPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string hoursstring = Properties.Settings.Default.HoursList;
            if (hoursstring != string.Empty)
            {
                foreach (var item in hoursstring.Split(';'))
                {
                    HoursList.Add(item);
                }
            }
            string pricestring = Properties.Settings.Default.PricesList;
            if (pricestring != string.Empty)
            {
                foreach (var item in pricestring.Split(';'))
                {
                    PriceList.Add(item);
                }
            }
            onstart = false;
        }

    }

    public class NonEmptyStringDoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double x;
            if (value == null || double.TryParse(value.ToString(), out x))
                // return (x >= 0 && x <= 2)
                return new ValidationResult(true, null);
            //: new ValidationResult(false, "Must be number between 0 to 2.");
            return new ValidationResult(false, "Must be a decimal number");
        }

    }

    public class NonEmptyStringIntValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int x;
            if (value == null || int.TryParse(value.ToString(), out x))
                // return (x >= 0 && x <= 2)
                return new ValidationResult(true, null);
            //: new ValidationResult(false, "Must be number between 0 to 2.");
            return new ValidationResult(false, "Must be a integer");
        }

    }

    public class DefaultLowerCaseConverter : System.Windows.Data.IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class EnumBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }
        #endregion
    }
}

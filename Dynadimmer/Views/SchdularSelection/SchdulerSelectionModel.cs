using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Dynadimmer.Models;
using Dynadimmer.Views.Schedulers.Inner;
using System.Collections.ObjectModel;
using System.Windows;

namespace Dynadimmer.Views.SchdularSelection
{
    class SchdulerSelectionModel : INotifyPropertyChanged
    {
        List<MontlySchdulerDetails> Loaded = new List<MontlySchdulerDetails>();
        private StackPanel Container;

        private ObservableCollection<Lamp> _lamps = new ObservableCollection<Lamp>();
        public ObservableCollection<Lamp> LampsList
        {
            get { return _lamps; }
            set
            {
                _lamps = value;
                NotifyPropertyChanged("LampsList");
            }
        }

        public Month[] MonthsList { get; set; }

        public Month SelectedMonth { get; set; }

        private Lamp selectedlamp;
        public Lamp SelectedLamp
        {
            get { return selectedlamp; }
            set
            {
                selectedlamp = value;
                NotifyPropertyChanged("SelectedLamp");
            }
        }

        private bool isenable;
        public bool IsWinEnabled
        {
            get { return isenable; }
            set
            {
                isenable = value;
                WinVisibility = isenable ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("IsWinEnabled");
            }
        }

        private Visibility isvisibility;
        public Visibility WinVisibility
        {
            get { return isvisibility; }
            set
            {
                isvisibility = value;
                NotifyPropertyChanged("WinVisibility");
            }
        }

        public SchdulerSelectionModel()
        {
            MonthsList = (Month[])Enum.GetValues(typeof(Month));
            SelectedMonth = Month.January;
            AddNeW = new MyCommand();
            AddNeW.CommandSent += AddNeW_CommandSent;
            IsWinEnabled = false;
        }

        public void SetContainer(StackPanel container)
        {
            Container = container;
        }

        internal void SetNumberOfLamps(int unitLampCount)
        {
            LampsList.Clear();
            if (unitLampCount == 0)
            {
                IsWinEnabled = false;
                SelectedLamp = Lamp.None;
                return;
            }
            IsWinEnabled = true;
            foreach (var item in ((Lamp[])Enum.GetValues(typeof(Lamp))).ToList().GetRange(0, unitLampCount))
            {
                LampsList.Add(item);
                foreach (Month month in Enum.GetValues(typeof(Month)))
                {
                    MontlySchdulerDetails Details = new MontlySchdulerDetails(item, month);
                    Loaded.Add(Details);
                    Container.Children.Add(Details);
                }
            }
            SelectedLamp = LampsList[0];
        }

        #region Event Handler
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Commands
        public MyCommand AddNeW { get; set; }

        private void AddNeW_CommandSent(object sender, EventArgs e)
        {
            string tempid = MontlySchdulerDetails.CreateID(SelectedLamp, SelectedMonth);
            MontlySchdulerDetails result = Loaded.Find(x => x.Uniqid == tempid);
            //result.ShowItem();
            result.ReSend();
        }
        #endregion
    }

    public enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12,
    }

    public enum Lamp
    {
        Lamp_1 = 0,
        Lamp_2 = 1,
        None
    }
}

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
using Dynadimmer.Views.Schedulers;

namespace Dynadimmer.Views.SchdularSelection
{
    class SchdulerSelectionModel : INotifyPropertyChanged
    {
        List<LampTime> CopiedList;
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

        private bool _HaveCopyItems;
        public bool HaveCopyItems
        {
            get { return _HaveCopyItems; }
            set
            {
                _HaveCopyItems = value;
                foreach (var item in Loaded)
                {
                    item.CanPaste(CopiedList);
                }
                NotifyPropertyChanged("HaveCopyItems");
            }
        }

        public SchdulerSelectionModel()
        {
            MonthsList = (Month[])Enum.GetValues(typeof(Month));
            SelectedMonth = Month.January;
            AddNeW = new MyCommand();
            AddNeW.CommandSent += AddNeW_CommandSent;
            IsWinEnabled = false;
            foreach (Lamp item in Enum.GetValues(typeof(Lamp)))
            {
                foreach (Month month in Enum.GetValues(typeof(Month)))
                {
                    MontlySchdulerDetails Details = new MontlySchdulerDetails(item, month);
                    Details.CopiedItems += Details_CopiedItems;
                    Loaded.Add(Details);
                }
            }
        }

        private void Details_CopiedItems(object sender, EventArgs e)
        {
            MontlySchdulerDetailsModel temp = sender as MontlySchdulerDetailsModel;
            CopiedList = temp.LampTimes.ToList();
            if (CopiedList.Count > 0)
                HaveCopyItems = true;
            temp.CanPaste = false;
        }

        public void SetContainer(StackPanel container)
        {
            Container = container;
            foreach (var item in Loaded)
            {
                Container.Children.Add(item);
            }
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

        public UnitProperty Sent(Lamp lamp, Month month)
        {
            string tempid = MontlySchdulerDetails.CreateID(lamp, month);
            MontlySchdulerDetails result = Loaded.Find(x => x.Uniqid == tempid);
            if (result != null)
                return Loaded.Find(x => x.Uniqid == tempid).Model;
            return null;
            //result.ShowItem();
            //result.ReSend();
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

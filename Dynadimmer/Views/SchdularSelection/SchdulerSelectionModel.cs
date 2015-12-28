using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Dynadimmer.Models;
using Dynadimmer.Views.Schedulers.Inner;

namespace Dynadimmer.Views.SchdularSelection
{
    class SchdulerSelectionModel : INotifyPropertyChanged
    {
        List<MontlySchdulerDetails> Loaded = new List<MontlySchdulerDetails>();

        public Month[] MonthsList { get; set; }
        public Lamp[] LampsList { get; set; }

        public Month SelectedMonth { get; set; }
        public Lamp SelectedLamp { get; set; }


        public SchdulerSelectionModel()
        {
            MonthsList = (Month[])Enum.GetValues(typeof(Month));
            LampsList = (Lamp[])Enum.GetValues(typeof(Lamp)); 
            SelectedMonth = Month.January;
            SelectedLamp = Lamp.Lamp_1;

            AddNeW = new MyCommand();
            AddNeW.CommandSent += AddNeW_CommandSent;
        }

        public void SetContainer(StackPanel container)
        {
            Container = container;
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

        private StackPanel Container;
        public MyCommand AddNeW { get; set; }

        private void AddNeW_CommandSent(object sender, EventArgs e)
        {
            string tempid = MontlySchdulerDetails.CreateID(SelectedLamp, SelectedMonth);
            MontlySchdulerDetails result = Loaded.Find(x => x.Uniqid == tempid);
            //result.AddView += Result_AddView;
            if (result == null)
            {
                MontlySchdulerDetails Details = new MontlySchdulerDetails(SelectedLamp, SelectedMonth);
                Loaded.Add(Details);
            }
            else
            {
                result.ShowItem();
                result.ReSend();
            }

        }

        private void Result_AddView(object sender, EventArgs e)
        {
            SetView();
        }

        private void SetView()
        {
            Container.Children.Clear();
            foreach (var item in Loaded.OrderBy(x => x.Month).OrderBy(x => x.Lamp))
            {
                Container.Children.Add(item);
            }
        }

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
        Lamp_2 = 1
    }
}

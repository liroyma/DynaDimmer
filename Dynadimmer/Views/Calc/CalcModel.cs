using Dynadimmer.Models;
using Dynadimmer.Views.LampItem;
using Dynadimmer.Views.Schedulers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynadimmer.Views.Calc
{
    public class CalcModel : MyUIHandler
    {
        public MyCommand CalcCommand { get; set; }
        private List<LampModel> LampList { get; set; }

        private List<string> hourslist;
        public List<string> HoursList
        {
            get { return hourslist; }
            set
            {
                hourslist = value;
                NotifyPropertyChanged("HoursList");
            }
        }

        private string selectedhour;
        public string SelectedHour
        {
            get { return selectedhour; }
            set
            {
                selectedhour = value;
                Properties.Settings.Default.LastHour = value;
                Properties.Settings.Default.Save();
                NotifyPropertyChanged("SelectedHour");
            }
        }

        private List<string> pricelist;
        public List<string> PriceList
        {
            get { return pricelist; }
            set
            {
                pricelist = value;
                NotifyPropertyChanged("PriceList");
            }
        }

        private string selectedprice;
        public string SelectedPrice
        {
            get { return selectedprice; }
            set
            {
                selectedprice = value;
                Properties.Settings.Default.LastPrice = value;
                Properties.Settings.Default.Save();
                NotifyPropertyChanged("SelectedPrice");
            }
        }


        private string calcedprice;
        public string CalcedPrice
        {
            get { return calcedprice; }
            set
            {
                calcedprice = value;
                NotifyPropertyChanged("CalcedPrice");
            }
        }

        private string calcedhour;
        public string CalcedHour
        {
            get { return calcedhour; }
            set
            {
                calcedhour = value;
                NotifyPropertyChanged("CalcedHour");
            }
        }


        private ObservableCollection<LampClaculation> lampcacllist;
        public ObservableCollection<LampClaculation> LampCaclList
        {
            get { return lampcacllist; }
            set
            {
                lampcacllist = value;
                NotifyPropertyChanged("LampCaclList");
            }
        }
        

        public CalcModel()
        {
            LampCaclList = new ObservableCollection<LampClaculation>();
            SelectedPrice = Properties.Settings.Default.LastPrice;
            SelectedHour = Properties.Settings.Default.LastHour;
            string hoursstring = Properties.Settings.Default.HoursList;
            HoursList = new List<string>();
            if (hoursstring != string.Empty)
            {
                foreach (var item in hoursstring.Split(';'))
                {
                    HoursList.Add(item);
                }
            }
            string pricestring = Properties.Settings.Default.PricesList;
            PriceList = new List<string>();
            if (pricestring != string.Empty)
            {
                foreach (var item in pricestring.Split(';'))
                {
                    PriceList.Add(item);
                }
            }
            CalcCommand = new MyCommand();
            CalcCommand.CommandSent += CalcCommand_CommandSent;
        }

        public void SetLampListAndCalc(List<LampModel> lamplist)
        {
            LampList = lamplist;
            CalcCommand_CommandSent(null, null);
        }

        private void CalcCommand_CommandSent(object sender, EventArgs e)
        {
            LampCaclList.Clear();
            if (SelectedPrice == string.Empty || SelectedHour == string.Empty)
                return;
            CalcedHour = SelectedHour;
            CalcedPrice = SelectedPrice;
            foreach (var lamp in LampList)
            {
                LampCaclList.Add(new LampClaculation(lamp,int.Parse(SelectedHour),double.Parse(SelectedPrice)));
            }           
        }
    }

    public class LampClaculation
    {
        public string LampName { get; set; }
        public int LampPower { get; set; }
        public double YearlyUse { get; set; }
        public double YearlyCost { get; set; }
        public double YearlySavings { get; set; }
        public double YearlyCostSavings { get; set; }
        public double YearlySavingsPreeent { get; set; }

        public LampClaculation(LampModel lamp,int yearly,double cost)
        {
            LampName = lamp.Name;
            LampPower = lamp.LampPower;
            YearlyUse = lamp.LampPower * (double)yearly / 1000.0;
            YearlyCost = YearlyUse * cost;
            YearlySavings = 0;
            foreach (var item in lamp.GetMonths())
            {
                double daycalc = 0;
                for (int i = 0; i < item.LampTimes.Count; i++)
                {
                    double span;
                    if (item.LampTimes.Last() == item.LampTimes[i])
                        span = (lamp.LampPower * CalcTimeSpan(item.LampTimes[i], item.EndTime) / 1000) * ((100.0 - item.LampTimes[i].Precentage) / 100);
                    else
                        span = (lamp.LampPower * CalcTimeSpan(item.LampTimes[i], item.LampTimes[i]) / 1000) * ((100.0 - item.LampTimes[i].Precentage) / 100);
                    daycalc += span;
                }
                YearlySavings += daycalc * item.MonthDays;
            }
            YearlyCostSavings = YearlySavings * cost;
            YearlySavingsPreeent = (YearlyCost==0)?0:(YearlyCostSavings / YearlyCost*100);
        }

        private double CalcTimeSpan(LampTime one, LampTime two)
        {
            return (two.date - one.date).TotalHours;
        }

    }
}

using Dynadimmer.Models;
using Dynadimmer.Views.LampItem;
using Dynadimmer.Views.MonthItem;
using Dynadimmer.Views.Schedulers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using Dynadimmer.Models.Messages;

namespace Dynadimmer.Views.Calc
{
    public class CalcModel : MyUIHandler
    {
        public MyCommand ClearCommand { get; set; }

        private ObservableCollection<string> fileslist;
        public ObservableCollection<string> FilesList
        {
            get { return fileslist; }
            set
            {
                fileslist = value;
                NotifyPropertyChanged("FilesList");
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
            FilesList = new ObservableCollection<string>();
            ClearCommand = new MyCommand();
            ClearCommand.CommandSent += ClearCommand_CommandSent;
        }

        internal void Read(string path)
        {
            if (System.IO.Path.GetExtension(path) != ".dxml" || FilesList.Contains(path))
                return;
            FilesList.Add(path);
            foreach (var lamp in ReadFromFileModel.Read(path))
            {
                LampCaclList.Add(new LampClaculation(lamp, "File", path));
            }
            LampClaculation.colorindex++;
        }

        private void ClearCommand_CommandSent(object sender, EventArgs e)
        {
            LampCaclList.Clear();
            FilesList.Clear();
        }

        internal void SetLampListAndCalc(List<LampModel> lamplist, string unitid)
        {
            foreach (var item in LampCaclList.Where(x => x.Source == "Unit").ToList())
            {
                LampCaclList.Remove(item);
            }     
            foreach (var lamp in lamplist)
            {
                LampCaclList.Insert(0,new LampClaculation(lamp, "Unit", unitid));
            }
            LampClaculation.colorindex++;
        }


    }

    public class LampClaculation : MyUIHandler
    {
        static SolidColorBrush[] colors = new SolidColorBrush[] { Brushes.LightBlue, Brushes.LightCoral };
        public static int colorindex = 0;

        public string LampName { get; set; }
        public int LampPower { get; set; }
        public SolidColorBrush ItemBackground { get; set; }
        public string FileID { get; set; }
        public string FilePath { get; set; }
        public string Source { get; set; }

        public double yearlyuse;
        public double YearlyUse
        {
            get { return yearlyuse; }
            set
            {
                yearlyuse = value;
                NotifyPropertyChanged("YearlyUse");
            }
        }

        public double yearlycost;
        public double YearlyCost
        {
            get { return yearlycost; }
            set
            {
                yearlycost = value;
                NotifyPropertyChanged("YearlyCost");
            }
        }

        public double yearlysavings;
        public double YearlySavings
        {
            get { return yearlysavings; }
            set
            {
                yearlysavings = value;
                NotifyPropertyChanged("YearlySavings");
            }
        }

        public double yearlycostsavings;
        public double YearlyCostSavings
        {
            get { return yearlycostsavings; }
            set
            {
                yearlycostsavings = value;
                NotifyPropertyChanged("YearlyCostSavings");
            }
        }

        public double yearlysavingsprecent;
        public double YearlySavingsPrecent
        {
            get { return yearlysavingsprecent; }
            set
            {
                yearlysavingsprecent = value;
                NotifyPropertyChanged("YearlySavingsPrecent");
            }
        }

        private int numberoflamps;
        public int NumberOfLamps
        {
            get { return numberoflamps; }
            set
            {
                  
                numberoflamps = value;
                NotifyPropertyChanged("NumberOfLamps");
                Calc();
            }
        }

        public List<MonthModel> Months;

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

        private double selectedhour;
        public double SelectedHour
        {
            get { return selectedhour; }
            set
            {
                selectedhour = value;
                Calc();
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

        private double selectedprice;
        public double SelectedPrice
        {
            get { return selectedprice; }
            set
            {
                selectedprice = value;
                Calc();
                Properties.Settings.Default.LastPrice = value;
                Properties.Settings.Default.Save();
                NotifyPropertyChanged("SelectedPrice");
            }
        }

        bool init;
        
        public LampClaculation(LampModel lamp, string source, string info)
        {
            Source = source;
            if (Source == "File")
            {
                FilePath = info;
                string filename = System.IO.Path.GetFileNameWithoutExtension(info);
                string[] infosplit = info.Split('.');
                FileID = infosplit.Length > 1 ? infosplit[1] : "0";
                ItemBackground = colors[colorindex % colors.Length];
            }
            if (Source == "Unit")
            {
                FileID = info;
                ItemBackground = Brushes.Ivory;
            }
            init = true;
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
            Months = lamp.GetMonths();
            LampName = lamp.Name;
            LampPower = lamp.LampPower;
            NumberOfLamps = 1;
            init = false;
            Calc();

        }

        private void Calc()
        {
            if (init)
                return;
            YearlyUse = LampPower * SelectedHour / 1000.0;
            YearlyCost = YearlyUse * SelectedPrice;
            YearlySavings = 0;

            foreach (var item in Months)
            {
                double daycalc = 0;
                for (int i = 0; i < item.LampTimes.Count; i++)
                {
                    double span;
                    if (item.LampTimes.Last() == item.LampTimes[i])
                        span = (LampPower * LampTime.CalcTotalHoursSpan(item.LampTimes[i], item.EndTime) / 1000) * ((100.0 - item.LampTimes[i].Precentage) / 100);
                    else
                        span = (LampPower * LampTime.CalcTotalHoursSpan(item.LampTimes[i], item.LampTimes[i]) / 1000) * ((100.0 - item.LampTimes[i].Precentage) / 100);
                    daycalc += span;
                }
                YearlySavings += daycalc * item.MonthDays;
            }
            YearlyCostSavings = YearlySavings * SelectedPrice; 
            YearlySavingsPrecent = (YearlyCost == 0) ? 0 : (YearlyCostSavings / YearlyCost * 100);

            YearlyUse *= NumberOfLamps;
            YearlyCost *= NumberOfLamps;
            YearlyCostSavings *= NumberOfLamps;
            YearlySavings *= NumberOfLamps;

        }


    }
}

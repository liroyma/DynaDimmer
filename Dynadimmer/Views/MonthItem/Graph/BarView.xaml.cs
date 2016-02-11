using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dynadimmer.Views.Schedulers.Inner
{
    /// <summary>
    /// Interaction logic for BarView.xaml
    /// </summary>
    public partial class BarView : UserControl, INotifyPropertyChanged
    {
        public System.DateTime StartTime { get; private set; }
        public System.DateTime EndTime { get; private set; }
        public TimeSpan TotlaSpan { get; private set; }
        public double ItemActualWidth { get; private set; }
        public double Left { get; private set; }
        public bool isDefultWidth { get; private set; }


        private int _precentage;
        public int Precentage
        {
            get { return _precentage; }
            set
            {
                _precentage = value;
                if (value < 10)
                {
                    PrecentageColor = Brushes.Red;
                    PrecentagePosition = new Thickness(0, -15, 0, 0);
                }
                else
                {
                    PrecentageColor = Brushes.White;
                    PrecentagePosition = new Thickness(0, 5, 0, 0);
                }
                NotifyPropertyChanged("Precentage");
            }
        }
        
        private Thickness _precentageposition;
        public Thickness PrecentagePosition
        {
            get { return _precentageposition; }
            set
            {
                _precentageposition = value;
                NotifyPropertyChanged("PrecentagePosition");
            }
        }

        private Brush _precentagecolor;
        public Brush PrecentageColor
        {
            get { return _precentagecolor; }
            set
            {
                _precentagecolor = value;
                NotifyPropertyChanged("PrecentageColor");
            }
        }

        private double _barheight;
        public double BarHeight
        {
            get { return _barheight; }
            set
            {
                _barheight = value;
                NotifyPropertyChanged("BarHeight");
            }
        }



        public BarView()
        {
            InitializeComponent();
            this.DataContext = this;
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

        private void DockPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BarHeight = this.Height * Precentage / 100;
        }

        public void SetTimes(System.DateTime start, System.DateTime end,int precent )
        {
            StartTime = start;
            EndTime = end;
            TotlaSpan = end - start;
            Precentage = precent;
        }

        public void SetSize(double heigth, double oneminwidth)
        {
            this.Height = heigth;
            isDefultWidth = true;
            ItemActualWidth = oneminwidth * TotlaSpan.TotalHours;
            this.Width = ItemActualWidth;
            if (ItemActualWidth<40)
            {
                this.Width = 40;
                isDefultWidth = false;
            }
        }

    }
}

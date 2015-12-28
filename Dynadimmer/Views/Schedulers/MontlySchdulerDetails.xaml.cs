using System;
using System.Collections.Generic;
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
using Dynadimmer.Models;
using Dynadimmer.Views.SchdularSelection;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Dynadimmer.Views.Schedulers.Inner
{
    /// <summary>
    /// Interaction logic for MontlySchdulerDetails.xaml
    /// </summary>
    public partial class MontlySchdulerDetails : UserControl
    {
        public UnitProperty Model { get; private set; }
        

        public string Uniqid { get; set; }
        public int Lamp { get; set; }
        public int Month { get; set; }

        public MontlySchdulerDetails(Lamp selectedLamp, Month selectedMonth)
        {
            InitializeComponent();
            Lamp = (int)selectedLamp;
            Month = (int)selectedMonth;
            Model = new MontlySchdulerDetailsModel(selectedLamp, selectedMonth);
            ((MontlySchdulerDetailsModel)Model).UpdateView += MontlySchdulerDetails_UpdateView;
            Uniqid = CreateID(selectedLamp, selectedMonth);
            this.DataContext = Model;
        }

        private void MontlySchdulerDetails_UpdateView(object sender, EventArgs e)
        {
            UpdateView();
        }

        public static string CreateID(Lamp lamp, Month month)
        {
            return string.Format("L{0}M{1}", (int)lamp, (int)month);
        }

        public void ReSend()
        {
            Model.SendUpload(null);
        }

        internal void ShowItem()
        {
            if (((MontlySchdulerDetailsModel)Model).ItemVisablility == Visibility.Collapsed)
                ((MontlySchdulerDetailsModel)Model).ItemVisablility = Visibility.Visible;
        }


        private void container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateView();
        }

        internal void UpdateView()
        {
            List<LampTime> lampTimes = ((MontlySchdulerDetailsModel)Model).LampTimes.ToList();
            container.Children.Clear();
            if (lampTimes.Count == 0)
                return;

            double width = container.ActualWidth;
            double height = container.ActualHeight;

            double currentLeft = 20;

            double step = (width - 40) / (lampTimes.Count + 1);


            TextBlock starttextBlock = new TextBlock();
            starttextBlock.Text = "On";
            Canvas.SetBottom(starttextBlock, 0);
            Canvas.SetLeft(starttextBlock, currentLeft - 10);

            TextBlock endtextBlock = new TextBlock();
            endtextBlock.Text = "Off";
            Canvas.SetBottom(endtextBlock, 0);
            Canvas.SetLeft(endtextBlock, width - 30);

            BarView bar = new BarView();
            bar.Height = height - 40;
            bar.Width = step;
            bar.Precentage = 100;
            Canvas.SetBottom(bar, 20);
            Canvas.SetLeft(bar, currentLeft);


            container.Children.Add(bar);
            container.Children.Add(starttextBlock);
            container.Children.Add(endtextBlock);

            currentLeft += bar.Width;
            foreach (var item in lampTimes)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = item.TimeString;
                Canvas.SetLeft(textBlock, currentLeft - 10);
                Canvas.SetBottom(textBlock, 0);

                BarView bar1 = new BarView();
                bar1.Height = height - 40;
                bar1.Width = step;
                bar1.Precentage = item.Precentage;
                Canvas.SetBottom(bar1, 20);
                Canvas.SetLeft(bar1, currentLeft);

                container.Children.Add(bar1);
                container.Children.Add(textBlock);
                currentLeft += bar1.Width;
            }
        }
    }

    public class DoubleRangeRule : ValidationRule
    {
        public double Min { get; set; }

        public double Max { get; set; }

        public override ValidationResult Validate(object value,
                                                   CultureInfo cultureInfo)
        {
            double parameter = 0;

            try
            {
                if (((string)value).Length > 0)
                {
                    parameter = Double.Parse((String)value);
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Illegal characters or "
                                             + e.Message);
            }

            if ((parameter < this.Min) || (parameter > this.Max))
            {
                return new ValidationResult(false,
                    "Please enter value in the range: "
                    + this.Min + " - " + this.Max + ".");
            }
            return new ValidationResult(true, null);
        }
    }
}

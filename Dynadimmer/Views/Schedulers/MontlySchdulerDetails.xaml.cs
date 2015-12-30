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
        public event EventHandler CopiedItems;

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
            ((MontlySchdulerDetailsModel)Model).ItemsCopied += MontlySchdulerDetails_ItemsCopied;
            Uniqid = CreateID(selectedLamp, selectedMonth);
            this.DataContext = Model;
        }

        private void MontlySchdulerDetails_ItemsCopied(object sender, EventArgs e)
        {
            CopiedItems(sender, e);
        }

        private void MontlySchdulerDetails_UpdateView(object sender, EventArgs e)
        {
            UpdateView();
            UpdateView1();
        }

        public static string CreateID(Lamp lamp, Month month)
        {
            return string.Format("L{0}M{1}", (int)lamp, (int)month);
        }

        public void ReSend()
        {
            Model.SendUpload(null);
        }

        internal void CanPaste(List<LampTime> value)
        {
            ((MontlySchdulerDetailsModel)Model).PastebleLampTimes = value;
        }

        private void container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateView();
            UpdateView1();
        }

        internal void UpdateView1()
        {
            List<LampTime> lampTimes = ((MontlySchdulerDetailsModel)Model).LampTimes.ToList();
            container.Children.Clear();

            double width = container.ActualWidth;
            double height = container.ActualHeight;
            if (width <= 0 || height <= 0)
                return;

            double graphwidth = width - 40;

            double currentLeft = 20;

            double firstandlaststep = graphwidth / (lampTimes.Count + 1);
            double oneminutewidth = 0;


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
            bar.Width = firstandlaststep;
            bar.Precentage = 100;
            Canvas.SetBottom(bar, 20);
            Canvas.SetLeft(bar, currentLeft);


            container.Children.Add(bar);
            container.Children.Add(starttextBlock);
            container.Children.Add(endtextBlock);

            if (lampTimes.Count >= 1)
                oneminutewidth = (graphwidth - (firstandlaststep * 2)) / CalcTimeSpan(lampTimes.First(), lampTimes.Last());
            else
                return;

            currentLeft += bar.Width;
            for (int i = 0; i < lampTimes.Count - 1; i++)
            {
                double step = CalcTimeSpan(lampTimes[i], lampTimes[i + 1]) * oneminutewidth;
                if (step < 40)
                {
                    firstandlaststep -= (40 - step);
                    step = 40;
                }
                TextBlock textBlock = new TextBlock();
                textBlock.Text = lampTimes[i].TimeString;
                Canvas.SetLeft(textBlock, currentLeft - 10);
                Canvas.SetBottom(textBlock, 0);

                BarView bar1 = new BarView();
                bar1.Height = height - 40;
                bar1.Width = step;
                bar1.Precentage = lampTimes[i].Precentage;
                Canvas.SetBottom(bar1, 20);
                Canvas.SetLeft(bar1, currentLeft);

                container.Children.Add(bar1);
                container.Children.Add(textBlock);
                currentLeft += bar1.Width;
            }

            TextBlock lasttextBlock = new TextBlock();
            lasttextBlock.Text = lampTimes.Last().TimeString;
            Canvas.SetLeft(lasttextBlock, currentLeft - 10);
            Canvas.SetBottom(lasttextBlock, 0);

            BarView endbar = new BarView();
            endbar.Height = height - 40;
            endbar.Width = firstandlaststep;
            endbar.Precentage = lampTimes.Last().Precentage;
            Canvas.SetBottom(endbar, 20);
            Canvas.SetLeft(endbar, currentLeft);

            container.Children.Add(endbar);
            container.Children.Add(lasttextBlock);
        }


        private double CalcTimeSpan(LampTime one, LampTime two)
        {
            if (two.Hour < one.Hour)
                return (((24 + two.Hour) - one.Hour) * 60) + (two.Minute - one.Minute);
            return ((two.Hour - one.Hour) * 60) + (two.Minute - one.Minute);
        }

        internal void UpdateView()
        {
            List<LampTime> lampTimes = ((MontlySchdulerDetailsModel)Model).LampTimes.ToList();
            container.Children.Clear();

            double width = container.ActualWidth;
            double height = container.ActualHeight;
            if (width <= 0 || height <= 0)
                return;

            double graphwidth = width - 40;

            double currentLeft = 20;

            double step = graphwidth / (lampTimes.Count + 1);




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
            for (int i = 0; i < lampTimes.Count; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = lampTimes[i].TimeString;
                Canvas.SetLeft(textBlock, currentLeft - 10);
                Canvas.SetBottom(textBlock, 0);

                BarView bar1 = new BarView();
                bar1.Height = height - 40;
                step = graphwidth / (lampTimes.Count + 1);
                bar1.Width = step;
                bar1.Precentage = lampTimes[i].Precentage;
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

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
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
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }

            if ((parameter < this.Min) || (parameter > this.Max))
            {
                return new ValidationResult(false, "Please enter value in the range: " + this.Min + " - " + this.Max + ".");
            }
            return new ValidationResult(true, null);
        }
    }
}

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
using System.Globalization;
using System.Collections.ObjectModel;
using Dynadimmer.Views.NewSchdularSelection;

namespace Dynadimmer.Views.Schedulers.Inner
{
    /// <summary>
    /// Interaction logic for MontlySchdulerDetails.xaml
    /// </summary>
    public partial class MontlySchdulerDetails : UserControl
    {
        public MontlySchdulerDetailsModel Model { get; private set; }

        public MontlySchdulerDetails(NewSchedularSelectionModel perent, Lamp selectedLamp, Month selectedMonth)
        {
            InitializeComponent();
            Model = new MontlySchdulerDetailsModel(perent, container, selectedLamp, selectedMonth);
            this.DataContext = Model;
        }

        private void container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Model.UpdateView();
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

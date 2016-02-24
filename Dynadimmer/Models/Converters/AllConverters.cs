using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Dynadimmer.Models.Converters
{
    public class DoubleRangeRule : ValidationRule
    {
        public double Min { get; set; }

        public double Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double parameter = 0;
            this.Max = this.Max < this.Min ? Double.MaxValue : this.Max;
            //this.Min = this.Max < this.Min ? Double.MinValue : this.Min;

            if (!double.TryParse((string)value, out parameter))
            {
                return new ValidationResult(false, "Illegal characters.");
            }
            
            if (parameter > Int32.MaxValue || parameter < Int32.MinValue)
            {
                return new ValidationResult(false, "Please enter value in the range: " + this.Min + " - " + this.Max + ".");
            }

            if ((parameter < this.Min) || (parameter > this.Max))
            {
                return new ValidationResult(false, "Please enter value in the range: " + this.Min + " - " + this.Max + ".");
            }
            return new ValidationResult(true, null);
        }

    }
}

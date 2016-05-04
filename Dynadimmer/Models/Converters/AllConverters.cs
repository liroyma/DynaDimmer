using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Dynadimmer.Models.ConvertersAndRules
{
    #region Validation Rules
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

    public class IntRangeRule : ValidationRule
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int parameter = 0;
            this.Max = this.Max < this.Min ? int.MaxValue : this.Max;
            //this.Min = this.Max < this.Min ? Double.MinValue : this.Min;

            if (!int.TryParse((string)value, out parameter))
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
    #endregion

    #region Converters

    #region Value Converter
    public class EnumBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }
        #endregion
    }
    #endregion
    
    #region Multi Value Converter
    public class ListContainItemConverter : IMultiValueConverter
    {
        #region IValueConverter Members

        public object Convert(object[] value, Type targetType,
            object parameter, CultureInfo culture)
        {
            switch((string)parameter)
            {
                case "price":
                    if (value[0] is System.Collections.ObjectModel.ObservableCollection<double> && value[1] is string)
                    {
                        return ((System.Collections.ObjectModel.ObservableCollection<double>)value[0]).Contains(double.Parse((string)value[1]));
                    }
                    break;
                case "hour":
                    if (value[0] is System.Collections.ObjectModel.ObservableCollection<int> && value[1] is string)
                    {
                        return ((System.Collections.ObjectModel.ObservableCollection<int>)value[0]).Contains(int.Parse((string)value[1]));
                    }
                    break;
            }
           
            return false;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public class BooleanOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object value in values)
            {
                if ((bool)value == true)
                {
                    return true;
                }
            }
            return false;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    #endregion
    
    #endregion
}

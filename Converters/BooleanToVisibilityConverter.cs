using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SocinatorInstaller.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsInversed { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (bool)value;
            return IsInversed ? data ? Visibility.Collapsed : Visibility.Visible :
                data ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class BooleanToBooleanConverter : IValueConverter
    {
        public bool IsInvert { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (bool)value;
            return IsInvert ? data ? false : true : data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class BooleanToValueConverter : IValueConverter
    {
        public string TrueValue { get; set; }
        public string FalseValue { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (bool)value;
            return data ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QLHS_DR.Converter
{
    [ValueConversion(typeof(double), typeof(double))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(System.Windows.Visibility) && value != null)
            {
                bool isVisible = (bool)value;
                if (isVisible == true) return "Visible"; else return "Hidden";
            }
            else return "Hidden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var back = ((value is System.Windows.Visibility) && (((System.Windows.Visibility)value) == System.Windows.Visibility.Visible));
            if (parameter != null)
            {
                if ((bool)parameter)
                {
                    back = !back;
                }
            }
            return back;
        }
    }
}

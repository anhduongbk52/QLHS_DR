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
    public class GenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string gender)
            {
                return gender == "0" ? "Nam" : "Nữ";
            }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string genderString)
            {
                return genderString == "Nam" ? "0" : "1";
            }
            return Binding.DoNothing;
        }
    }
}

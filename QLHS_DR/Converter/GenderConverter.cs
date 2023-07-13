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
            if (value != null)
            {
                bool gender = (bool)value;
                if (gender == true)
                    return "Nam";
                else if (gender == false)
                    return "Nữ";
                else return null;
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gender = (string)value;
            if (gender == "Nam")
            {
                return true;
            }
            else if (gender == "Nữ") return false;
            else return null;
        }
    }
}

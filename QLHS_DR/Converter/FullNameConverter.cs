using QLHS_DR.ChatAppServiceReference;
using System;
using System.Globalization;
using System.Windows.Data;

namespace QLHS_DR.Converter
{
    public class FullNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Employee employee)
            {
                return $"{employee.FirtName} {employee.LastName}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

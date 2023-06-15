using QLHS_DR.ChatAppServiceReference;
using System;
using System.Globalization;
using System.Windows.Data;

namespace QLHS_DR.Converter
{
    [ValueConversion(typeof(double), typeof(double))]
    public class PermissionTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var permission = (PermissionType)value;
            if (permission == PermissionType.PRINT_DOCUMENT)
            {
                return true;
            }
            else return false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((bool)value == true)
            {
                return PermissionType.PRINT_DOCUMENT;
            }
            else

                return PermissionType.NONE;
        }
    }
}

﻿using QLHS_DR.ServiceReference1;
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

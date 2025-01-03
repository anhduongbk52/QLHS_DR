﻿using System;
using System.Windows.Data;

namespace QLHS_DR.Converter
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
         System.Globalization.CultureInfo culture)
        {
            if (value == null || targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null || targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value; // Trả về giá trị đảo ngược.
        }
    }
}

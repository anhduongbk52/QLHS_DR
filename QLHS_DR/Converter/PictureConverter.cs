using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DevExpress.DevAV;

namespace QLHS_DR.Converter
{
    public class PictureConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] imageBytes)
            {
                if (imageBytes.Length > 0)
                {
                    // Chuyển byte[] thành MemoryStream
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        return bitmap;
                    }
                }
            }
            return null; // Trả về null nếu không có dữ liệu ảnh
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Không cần ConvertBack
        }
    }
}

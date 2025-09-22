using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Axis2.WPF.Converters
{
    public class ImageScaleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3 || !(values[0] is BitmapSource bmp) || !(values[1] is double containerWidth) || !(values[2] is double containerHeight))
            {
                return 1.0;
            }

            if (bmp == null || containerWidth == 0 || containerHeight == 0)
                return 1.0;

            // If image is small (e.g., less than half the container size), zoom x2
            // But make sure the zoomed image is not larger than the container
            if (bmp.PixelWidth < containerWidth / 2 && bmp.PixelHeight < containerHeight / 2)
            {
                double potentialZoom = 2.0;
                if (bmp.PixelWidth * potentialZoom < containerWidth && bmp.PixelHeight * potentialZoom < containerHeight)
                {
                    return potentialZoom;
                }
            }

            // If image is larger than container, scale it down to fit
            double scaleX = containerWidth / bmp.PixelWidth;
            double scaleY = containerHeight / bmp.PixelHeight;
            double scale = Math.Min(scaleX, scaleY);

            if (scale < 1.0)
            {
                return scale;
            }

            return 1.0; // Otherwise, display at 1:1 scale
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

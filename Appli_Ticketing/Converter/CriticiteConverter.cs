using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Appli_Ticketing.Converter
{
    public class CriticiteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int criticite)
            {
                if (criticite >= 0 && criticite <= 30)
                    return new SolidColorBrush(Colors.Green); 
                if (criticite >= 31 && criticite <= 70)
                    return new SolidColorBrush(Colors.Orange); 
                if (criticite >= 71 && criticite <= 100)
                    return new SolidColorBrush(Colors.Red); 
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace Appli_Ticketing.Converter
{ 
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date.ToString("dddd dd MMMM yyyy 'à' HH:mm", new CultureInfo("fr-FR"));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
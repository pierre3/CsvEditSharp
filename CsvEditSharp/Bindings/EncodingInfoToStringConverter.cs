using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CsvEditSharp.Bindings
{
    public class EncodingInfoToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var encodings = value as EncodingInfo[];
            if (encodings == null) { return value; }
            return encodings.Select(x => x.DisplayName).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

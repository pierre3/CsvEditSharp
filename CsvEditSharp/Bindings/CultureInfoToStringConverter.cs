using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CsvEditSharp.Bindings
{
    public class CultureInfoToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cultureInfos = value as CultureInfo[];
            if (cultureInfos == null) { return value; }
            return cultureInfos.Select(x => x.DisplayName).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CsvEditSharp.Bindings
{
    public class DataGridColumnConverter : IValueConverter
    {

        private ITypeConverter typeConverter;
        private MemberMapData memberMapData;
        public string HeaderName { get; }
        
        public DataGridColumnConverter(MemberMapData mapData)
        {
            var headerName = mapData.Names[mapData.NameIndex];
            var converter = mapData.TypeConverter;
            typeConverter = converter ?? new DefaultTypeConverter();
            memberMapData = mapData;
            HeaderName = headerName;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string)) { return value; }
            try
            {
                return typeConverter.ConvertToString(value, null, memberMapData);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var oldValue = value as string;
            if (oldValue == null) { return value; }
            try
            {
                return typeConverter.ConvertFromString(oldValue, null, memberMapData);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}

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
        private MemberMapData memberMapData;
        private ITypeConverter converter;
        public string HeaderName { get; }
        
        public DataGridColumnConverter(CsvConfiguration configuration, MemberMapData mapData)
        {
            var headerName = mapData.Member.Name;
            if (mapData.Names.Count > 0)
            {
                headerName = mapData.Names[mapData.NameIndex];
            }
            if(mapData.TypeConverterOptions.CultureInfo == null)
            {
                mapData.TypeConverterOptions.CultureInfo = configuration.CultureInfo;
            }
            converter = mapData.TypeConverter
                ?? configuration.TypeConverterCache.GetConverter(mapData.Member)
                ?? new DefaultTypeConverter();
            memberMapData = mapData;
            HeaderName = headerName;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string)) { return value; }
            try
            {
                return converter.ConvertToString(value, null, memberMapData);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string oldValue)) { return value; }
            try
            {
                return converter.ConvertFromString(oldValue, null, memberMapData);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}

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
        private TypeConverterOptions converterOptions;

        public string HeaderName { get; }
        
        public DataGridColumnConverter(string headerName, ITypeConverter converter, TypeConverterOptions options)
        {
            typeConverter = converter ?? new DefaultTypeConverter();
            converterOptions = options ?? new TypeConverterOptions();
            HeaderName = headerName;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string)) { return value; }
            try
            {
                return typeConverter.ConvertToString(converterOptions, value);
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
                return typeConverter.ConvertFromString(converterOptions, oldValue);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}

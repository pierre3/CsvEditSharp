using System.Linq;
using CsvHelper.TypeConversion;

namespace CsvEditSharp.Models
{
    public class CustomBooleanConverter : BooleanConverter
    {
        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            if (!(value is bool)) { return null; }
            var b = (bool)value;
            if (options != null)
            {
                var trueString = options.BooleanTrueValues.FirstOrDefault();
                var falseString = options.BooleanFalseValues.FirstOrDefault();
                if (b)
                {
                    if (!string.IsNullOrEmpty(trueString))
                    {
                        return trueString;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(falseString))
                    {
                        return falseString;
                    }
                }
            }
            return base.ConvertToString(options, value);
        }
    }
}

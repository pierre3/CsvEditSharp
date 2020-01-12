using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CsvEditSharp.Models
{
    public class CustomBooleanConverter : BooleanConverter
    {
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            
            if (!(value is bool)) { return null; }
            var b = (bool)value;
            var options = memberMapData.TypeConverterOptions;
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
            return base.ConvertToString(value,row,memberMapData);
        }
    }
}

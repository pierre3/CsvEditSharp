using System.Globalization;
using System.Text;

namespace CsvEditSharp.Models
{
    public class GenerateConfigSettings
    {
        public string TemplateName { get; set; } = string.Empty;

        public Encoding TargetFileEncoding { get; set; }
        public CultureInfo CaltureInfo { get; set; }

        public string TargetFileEncodingName
        {
            get { return TargetFileEncoding.WebName; }
            set
            {
                TargetFileEncoding = Encoding.GetEncoding(value);
            }
        }

        public bool HasHeaderRecord { get; set; }
    }
    
}

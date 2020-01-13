using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CsvEditSharp.T4
{
    public partial class ConfigurationTemplateGenerator
    {
        private IEnumerable<PropertyDefs> Prop { get; }
        private bool HasHeaders { get; }
        private string EncodingName { get; }
        private CultureInfo CultureInfo { get; }
        public ConfigurationTemplateGenerator(string encodingName, CultureInfo cultureInfo, IEnumerable<string> firstRow, IEnumerable<string> headers = null)
        {
            if (firstRow == null) { throw new ArgumentNullException(nameof(firstRow)); }

            EncodingName = encodingName;
            HasHeaders = headers != null;
            CultureInfo = cultureInfo?? CultureInfo.CurrentCulture;
            Prop = firstRow.Select(field => FieldToTypeName(field))
                .Zip(GenerateColumnDefs(headers), (type, defs) =>
                    new PropertyDefs
                    {
                        Column = defs,
                        Name = IsIdentifier(defs.Name) ? defs.Name : $"column_{ defs.Index}",
                        Type = type,
                        CultureInfo = CultureInfo
                    });
        }

        private IEnumerable<ColumnDefs> GenerateColumnDefs(IEnumerable<string> headers)
        {
            var i = 0;
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    yield return new ColumnDefs { Index = i++, Name = h };
                }
            }
            else
            {
                while (true)
                {
                    yield return new ColumnDefs { Index = i++, Name = null };
                }
            }
        }

        private bool IsIdentifier(string s)
        {

            if (s == null || s.Length == 0) { return false; }

            if (!SyntaxFacts.IsIdentifierStartCharacter(s[0]))
            {
                return false;
            }
            return s.Skip(1).All(c => SyntaxFacts.IsIdentifierPartCharacter(c));
        }

        private string FieldToTypeName(string field)
        {
            if (field.ToLower() == "true" || field.ToLower() == "false")
            {
                return "bool";
            }

            if (DateTime.TryParse(field,CultureInfo, DateTimeStyles.None , out DateTime _))
            {
                return "DateTime";
            }

            if (decimal.TryParse(field,NumberStyles.Any, CultureInfo, out decimal _))
            {
                return "decimal";
            }
            
            return "string";
        }

        private class ColumnDefs
        {
            public int Index { get; set; }
            public string Name { get; set; }
        }

        private class PropertyDefs
        {
            public ColumnDefs Column { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public CultureInfo CultureInfo { get;set; }

        }
    }
}

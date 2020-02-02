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
        private bool AutoTypeDetection { get; }
        public ConfigurationTemplateGenerator(string encodingName, CultureInfo cultureInfo, bool autoTypeDetection, IEnumerable<string> firstRow, IEnumerable<string> headers = null)
        {
            if (firstRow == null && headers == null) { throw new System.IO.InvalidDataException("Invalid CSV file format."); }

            EncodingName = encodingName;
            HasHeaders = headers != null;
            CultureInfo = cultureInfo ?? CultureInfo.CurrentCulture;
            Prop = DetectFieldTypeNames(firstRow)
                .Zip(GenerateColumnDefs(headers), (type, defs) =>
                {
                    var columnName = defs.Name;
                    if (string.IsNullOrEmpty(defs.Name))
                    {
                        defs.UseIndex = true;
                    }
                    else
                    {
                        columnName = ReplaceIdentifier(defs.Name);
                        if (defs.Name != columnName)
                        {
                            defs.UseName = true;
                        }
                    }
                    return new PropertyDefs
                    {
                        Column = defs,
                        Name = defs.UseIndex
                            ? $"Column_{ defs.Index}"
                            : columnName,
                        Type = type,
                        CultureInfo = CultureInfo
                    };
                });
            AutoTypeDetection = autoTypeDetection;
        }

        private IEnumerable<string> DetectFieldTypeNames(IEnumerable<string> firstRow)
        {
            if (firstRow == null)
            {
                while (true)
                {
                    yield return "string";
                }
            }
            else
            {
                foreach (var field in firstRow)
                {
                    yield return FieldToTypeName(field);
                }
            }
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

        private string ReplaceIdentifier(string s)
        {
            if (string.IsNullOrEmpty(s)) { return s; }

            var startChar = SyntaxFacts.IsIdentifierStartCharacter(s[0]) ? s[0] : '_';

            return new string(new[] { startChar }.Concat(
                s.Skip(1)
                    .Select(c => SyntaxFacts.IsIdentifierPartCharacter(c) ? c : '_'))
                    .ToArray());
        }

        private string FieldToTypeName(string field)
        {
            if (!AutoTypeDetection) { return "string"; }
            if (field.ToLower() == "true" || field.ToLower() == "false")
            {
                return "bool";
            }

            if (DateTime.TryParse(field, CultureInfo, DateTimeStyles.None, out DateTime _))
            {
                return "DateTime";
            }

            if (decimal.TryParse(field, NumberStyles.Any, CultureInfo, out decimal _))
            {
                return "decimal";
            }

            return "string";
        }

        private class ColumnDefs
        {
            public int Index { get; set; }
            public string Name { get; set; }

            public bool UseIndex { get; set; }
            public bool UseName { get; set; }

        }

        private class PropertyDefs
        {
            public ColumnDefs Column { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public CultureInfo CultureInfo { get; set; }

        }
    }
}

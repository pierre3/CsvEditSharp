using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CsvEditSharp.T4
{
    public partial class ConfigurationTemplateGenerator
    {
        private IEnumerable<PropertyDefs> Prop { get; }
        private bool HasHeaders { get; }
        private string EncodingName { get; }
        public ConfigurationTemplateGenerator(string encodingName, IEnumerable<string> firstRow, IEnumerable<string> headers = null)
        {
            if (firstRow == null) { throw new ArgumentNullException(nameof(firstRow)); }

            EncodingName = encodingName;
            HasHeaders = headers != null;

            Prop = firstRow.Select(field => FieldToTypeName(field))
                .Zip(GenerateColumnDefs(headers), (type, defs) =>
                    new PropertyDefs
                    {
                        Column = defs,
                        Name = IsIdentifier(defs.Name) ? defs.Name : $"column_{ defs.Index}",
                        Type = type
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
            /* 
            bool b;
            if (bool.TryParse(field, out b))
            {
                return "bool";
            }

            DateTime dt;
            if (DateTime.TryParse(field, out dt))
            {
                return "DateTime";
            }

            double d;
            if (double.TryParse(field, out d))
            {
                return "double";
            }
            */
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

        }
    }
}

using System;
using System.Globalization;
using System.Windows.Controls;

namespace CsvEditSharp.Bindings
{
    public class DataGridColumnValidationRule : ValidationRule
    {
        private Func<object, bool> isValidate;
        private object errorContent;

        public DataGridColumnValidationRule(Func<object, bool> isValidate, object errorContent)
        {
            if (isValidate == null) { throw new ArgumentNullException(nameof(isValidate)); }
            if (errorContent == null) { throw new ArgumentNullException(nameof(errorContent)); }

            ValidationStep = ValidationStep.ConvertedProposedValue;
            this.isValidate = isValidate;
            this.errorContent = errorContent ?? "invalid value";
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return (isValidate(value)) ?
                ValidationResult.ValidResult :
                new ValidationResult(false, errorContent);
        }
    }
}

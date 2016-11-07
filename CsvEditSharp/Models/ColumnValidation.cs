using System;

namespace CsvEditSharp.Models
{
    public class ColumnValidation
    {
        public Func<object, bool> Validation { get; }
        public string ErrorMessage { get; }

        public ColumnValidation(Func<object, bool> validation, string errorMessage)
        {
            Validation = validation;
            ErrorMessage = errorMessage;
        }

    }
}

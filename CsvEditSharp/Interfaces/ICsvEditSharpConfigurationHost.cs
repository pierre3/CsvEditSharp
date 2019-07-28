using CsvEditSharp.Enums;
using CsvEditSharp.Models;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CsvEditSharp.Interfaces
{
    public interface ICsvEditSharpConfigurationHost
    {
        Encoding Encoding { get; set; }
        CsvClassMap ClassMapForReading { get; }
        CsvClassMap ClassMapForWriting { get; }
        IDictionary<string, ColumnValidation> ColumnValidations { get; }

        void RegisterClassMap<T>();

        void RegisterClassMap<T>(Action<CsvClassMap<T>> propertyMapSetter);

        void RegisterClassMap<T>(Action<CsvClassMap<T>> propertyMapSetter, RegisterClassMapTarget target);

        void SetConfiguration(Action<CsvConfiguration> configurationSetter);

        void AddValidation<TType, TMember>(Expression<Func<TType, TMember>> memberSelector, Func<TMember, bool> validation, string errorMessage);

        void Query<T>(Func<IEnumerable<T>, IEnumerable<T>> query);

        void Query<T>(Action<IEnumerable<T>> query);
    }


}

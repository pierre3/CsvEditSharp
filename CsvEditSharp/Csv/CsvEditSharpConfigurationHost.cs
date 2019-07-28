﻿using CsvEditSharp.Enums;
using CsvEditSharp.Interfaces;
using CsvEditSharp.Models;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CsvEditSharp.Csv
{
    public class CsvEditSharpConfigurationHost : ICsvEditSharpConfigurationHost
    {
        private Action<CsvConfiguration> configurationSetter;

        private Type recordType;
        private IList<object> itemsSource;
        private Func<IEnumerable<object>, IEnumerable<object>> query;

        public IEnumerable<object> Records { get; private set; }
        public CsvClassMap ClassMapForReading { get; private set; }

        public CsvClassMap ClassMapForWriting { get; private set; }

        public Encoding Encoding { get; set; }

        public IDictionary<string, ColumnValidation> ColumnValidations { get; } = new Dictionary<string, ColumnValidation>();

        public CsvEditSharpConfigurationHost()
        {
            Reset();
        }

        public void RegisterClassMap<T>()
        {
            recordType = typeof(T);
        }

        public void RegisterClassMap<T>(Action<CsvClassMap<T>> propertyMapSetter)
        {
            RegisterClassMap(propertyMapSetter, RegisterClassMapTarget.Both);
        }

        public void RegisterClassMap<T>(Action<CsvClassMap<T>> propertyMapSetter, RegisterClassMapTarget target)
        {
            recordType = typeof(T);
            if (propertyMapSetter != null)
            {
                var classMap = new AnonimousCsvClassMap<T>(propertyMapSetter);
                switch (target)
                {
                    case RegisterClassMapTarget.Reader:
                        ClassMapForReading = classMap;
                        break;
                    case RegisterClassMapTarget.Writer:
                        ClassMapForWriting = classMap;
                        break;
                    case RegisterClassMapTarget.Both:
                        ClassMapForReading = classMap;
                        ClassMapForWriting = classMap;
                        break;
                }
            }
        }

        public void SetConfiguration(Action<CsvConfiguration> configurationSetter)
        {
            this.configurationSetter = configurationSetter;
        }

        public void Query<T>(Func<IEnumerable<T>, IEnumerable<T>> query)
        {
            this.query = source => query(source.Cast<T>()).Cast<object>();
        }

        public void Query<T>(Action<IEnumerable<T>> query)
        {
            this.query = source =>
            {
                query(source.Cast<T>());
                return source;
            };
        }

        public void ExecuteQuery()
        {
            Records = query?.Invoke(itemsSource) ?? itemsSource;
        }

        public void ResetQuery()
        {
            Records = itemsSource;
        }

        public void Read(TextReader baseReader)
        {
            using (var reader = new CsvReader(baseReader))
            {
                configurationSetter?.Invoke(reader.Configuration);
                if (ClassMapForReading != null)
                {
                    reader.Configuration.RegisterClassMap(ClassMapForReading);
                }
                if (recordType != null)
                {
                    itemsSource = reader.GetRecords(recordType).ToList();
                    Records = itemsSource;
                }
            }
        }

        public void Write(TextWriter baseWriter, IEnumerable records)
        {
            using (var writer = new CsvWriter(baseWriter))
            {
                configurationSetter?.Invoke(writer.Configuration);
                if (ClassMapForWriting != null)
                {
                    var booleanMaps = ClassMapForWriting.PropertyMaps
                        .Where(map => map.Data.TypeConverter is BooleanConverter);
                    foreach (var map in booleanMaps)
                    {
                        map.Data.TypeConverter = new CustomBooleanConverter();
                    }

                    writer.Configuration.RegisterClassMap(ClassMapForWriting);
                }
                writer.WriteRecords(records);
            }
        }

        public void Reset()
        {
            itemsSource = new object[0];
            Records = itemsSource;
            ClassMapForReading = null;
            ClassMapForWriting = null;
            recordType = null;
            configurationSetter = null;
            Encoding = Encoding.Default;
            ColumnValidations.Clear();
        }

        public void AddValidation<TType, TMember>(Expression<Func<TType, TMember>> memberSelector, Func<TMember, bool> validation, string errorMessage)
        {
            MemberExpression memberExpression = null;
            if (memberSelector.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)memberSelector.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (memberSelector.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = memberSelector.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", nameof(memberSelector));
            }
            ColumnValidations.Add(memberExpression.Member.Name, new ColumnValidation(m => validation((TMember)m), errorMessage));

        }

        private class AnonimousCsvClassMap<T> : CsvClassMap<T>
        {
            public AnonimousCsvClassMap(Action<CsvClassMap<T>> propertyMapSetter)
            {
                propertyMapSetter(this);
            }
        }


    }


}

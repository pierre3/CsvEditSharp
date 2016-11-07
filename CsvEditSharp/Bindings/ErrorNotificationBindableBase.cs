using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace CsvEditSharp.Bindings
{
    public abstract class ErrorNotificationBindableBase : BindableBase, INotifyDataErrorInfo
    {
        private IDictionary<string, IList<object>> _errors = new Dictionary<string, IList<object>>();

        public bool HasErrors
        {
            get
            {
                return _errors.Count > 0;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                return _errors[propertyName];
            }
            return new object[0];
        }

        protected void RaiseErrorsChanged(string propertyName)
            => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        protected void AddErrorInfo(string propertyName, object errorInfo)
        {
            if (_errors.ContainsKey(propertyName))
            {
                var propErrors = _errors[propertyName];
                if (!propErrors.Contains(errorInfo))
                {
                    propErrors.Add(errorInfo);
                }
            }
            else
            {
                _errors.Add(propertyName, new List<object> { errorInfo });
            }
            RaiseErrorsChanged(propertyName);
        }

        protected void RemoveErrorInfo(string propertyName, object errorInfo)
        {
            if (string.IsNullOrWhiteSpace(propertyName) ||
                !_errors.ContainsKey(propertyName)) { return; }

            var propErrors = _errors[propertyName];
            if (propErrors.Contains(errorInfo)) { return; }

            propErrors.Remove(errorInfo);
            if (propErrors.Count == 0)
            {
                _errors.Remove(propertyName);
            }
            RaiseErrorsChanged(propertyName);
            
        }
        protected void ClearErrorInfo(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                RaiseErrorsChanged(propertyName);
            }
        }
    }
}

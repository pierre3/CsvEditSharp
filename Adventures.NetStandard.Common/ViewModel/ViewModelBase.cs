using CsvEditSharp.Events;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using ioc = Unity;

namespace Adventures.NetStandard.Common.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DelegateCommand<object> _buttonCommand;
        public DelegateCommand<object> ButtonCommand
        {
            get { return _buttonCommand; }
            set { _buttonCommand = value; OnPropertyChanged(); }
        } 

        [ioc.Dependency] public IEventAggregator EventAggregator {get;set;}

        public ViewModelBase()
        {
            ButtonCommand = new DelegateCommand<object>(ButtonExecute, CanButtonExecute);
        }

        public void ButtonCommandRefresh()
        {
            ButtonCommand.RaiseCanExecuteChanged();
        }

        public void ButtonExecute(object obj)
        {
            var button = obj as Button;
            var args = new CommandButtonEventArgs
            {
                Button = obj,
                ButtonCommand = ButtonCommand,
                CommandType = Enums.CommandType.Execute

            };

            EventAggregator?.GetEvent<CommandButtonEvent>().Publish(args);
        }


        public bool CanButtonExecute(object obj)
        {
            var button = obj as Button;
            var args = new CommandButtonEventArgs
            {
                Button = obj,
                ButtonCommand = ButtonCommand,
                CommandType = Enums.CommandType.CanExecute,
                CanExecute = true // default
            };

            EventAggregator?.GetEvent<CommandButtonEvent>().Publish(args);
            return args.CanExecute; // return value set by handlers
        }


        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    OnDispose();

                disposedValue = true;
            }
        }

        protected virtual void OnDispose() { }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}

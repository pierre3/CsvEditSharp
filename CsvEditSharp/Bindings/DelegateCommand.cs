using System;
using System.Windows.Input;

namespace CsvEditSharp.Bindings
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter)??true;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
       
    }
}

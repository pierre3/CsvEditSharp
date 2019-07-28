using CsvEditSharp.ViewModels;
using System.Collections.ObjectModel;

namespace CsvEditSharp.Commands
{
    public class ResetQueryCommand : CommandBase
    {
        public static string CommandName = "ResetQueryCommand";

        public MainWindowViewModel MainViewModel => (MainWindowViewModel)ViewModel;

        public override bool CanExecute(object parameter)
        {
            return MainViewModel.Workspace.HasScriptState;
        }

        public override void Execute(object parameter)
        {
            MainViewModel.Host.ResetQuery();

            MainViewModel.CsvRows = 
                new ObservableCollection<object>(MainViewModel.Host.Records);
        }
    }
}

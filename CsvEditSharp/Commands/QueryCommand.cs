using CsvEditSharp.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace CsvEditSharp.Commands
{
    public class QueryCommand : CommandBase
    {
        public static string CommandName = "QueryCommand";

        public MainWindowViewModel MainViewModel => (MainWindowViewModel)ViewModel;

        public override bool CanExecute(object parameter)
        {
            return MainViewModel.Workspace.HasScriptState;
        }

        public async override void Execute(object parameter)
        {
            MainViewModel.ErrorMessages.Clear();

            await MainViewModel.Workspace.ContinueScriptAsync(MainViewModel.QueryDoc.Text);
            try
            {
                MainViewModel.Host.ExecuteQuery();
                MainViewModel.CsvRows = new ObservableCollection<object>(MainViewModel.Host.Records);
            }
            catch (Exception e)
            {
                MainViewModel.ErrorMessages.Add(e.ToString());
            }
        }
    }
}

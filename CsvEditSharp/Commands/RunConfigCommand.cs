using CsvEditSharp.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace CsvEditSharp.Commands
{
    public class RunConfigCommand : CommandBase
    {
        public static string CommandName = "RunConfigCommand";

        public MainWindowViewModel MainViewModel => (MainWindowViewModel)ViewModel;

        public override void Execute(object parameter)
        {
            MainViewModel.Host.Reset();
            MainViewModel.ErrorMessages.Clear();

            MainViewModel.Workspace.RunScriptAsync(MainViewModel.ConfigurationDoc.Text);

            try
            {
                using (var stream = new FileStream(MainViewModel.CurrentFilePath, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(stream, MainViewModel.Host.Encoding ?? Encoding.Default))
                {
                    MainViewModel.Host.Read(reader);
                }
            }
            catch (Exception e)
            {
                MainViewModel.ErrorMessages.Add(e.ToString());
            }

            MainViewModel.CsvRows = new ObservableCollection<object>(MainViewModel.Host.Records);
            MainViewModel.SelectedTab = 0;
            MainViewModel.ButtonCommandRefresh();

        }
    }
}

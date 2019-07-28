using CsvEditSharp.Csv;
using CsvEditSharp.Interfaces;
using CsvEditSharp.ViewModels;
using System.IO;
using System.Windows.Threading;
using Unity;

namespace CsvEditSharp.Commands
{
    public class ReadCsvCommand : CommandBase
    {
        public static string CommandName = "ReadCsvCommand";

        [Dependency] public IViewServiceProvider ViewService { get; set; }

        public MainWindowViewModel MainViewModel => (MainWindowViewModel) ViewModel;

        public async override void Execute(object para)
        {
            string currentFilePath = null;

            // para will be null if invoked by Loaded event
            if (para == null)
            {
                // If there is a parameter then load the file
                if (StartupArgs.Args.Length > 0)
                    currentFilePath = StartupArgs.Args[0];
                else
                    // Otherwise, we'll exit the load event
                    return;
            }

            var openFileService = ViewService.OpenFileSelectionService;
            MainViewModel.CurrentFilePath = currentFilePath == null
                ? openFileService.SelectFile("Select a CSV File", MainViewModel.CsvFileFilter, null)
                : currentFilePath;

            if (!File.Exists(MainViewModel.CurrentFilePath)) { return; }

            var configText = CsvConfigFileManager.Default
                .GetCsvConfigString(MainViewModel.CurrentFilePath, MainViewModel.SelectedTemplate);

            MainViewModel.CurrentConfigName = Path.GetFileName(CsvConfigFileManager.Default.CurrentConfigFilePath);
            MainViewModel.CurrentFileName = Path.GetFileName(MainViewModel.CurrentFilePath);

            MainViewModel.ConfigurationDoc.Text = configText;

            // Let UI refresh before long running task
            await Dispatcher.Yield(DispatcherPriority.ApplicationIdle);

            InvokeCommand(RunConfigCommand.CommandName, para);
        }

    }
}

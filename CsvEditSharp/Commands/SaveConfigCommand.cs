using CsvEditSharp.Csv;
using CsvEditSharp.ViewModels;
using System.IO;

namespace CsvEditSharp.Commands
{
    public class SaveConfigCommand : CommandBase
    {
        public static string CommandName = "SaveConfigCommand";

        public MainWindowViewModel MainViewModel => (MainWindowViewModel)ViewModel;

        public override bool CanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(MainViewModel.ConfigurationDoc.Text)
               && File.Exists(CsvConfigFileManager.Default.CurrentConfigFilePath);
        }

        public override void Execute(object parameter)
        {
            CsvConfigFileManager.Default.SaveConfigFile(MainViewModel.ConfigurationDoc.Text, null);
        }
    }
}

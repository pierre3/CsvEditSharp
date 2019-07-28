using CsvEditSharp.Csv;
using CsvEditSharp.ViewModels;
using System.Diagnostics;

namespace CsvEditSharp.Commands
{
    public class DeleteTemplateCommand: CommandBase
    {
        public static string CommandName = "DeleteTemplateCommand";

        public MainWindowViewModel MainViewModel => (MainWindowViewModel)ViewModel;

        public override bool CanExecute(object parameter)
        {
            Debug.WriteLine("Can Execute");
            return !string.IsNullOrEmpty(MainViewModel.SelectedTemplate);
        }

        public override void Execute(object parameter)
        {
            Debug.WriteLine("Execute");
            CsvConfigFileManager.Default.RemoveConfigFile(MainViewModel.SelectedTemplate);
        }

    }
}

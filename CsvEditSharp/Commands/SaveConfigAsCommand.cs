using CsvEditSharp.Csv;
using CsvEditSharp.Interfaces;
using CsvEditSharp.ViewModels;
using System.IO;
using Unity;

namespace CsvEditSharp.Commands
{
    public class SaveConfigAsCommand : CommandBase
    {
        public static string CommandName = "SaveConfigAsCommand";
         
        [Dependency] public IViewServiceProvider ViewService { get; set; }

        public MainWindowViewModel MainViewModel => (MainWindowViewModel)ViewModel;

        public override bool CanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(MainViewModel.ConfigurationDoc.Text)
               && File.Exists(CsvConfigFileManager.Default.CurrentConfigFilePath);
        }

        public override void Execute(object parameter)
        {
            var service = ViewService.SaveConfigDialogService;
            if (true == service.ShowModal())
            {
                var fileName = string.Empty;
                if (service.Result.IsTemplate)
                {
                    fileName = CsvConfigFileManager.Default.MakeCurrentConfigFilePath(service.Result.TemplateName);
                }
                else
                {
                    fileName = Path.Combine(Path.GetDirectoryName(MainViewModel.CurrentFilePath), "Default.config.csx");
                }
                MainViewModel.CurrentConfigName = Path.GetFileName(fileName);
                CsvConfigFileManager.Default.CurrentConfigFilePath = fileName;
                CsvConfigFileManager.Default.SaveConfigFile(MainViewModel.ConfigurationDoc.Text, service.Result.TemplateName);
            }
        }
    }
}

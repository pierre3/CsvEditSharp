using CsvEditSharp.Models;
using CsvEditSharp.Services;
using CsvEditSharp.ViewModels;
using System.Windows;
using System.Linq;

namespace CsvEditSharp
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var viewService = new ViewServiceProvider();
            CsvConfigFileManager.InitializeDefault(viewService.GenerateConfigDialogService);
            var vm = new MainWindowViewModel(viewService, e.Args.FirstOrDefault());
            MainWindow = new MainWindow(vm);
            MainWindow.Show();
        }

    }
}

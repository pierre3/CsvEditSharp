using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;

namespace CsvEditSharp.Services
{
    class ConfigSettingsDialogService : IModalDialogService
    {
        public bool? ShowModal()
        {
            var dialog = new ConfigSettingsDialog();
            dialog.Owner = App.Current.MainWindow;
            dialog.DataContext = new ConfigSettingsDialogViewModel();
            return dialog.ShowDialog();               
        }
    }
}

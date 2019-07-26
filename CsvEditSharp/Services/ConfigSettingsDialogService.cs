using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;
using Unity;

namespace CsvEditSharp.Services
{
    class ConfigSettingsDialogService : IModalDialogService
    {
        [Dependency]
        public ConfigSettingsDialogViewModel DialogViewModel { get; set; }

        [Dependency]
        public ConfigSettingsDialog Dialog { get; set; }

        public bool? ShowModal()
        {
            Dialog.Owner = App.Current.MainWindow;
            Dialog.DataContext = DialogViewModel;
            return Dialog.ShowDialog();               
        }
    }
}

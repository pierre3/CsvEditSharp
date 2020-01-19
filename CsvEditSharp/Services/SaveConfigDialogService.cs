using CsvEditSharp.Models;
using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;

namespace CsvEditSharp.Services
{
    public class SaveConfigDialogService : IModalDialogService<SaveConfigSettings>
    {
        public SaveConfigSettings Result { get; private set; }
        public bool? ShowModal(params object[] parameters)
        {
            var vm = new SaveConfigDialogViewModel();
            var dialog = new SaveConfigDialog();
            dialog.Owner = App.Current.MainWindow;
            dialog.DataContext = vm;
            var result = dialog.ShowDialog();
            if (result == true)
            {
                Result = new SaveConfigSettings {
                    IsTemplate = vm.IsTemplate,
                    TemplateName = vm.TemplateName
                };
            }
            return result;
        }
    }
}

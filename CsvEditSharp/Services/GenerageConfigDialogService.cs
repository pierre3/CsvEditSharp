using CsvEditSharp.Models;
using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;

namespace CsvEditSharp.Services
{
    public class GenerateConfigDialogService : IModalDialogService<GenerateConfigSettings>
    {
        public GenerateConfigSettings Result { get; private set; }
        public bool? ShowModal()
        {
            var vm = new GenerateConfigDialogViewModel();
            var dialog = new GenerateConfigDialog();
            dialog.DataContext = vm;
            var result = dialog.ShowDialog();
            if (result == true)
            {
                Result = vm.Settings;
            }
            return result;
        }
    }
}

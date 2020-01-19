using CsvEditSharp.Models;
using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;
using System.Linq;

namespace CsvEditSharp.Services
{
    public class GenerateConfigDialogService : IModalDialogService<GenerateConfigSettings>
    {
        public GenerateConfigSettings Result { get; private set; }
        public bool? ShowModal(params object[] parameters)
        {
            var vm = new GenerateConfigDialogViewModel((string)parameters?.FirstOrDefault());
            var dialog = new GenerateConfigDialog();
            dialog.Owner = App.Current.MainWindow;
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

using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;
using System.Linq;

namespace CsvEditSharp.Services
{
    class SelectConfigurationDialogService : IModalDialogService<string>
    {
        public string Result { get; private set; }

        public bool? ShowModal(params object[] parameters)
        {
            var dialog = new SelectConfigurationDialog();
            var vm = new SelectConfigurationDialogViewModel((string)parameters.FirstOrDefault());
            dialog.Owner = App.Current.MainWindow;
            dialog.DataContext = vm;
            var ret = dialog.ShowDialog();
            if (ret == true)
            {
                Result = vm.ConfigurationText;
            }
            return ret;
        }
    }
}

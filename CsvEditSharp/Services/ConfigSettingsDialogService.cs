using CsvEditSharp.Extensions;
using CsvEditSharp.Interfaces;
using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;
using Unity;

namespace CsvEditSharp.Services
{
    class ConfigSettingsDialogService : IModalDialogService
    {
        [Dependency] public IUnityContainer IocContainer { get; set; }

        [Dependency] public ConfigSettingsDialogViewModel DialogViewModel { get; set; }

        public bool? ShowModal()
        {
            IocContainer.ShowDialog<ConfigSettingsDialog>(DialogViewModel);
            return false;
        }

    }
}

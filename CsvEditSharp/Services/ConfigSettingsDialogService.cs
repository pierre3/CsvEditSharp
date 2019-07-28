using CsvEditSharp.Extensions;
using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;
using Unity;

namespace CsvEditSharp.Services
{
    public class ConfigSettingsDialogService : ServiceBase
    {
        [Dependency] public ConfigSettingsDialogViewModel DialogViewModel { get; set; }

        public override bool? ShowModal()
        {
            IocContainer.ShowDialog<ConfigSettingsDialog>(DialogViewModel);
            return false;
        }

    }
}

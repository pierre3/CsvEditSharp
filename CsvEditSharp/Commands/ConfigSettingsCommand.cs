using CsvEditSharp.Interfaces;
using Unity;

namespace CsvEditSharp.Commands
{
    public class ConfigSettingsCommand : CommandBase
    {
        public static string CommandName = "ConfigSettingsCommand";

        [Dependency] public IViewServiceProvider ViewService { get; set; }

        public override void Execute(object parameter)
        {
            ViewService.ConfigSettingsDialogService.ShowModal();
        }

    }
}

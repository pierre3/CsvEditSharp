namespace CsvEditSharp.Commands
{
    public class ConfigSettingsCommand : CommandBase
    {
        public static string CommandName = "ConfigSettingsCommand";

        public override void Execute(object parameter)
        {
            ViewService.ConfigSettingsDialogService.ShowModal();
        }

    }
}

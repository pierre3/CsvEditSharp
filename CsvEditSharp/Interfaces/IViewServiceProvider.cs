using CsvEditSharp.Models;

namespace CsvEditSharp.Interfaces
{
    public interface IViewServiceProvider
    {
        IFileSelectionService OpenFileSelectionService { get; }
        IFileSelectionService SaveFileSelectionService { get; }
        IModalDialogService<GenerateConfigSettings> GenerateConfigDialogService { get; }
        IModalDialogService<SaveConfigSettings> SaveConfigDialogService { get; }
        IModalDialogService ConfigSettingsDialogService { get; }
    }

}

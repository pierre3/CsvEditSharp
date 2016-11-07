using CsvEditSharp.Models;

namespace CsvEditSharp.Services
{
    public interface IViewServiceProvider
    {
        IFileSelectionService OpenFileSelectionService { get; }
        IFileSelectionService SaveFileSelectionService { get; }
        IModalDialogService<GenerateConfigSettings> GenerateConfigDialogService { get; }
        IModalDialogService<SaveConfigSettings> SaveConfigDialogService { get; }
    }

}

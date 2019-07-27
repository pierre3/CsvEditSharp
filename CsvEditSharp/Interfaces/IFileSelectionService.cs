namespace CsvEditSharp.Interfaces
{
    public interface IFileSelectionService
    {
        string SelectFile(string title, string filter, string initialDirectory);
    }

}

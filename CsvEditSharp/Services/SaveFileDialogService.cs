using Microsoft.Win32;

namespace CsvEditSharp.Services
{
    public class SaveFileDialogService : IFileSelectionService
    {
        public string SelectFile(string title, string filter, string initialDirectory)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = title;
            dialog.Filter = filter;
            dialog.InitialDirectory = initialDirectory;
            if (false == dialog.ShowDialog())
            {
                return null;
            }
            return dialog.FileName;
        }
    }
}

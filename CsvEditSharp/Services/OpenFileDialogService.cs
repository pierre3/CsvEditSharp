using Microsoft.Win32;

namespace CsvEditSharp.Services
{
    public class OpenFileDialogService : IFileSelectionService
    {
        public string SelectFile(string title, string filter, string initialDirectory)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = title;
            dialog.Filter = filter;
            dialog.InitialDirectory = initialDirectory ?? "";
            dialog.Multiselect = false;
            if (false == dialog.ShowDialog())
            {
                return null;
            }
            return dialog.FileName;
        }
    }
}

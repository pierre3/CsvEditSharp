using System.Windows;
namespace CsvEditSharp.Services
{
    public class OkCancelDialogService : IModalDialogService
    {
        public bool? ShowModal(params object[] parameters)
        {
            var ret = System.Windows.MessageBox.Show((string)parameters[0], (string)parameters[1],MessageBoxButton.OKCancel,MessageBoxImage.Question);
            return (ret == MessageBoxResult.OK);
        }
    }

}

namespace CsvEditSharp.Services
{
    public class MessageDialogService : IModalDialogService
    {
        public bool? ShowModal(params object[] parameters)
        {
            System.Windows.MessageBox.Show((string)parameters[0], (string)parameters[1]);
            return true;
        }
    }

}

using CsvEditSharp.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace CsvEditSharp.Commands
{
    public class RunConfigCommand : CommandBase
    {
        public static string CommandName = "RunConfigCommand";

        public MainWindowViewModel MainViewModel => (MainWindowViewModel)ViewModel;

        public override void Execute(object parameter)
        {
            MainViewModel.Host.Reset();
            MainViewModel.ErrorMessages.Clear();
            MainViewModel.SelectedTab = 0;
            MainViewModel.IsLoading = true;

            StartBackgroundWork(MainViewModel.ConfigurationDoc.Text);
        }

        /// <summary>
        /// Run the expensive process in the background so our UI stays responsive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async override void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            // Get UI data passed in from StartBackgroundWork(para)
            var para = e.Argument.ToString();
            
            // Wait for the [expensive] script to complete running before updating the UI
            await MainViewModel.Workspace.RunScriptAsync(para);

            // Update the UI with the file data
            try
            {
                using (var stream = new FileStream(MainViewModel.CurrentFilePath, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(stream, MainViewModel.Host.Encoding ?? Encoding.Default))
                {
                    MainViewModel.Host.Read(reader);
                }
            }
            catch (Exception ex)
            {
                MainViewModel.ErrorMessages.Add(ex.ToString());
            }

            MainViewModel.CsvRows = new ObservableCollection<object>(MainViewModel.Host.Records);
            MainViewModel.ButtonCommandRefresh();
            MainViewModel.IsLoading = false;
        }
    }
}

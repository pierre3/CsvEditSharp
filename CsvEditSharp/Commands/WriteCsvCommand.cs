using CsvEditSharp.ViewModels;
using System;
using System.IO;

namespace CsvEditSharp.Commands
{
    public class WriteCsvCommand : CommandBase
    {
        public static string CommandName = "WriteCsvCommand";

        public MainWindowViewModel MainViewModel => (MainWindowViewModel)ViewModel;

        public override bool CanExecute(object parameter)
        {
            return MainViewModel.HasCsvRows;
        }

        public override void Execute(object parameter)
        {
            var saveFileService = ViewService.SaveFileSelectionService;
            var fileName = saveFileService
                .SelectFile("Save As..", MainViewModel.CsvFileFilter, MainViewModel.CurrentFilePath);

            if (fileName == null) { return; }

            try
            {
                using (var writer = new StreamWriter(fileName, false, MainViewModel.Host.Encoding))
                {
                    MainViewModel.Host.Write(writer, MainViewModel.CsvRows);
                }
            }
            catch (Exception e)
            {
                MainViewModel.ErrorMessages.Add(e.ToString());
            }
        }
    }
}

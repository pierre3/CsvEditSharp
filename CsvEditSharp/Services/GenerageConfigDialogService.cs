using CsvEditSharp.Models;
using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Unity;

namespace CsvEditSharp.Services
{
    public class GenerateConfigDialogService : IModalDialogService<GenerateConfigSettings>
    {
        public GenerateConfigSettings Result { get; private set; }

        [Dependency] public StartupEventArgs StartupArgs { get; set; }

        [Dependency] public GenerateConfigDialog Dialog { get; set; }

        [Dependency] public GenerateConfigDialogViewModel ViewModel { get; set; }

        public bool? ShowModal()
        {
            Dialog.Owner = App.Current.MainWindow;
            Dialog.DataContext = ViewModel;

            // If an argument was provided then we'll accept defaults
            if (StartupArgs.Args.Length > 0)
            {
                ViewModel.ApplyCommand.Execute(null);
                Result = ViewModel.Settings;
                return true;
            }

            var result = Dialog.ShowDialog();
            if (result == true)
            {
                Result = ViewModel.Settings;
            }
            return result;
        }

    }
}

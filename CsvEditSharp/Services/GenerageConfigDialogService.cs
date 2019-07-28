﻿using CsvEditSharp.Models;
using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;
using Unity;

namespace CsvEditSharp.Services
{
    public class GenerateConfigDialogService : ServiceBase<GenerateConfigSettings>
    {
        [Dependency] public GenerateConfigDialog Dialog { get; set; }

        [Dependency] public GenerateConfigDialogViewModel ViewModel { get; set; }

        public override bool? ShowModal()
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

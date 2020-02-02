using CsvEditSharp.Bindings;
using CsvEditSharp.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace CsvEditSharp.ViewModels
{
    public class SelectConfigurationDialogViewModel : BindableBase
    {
        private CsvConfigFileManager _configManager;

        private string _selectedItem;
        public ICommand OkCommand { get; }

        public string ConfigurationText { get; set; }
       

        public string SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public ObservableCollection<string> Items { get { return _configManager.SettingsList; } }

        public SelectConfigurationDialogViewModel(string currentFilePath)
        {
            OkCommand = new DelegateCommand(_ => SelectConfiguration(currentFilePath) , _ => SelectedItem != null);
            _configManager = CsvConfigFileManager.Default;
            SelectedItem = Items.FirstOrDefault();
        }

        private void SelectConfiguration(string currentFilePath)
        {
            ConfigurationText = CsvConfigFileManager.Default.GetCsvConfigString(currentFilePath, _selectedItem);
        }
    }
}

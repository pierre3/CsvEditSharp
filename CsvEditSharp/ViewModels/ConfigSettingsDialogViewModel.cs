using CsvEditSharp.Bindings;
using CsvEditSharp.Csv;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CsvEditSharp.ViewModels
{
    public class ConfigSettingsDialogViewModel : BindableBase
    {
        private CsvConfigFileManager _configManager;

        private string _selectedItem;
        private string _newName;
        private ICommand _renameCommand;
        private ICommand _deleteCommand;

        public string SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public string NewName
        {
            get { return _newName; }
            set { SetProperty(ref _newName, value); }
        }

        public ICommand RenameCommand
        {
            get
            {
                if (_renameCommand == null)
                {
                    _renameCommand = new DelegateCommand(
                        _ => _configManager.RenameConfigFile(SelectedItem,NewName),
                        _ => _configManager.CanRename(SelectedItem, NewName));
                }
                return _renameCommand;
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new DelegateCommand(
                        _ => _configManager.RemoveConfigFile(SelectedItem),
                        _ => _configManager.CanRemove(SelectedItem));
                }
                return _deleteCommand;
            }
        }

        public ObservableCollection<string> Items { get { return _configManager.SettingsList; } }

        public ConfigSettingsDialogViewModel()
        {
            _configManager = CsvConfigFileManager.Default;
        }
    }
}

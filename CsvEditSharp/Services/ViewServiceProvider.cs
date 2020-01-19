using CsvEditSharp.Models;

namespace CsvEditSharp.Services
{
    public class ViewServiceProvider : IViewServiceProvider
    {
        private IFileSelectionService _openFileSelectionService;
        private IFileSelectionService _saveFileSelectionService;
        private IModalDialogService<GenerateConfigSettings> _generateConfigDialogService;
        private IModalDialogService<SaveConfigSettings> _saveConfigDialogService;
        private IModalDialogService _configSettingsDialogService;
        private IModalDialogService _messageDialogService;
        public IFileSelectionService OpenFileSelectionService
        {
            get
            {
                if (_openFileSelectionService == null)
                {
                    _openFileSelectionService = new OpenFileDialogService();
                }
                return _openFileSelectionService;
            }
        }

        public IFileSelectionService SaveFileSelectionService
        {
            get
            {
                if (_saveFileSelectionService == null)
                {
                    _saveFileSelectionService = new SaveFileDialogService();
                }
                return _saveFileSelectionService;
            }
        }

        public IModalDialogService<GenerateConfigSettings> GenerateConfigDialogService
        {
            get
            {
                if (_generateConfigDialogService == null)
                {
                    _generateConfigDialogService = new GenerateConfigDialogService();
                }
                return _generateConfigDialogService;
            }
        }

        public IModalDialogService<SaveConfigSettings> SaveConfigDialogService
        {
            get
            {
                if (_saveConfigDialogService == null)
                {
                    _saveConfigDialogService = new SaveConfigDialogService();
                }
                return _saveConfigDialogService;
            }
        }

        public IModalDialogService ConfigSettingsDialogService
        {
            get
            {
                if (_configSettingsDialogService == null)
                {
                    _configSettingsDialogService = new ConfigSettingsDialogService();
                }
                return _configSettingsDialogService;
            }
        }

        public IModalDialogService MessageDialogService
        {
            get
            {
                if(_messageDialogService == null)
                {
                    _messageDialogService = new MessageDialogService();
                }
                return _messageDialogService;
            }
        }
    }
}

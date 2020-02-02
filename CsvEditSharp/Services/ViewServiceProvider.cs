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
        private IModalDialogService _okCancelDialogService;
        private IModalDialogService<string> _selectConfigurationDialogService;
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

        public IModalDialogService OkCancelDialogService
        {
            get
            {
                if(_okCancelDialogService == null)
                {
                    _okCancelDialogService = new OkCancelDialogService();
                }
                return _okCancelDialogService;
            }
        }

        public IModalDialogService<string> SelectConfigurationDialogService
        {
            get
            {
                if(_selectConfigurationDialogService == null)
                {
                    _selectConfigurationDialogService = new SelectConfigurationDialogService();
                }
                return _selectConfigurationDialogService;
            }
        }
    }
}

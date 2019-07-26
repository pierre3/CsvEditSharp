using CsvEditSharp.Models;
using Unity;

namespace CsvEditSharp.Services
{
    public class ViewServiceProvider : IViewServiceProvider
    {
        private IFileSelectionService _openFileSelectionService;
        private IFileSelectionService _saveFileSelectionService;
        private IModalDialogService<GenerateConfigSettings> _generateConfigDialogService;
        private IModalDialogService<SaveConfigSettings> _saveConfigDialogService;
        private IModalDialogService _configSettingsDialogService;

        private IUnityContainer _iocContainer;

        public ViewServiceProvider(IUnityContainer iocContainer)
        {
            _iocContainer = iocContainer;
        }

        public IFileSelectionService OpenFileSelectionService
        {
            get
            {
                if (_openFileSelectionService == null)
                {
                    _openFileSelectionService = _iocContainer.Resolve<OpenFileDialogService>();
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
                    _saveFileSelectionService = _iocContainer.Resolve<SaveFileDialogService>();
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
                    _generateConfigDialogService = _iocContainer.Resolve<GenerateConfigDialogService>();
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
                    _saveConfigDialogService = _iocContainer.Resolve<SaveConfigDialogService>();
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
                    _configSettingsDialogService = _iocContainer.Resolve<ConfigSettingsDialogService>();
                }
                return _configSettingsDialogService;
            }
        }
    }
}

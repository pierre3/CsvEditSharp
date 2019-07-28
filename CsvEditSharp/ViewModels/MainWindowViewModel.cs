using CsvEditSharp.Bindings;
using CsvEditSharp.Converters;
using CsvEditSharp.Csv;
using CsvEditSharp.Interfaces;
using CsvEditSharp.Models;
using ICSharpCode.AvalonEdit.Document;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace CsvEditSharp.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainViewModel
    {
        public readonly string CsvFileFilter = "CSV File|*.csv|Plain Text File|*.txt|All Files|*.*";

        #region fields 
        private ObservableCollection<object> _csvRows;
        private ObservableCollection<string> _errorMessages = new ObservableCollection<string>();
        private bool _hasErrorMessages = false;
        private IDocument _configurationDoc;
        private IDocument _queryDoc;
        private CsvEditSharpConfigurationHost _host;
        private int _selectedTab;

        private string _currentFilePath;
        private string _currentFileName;
        private string _currentConfigName;
        private string _selectedTemplate;

        private ICommand _deleteTemplateCommand;

        private IUnityContainer _iocContainer;
        #endregion 

        [Dependency] public IViewServiceProvider ViewServiceProvider { get; set; }

        public CsvEditSharpWorkspace Workspace { get; }

        public CsvEditSharpConfigurationHost Host => _host;

        public ObservableCollection<object> CsvRows
        {
            get { return _csvRows; }
            set
            {
                SetProperty(ref _csvRows, value);
                OnPropertyChanged("HasCsvRows");
            }
        }

        public bool HasCsvRows => (CsvRows?.Count ?? 0) > 0;

        public int SelectedTab
        {
            get { return _selectedTab; }
            set { SetProperty(ref _selectedTab, value); }
        }

        public string SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { SetProperty(ref _selectedTemplate, value); }
        }

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            set { SetProperty(ref _currentFilePath, value); }
        }

        public string CurrentFileName
        {
            get { return _currentFileName; }
            set { SetProperty(ref _currentFileName, value); }
        }

        public string CurrentConfigName
        {
            get { return _currentConfigName; }
            set
            {
                if (value == "") { value = "(Empty)"; }
                SetProperty(ref _currentConfigName, value);
            }
        }

        public ObservableCollection<string> ErrorMessages
        {
            get { return _errorMessages; }
            set { SetProperty(ref _errorMessages, value); }
        }

        public bool HasErrorMessages
        {
            get { return _hasErrorMessages; }
            set { SetProperty(ref _hasErrorMessages, value); }
        }

        public ObservableCollection<string> ConfigFileTemplates
        {
            get { return CsvConfigFileManager.Default.SettingsList; }
        }

        public IDocument ConfigurationDoc
        {
            get { return _configurationDoc; }
            set { SetProperty(ref _configurationDoc, value); }
        }

        public IDocument QueryDoc
        {
            get { return _queryDoc; }
            set { SetProperty(ref _queryDoc, value); }
        }

        public ICommand DeleteTemplateCommand
        {
            get
            {
                if (_deleteTemplateCommand == null)
                {
                    _deleteTemplateCommand = new DelegateCommand(_ => CsvConfigFileManager.Default.RemoveConfigFile(SelectedTemplate),
                        _ => !string.IsNullOrEmpty(SelectedTemplate));
                }
                return _deleteTemplateCommand;
            }
        }

        public MainWindowViewModel(IUnityContainer iocContainer)
        {
            _iocContainer = iocContainer;
            _errorMessages.CollectionChanged += (_, __) => HasErrorMessages = _errorMessages.Count > 0;

            _host = iocContainer.Resolve<CsvEditSharpConfigurationHost>();
            iocContainer.RegisterInstance<ICsvEditSharpConfigurationHost>(_host);
            iocContainer.RegisterInstance<IList<string>>(_errorMessages);

            Workspace = iocContainer.Resolve<CsvEditSharpWorkspace>();

            ConfigurationDoc = new TextDocument(StringTextSource.Empty);
            QueryDoc = new TextDocument(new StringTextSource("Query<FieldData>( records => records.Where(row => true));"));

            CurrentFilePath = string.Empty;
            CurrentFileName = "(Empty)";
            CurrentConfigName = "(Empty)";
            SelectedTemplate = ConfigFileTemplates.First();
            SelectedTab = 0;
        }

        public DataGridColumnValidationRule GetDataGridColumnValidation(string propertyName)
        {
            ColumnValidation columnValidaiton;
            if (_host.ColumnValidations.TryGetValue(propertyName, out columnValidaiton))
            {
                return new DataGridColumnValidationRule(columnValidaiton.Validation, columnValidaiton.ErrorMessage);
            }
            return null;
        }


        public async Task<IEnumerable<CompletionData>> GetCompletionListAsync(int position, string code)
        {
            return await Workspace.GetCompletionListAsync(position, code);
        }

        public DataGridColumnConverter GetDataGridColumnConverter(string propertyName)
        {
            var propertyMap = _host.ClassMapForWriting?.PropertyMaps?.FirstOrDefault(m => m.Data.Property.Name == propertyName);
            if (propertyMap == null) { return null; }

            return new DataGridColumnConverter(
                propertyMap.Data.Names[propertyMap.Data.NameIndex],
                propertyMap.Data.TypeConverter,
                propertyMap.Data.TypeConverterOptions);
        }

        protected override void OnDispose()
        {
            Workspace?.Dispose();
        }
    }
}

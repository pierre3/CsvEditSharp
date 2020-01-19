using CsvEditSharp.Bindings;
using CsvEditSharp.Models;
using CsvEditSharp.Services;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CsvEditSharp.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        private static readonly string CsvFileFilter = "CSV File|*.csv|Plain Text File|*.txt|All Files|*.*";
        private ObservableCollection<object> _csvRows;
        private ObservableCollection<string> _errorMessages = new ObservableCollection<string>();
        private bool _hasErrorMessages = false;
        private IDocument _configurationDoc;
        private IDocument _queryDoc;
        private CsvEditSharpConfigurationHost _host;
        private IViewServiceProvider _viewService;
        private int _selectedTab;

        private string _currentFilePath;
        private string _currentFileName;
        private string _currentConfigName;
        private string _selectedTemplate;

        private ICommand _readCsvCommand;
        private ICommand _queryCommand;
        private ICommand _writeCsvCommand;
        private ICommand _runConfigComannd;
        private ICommand _resetQueryCommand;
        private ICommand _saveConfigCommand;
        private ICommand _saveConfigAsCommand;
        private ICommand _configSettingsCommand;
        private ICommand _deleteTemplateCommand;

        private CsvEditSharpWorkspace Workspace { get; }

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

        public ObservableCollection<object> CsvRows
        {
            get { return _csvRows; }
            set { SetProperty(ref _csvRows, value); }
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

        public ICommand ReadCsvCommand
        {
            get
            {
                if (_readCsvCommand == null)
                {
                    _readCsvCommand = new DelegateCommand(_ => ReadCsvAsync());
                }
                return _readCsvCommand;
            }
        }

        public ICommand WriteCsvCommand
        {
            get
            {
                if (_writeCsvCommand == null)
                {
                    _writeCsvCommand = new DelegateCommand(_ => WriteCsv(), _ => (CsvRows?.Count ?? 0) > 0);
                }
                return _writeCsvCommand;
            }
        }

        public ICommand QueryCommand
        {
            get
            {
                if (_queryCommand == null)
                {
                    _queryCommand = new DelegateCommand(async _ => await ExecuteQueryAsync(), _ => Workspace.HasScriptState);
                }
                return _queryCommand;
            }
        }

        public ICommand ResetQueryCommand
        {
            get
            {
                if (_resetQueryCommand == null)
                {
                    _resetQueryCommand = new DelegateCommand(_ => ResetQuery(), _ => Workspace.HasScriptState);
                }
                return _resetQueryCommand;
            }
        }

        public ICommand RunConfigCommand
        {
            get
            {
                if (_runConfigComannd == null)
                {
                    _runConfigComannd = new DelegateCommand(async _ => await RunConfigurationAsync(), _ => CanExecuteRunConfigCommand());
                }
                return _runConfigComannd;
            }
        }

        public ICommand SaveConfigCommand
        {
            get
            {
                if (_saveConfigCommand == null)
                {
                    _saveConfigCommand = new DelegateCommand(_ => SaveConfigFile(), _ => !string.IsNullOrWhiteSpace(ConfigurationDoc.Text)
                        && File.Exists(CsvConfigFileManager.Default.CurrentConfigFilePath));
                }
                return _saveConfigCommand;
            }
        }

        public ICommand SaveConfigAsCommand
        {
            get
            {
                if (_saveConfigAsCommand == null)
                {
                    _saveConfigAsCommand = new DelegateCommand(_ => SaveConfigAs(), _ => !string.IsNullOrWhiteSpace(ConfigurationDoc.Text)
                        && File.Exists(CsvConfigFileManager.Default.CurrentConfigFilePath));
                }
                return _saveConfigAsCommand;
            }
        }

        public ICommand ConfigSettingsCommand
        {
            get
            {
                if (_configSettingsCommand == null)
                {
                    _configSettingsCommand = new DelegateCommand(_ => _viewService.ConfigSettingsDialogService.ShowModal(), _ => true);
                }
                return _configSettingsCommand;
            }
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

        public MainWindowViewModel(IViewServiceProvider viewServiceProvider)
        {
            _viewService = viewServiceProvider;
            _errorMessages.CollectionChanged += (_, __) => HasErrorMessages = _errorMessages.Count > 0;
            _host = new CsvEditSharpConfigurationHost();
            Workspace = new CsvEditSharpWorkspace(_host, _errorMessages);

            ConfigurationDoc = new TextDocument(StringTextSource.Empty);
            QueryDoc = new TextDocument(new StringTextSource("host.Query<FieldData>( records => records.Where(row => true).OrderBy(row => row) );"));

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

        private bool CanExecuteRunConfigCommand()
        {
            return (CurrentFilePath != null)
                && File.Exists(CurrentFilePath)
                && !string.IsNullOrWhiteSpace(ConfigurationDoc.Text);
        }

        private async void ReadCsvAsync()
        {
            //OpenFileDialog
            var openFileService = _viewService.OpenFileSelectionService;
            CurrentFilePath = openFileService.SelectFile("Select a CSV File", CsvFileFilter, null);
            if (!File.Exists(CurrentFilePath)) { return; }
            try
            {
                var configText = CsvConfigFileManager.Default.GetCsvConfigString(CurrentFilePath, _selectedTemplate);

                CurrentConfigName = Path.GetFileName(CsvConfigFileManager.Default.CurrentConfigFilePath);
                CurrentFileName = Path.GetFileName(CurrentFilePath);
                ConfigurationDoc.Text = configText;
            }
            catch(Exception e)
            {
                ErrorMessages.Add(e.ToString());
                return;
            }
            await RunConfigurationAsync();
        }

        private async Task RunConfigurationAsync()
        {
            _host.Reset();
            ErrorMessages.Clear();
            await Workspace.RunScriptAsync(ConfigurationDoc.Text);

            try
            {
                using (var stream = new FileStream(_currentFilePath, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(stream, _host.Encoding ?? Encoding.Default))
                {
                    _host.Read(reader);
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add(e.ToString());
            }

            CsvRows = new ObservableCollection<object>(_host.Records);
            SelectedTab = 0;
        }

        private void WriteCsv()
        {
            var saveFileService = _viewService.SaveFileSelectionService;
            var fileName = saveFileService.SelectFile("Save As..", CsvFileFilter, _currentFilePath);
            if (fileName == null) { return; }

            try
            {
                using (var writer = new StreamWriter(fileName, false, _host.Encoding))
                {
                    _host.Write(writer, CsvRows);
                }
                _viewService.MessageDialogService.ShowModal($"Saved to \"{fileName}\".", "CSV Data Saving");
            }
            catch (Exception e)
            {
                ErrorMessages.Add(e.ToString());
            }
        }

        private async Task ExecuteQueryAsync()
        {
            ErrorMessages.Clear();
            await Workspace.ContinueScriptAsync(QueryDoc.Text);
            try
            {
                _host.ExecuteQuery();
                CsvRows = new ObservableCollection<object>(_host.Records);
            }
            catch (Exception e)
            {
                ErrorMessages.Add(e.ToString());
            }
        }

        private void ResetQuery()
        {
            _host.ResetQuery();
            CsvRows = new ObservableCollection<object>(_host.Records);
        }

        private void SaveConfigFile()
        {
            CsvConfigFileManager.Default.SaveConfigFile(ConfigurationDoc.Text);
            _viewService.MessageDialogService.ShowModal($"Overwrite to \"{CsvConfigFileManager.Default.CurrentConfigFilePath}\".", "Configuration Data Saving");
        }

        private void SaveConfigAs()
        {
            try
            {
                var service = _viewService.SaveConfigDialogService;
                if (true == service.ShowModal())
                {
                    var fileName = string.Empty;
                    if (service.Result.IsTemplate)
                    {
                        fileName = CsvConfigFileManager.Default.MakeCurrentConfigFilePath(service.Result.TemplateName);
                    }
                    else
                    {
                        fileName = Path.Combine(Path.GetDirectoryName(CurrentFilePath), "Default.config.csx");
                    }
                    CurrentConfigName = Path.GetFileName(fileName);
                    CsvConfigFileManager.Default.CurrentConfigFilePath = fileName;
                    CsvConfigFileManager.Default.SaveConfigFile(ConfigurationDoc.Text);
                    _viewService.MessageDialogService.ShowModal($"Saved to \"{fileName}\".", "Configuration Data Saving");
                }
            }catch(Exception e)
            {
                ErrorMessages.Add(e.ToString());
            }
        }

        public async Task<IEnumerable<CompletionData>> GetCompletionListAsync(int position, string code)
        {
            return await Workspace.GetCompletionListAsync(position, code);
        }

        public DataGridColumnConverter GetDataGridColumnConverter(string propertyName)
        {
           
            var memberMap = _host.ClassMapForWriting?.MemberMaps?.FirstOrDefault(m => m.Data.Member.Name == propertyName);
            if (memberMap?.Data == null) { return null; }

            return new DataGridColumnConverter(memberMap.Data);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Workspace?.Dispose();
            }
        }

        ~MainWindowViewModel()
        {
            Dispose(false);
        }

    }
}

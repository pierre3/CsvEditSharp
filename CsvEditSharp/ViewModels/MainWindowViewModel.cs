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
        private static readonly string CsvFileFilter = "CSV File|*.csv|Plain Text File|*.txt|Tab Separated Values File|*.tsv|All Files|*.*";
        private ObservableCollection<object> _csvRows;
        private ObservableCollection<string> _errorMessages = new ObservableCollection<string>();
        private bool _hasErrorMessages = false;
        private IDocument _configurationDoc;
        private IDocument _queryDoc;
        private CsvEditSharpApi _host;
        private IViewServiceProvider _viewService;
        private int _selectedTab;

        private string _currentFilePath;
        private string _currentFileName;
        private string _currentConfigName;
        private string _startupFilePath;

        private ICommand _readCsvCommand;
        private ICommand _queryCommand;
        private ICommand _writeCsvCommand;
        private ICommand _runConfigComannd;
        private ICommand _resetQueryCommand;
        private ICommand _saveConfigCommand;
        private ICommand _saveConfigAsCommand;
        private ICommand _configSettingsCommand;

        private CsvEditSharpWorkspace Workspace { get; }


        public int SelectedTab
        {
            get { return _selectedTab; }
            set { SetProperty(ref _selectedTab, value); }
        }

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            set { SetProperty(ref _currentFilePath, value); }
        }

        public string CurrentFileName
        {
            get { return _currentFileName.Replace("_","__"); }
            set { SetProperty(ref _currentFileName, value); }
        }

        public string CurrentConfigName
        {
            get { return _currentConfigName.Replace("_", "__"); }
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
                    _saveConfigAsCommand = new DelegateCommand(_ => SaveConfigAs(), _ => !string.IsNullOrWhiteSpace(ConfigurationDoc.Text));
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


        public MainWindowViewModel(IViewServiceProvider viewServiceProvider,string startupFileName)
        {
            _viewService = viewServiceProvider;
            _errorMessages.CollectionChanged += (_, __) => HasErrorMessages = _errorMessages.Count > 0;
            _host = new CsvEditSharpApi();
            Workspace = new CsvEditSharpWorkspace(_host, _errorMessages);

            ConfigurationDoc = new TextDocument(StringTextSource.Empty);
            QueryDoc = new TextDocument(new StringTextSource("host.Query<FieldData>( records => records.Where(row => true).OrderBy(row => row) );"));

            CurrentFilePath = string.Empty;
            CurrentFileName = "(Empty)";
            CurrentConfigName = "(Empty)";
            SelectedTab = 0;
            _startupFilePath = startupFileName;
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
            var fileName = openFileService.SelectFile("Select a CSV File", CsvFileFilter, null);
            await ReadCsvAsync(fileName);
        }

        public async Task ReadStartupCsvAsync()
        {
            if (!string.IsNullOrEmpty(_startupFilePath))
            {
                await ReadCsvAsync(_startupFilePath);
            }
        }

        private async Task ReadCsvAsync(string fileName)
        {
            CurrentFilePath = fileName;
            if (!File.Exists(CurrentFilePath)) { return; }
            try
            {
                var configText = CsvConfigFileManager.Default.GetDefaultConfigString(CurrentFilePath);
                if (null == configText)
                {
                    var selectConfigDialog = _viewService.SelectConfigurationDialogService;
                    if (true != selectConfigDialog.ShowModal(CurrentFilePath))
                    {
                        return;
                    }
                    configText = selectConfigDialog.Result;
                }
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
            if (true == _viewService.OkCancelDialogService.ShowModal(
                $"Are you sure you want to overwrite the configuration file ? \"{CsvConfigFileManager.Default.CurrentConfigFilePath}\"", 
                "Configuration Data Saving"))
            {
                CsvConfigFileManager.Default.SaveConfigFile(ConfigurationDoc.Text);
            }
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
                        fileName = Path.Combine(Path.GetDirectoryName(CurrentFilePath), "_default.config.csx");
                    }
                    CurrentConfigName = Path.GetFileName(fileName);
                    CsvConfigFileManager.Default.CurrentConfigFilePath = fileName;
                    CsvConfigFileManager.Default.SaveConfigFile(ConfigurationDoc.Text);
                    CsvConfigFileManager.Default.GetConfigFiles();
                    _viewService.OkCancelDialogService.ShowModal($"Saved to \"{fileName}\".", "Configuration Data Saving");
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
            return new DataGridColumnConverter(_host.CsvConfiguration, memberMap.Data);
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

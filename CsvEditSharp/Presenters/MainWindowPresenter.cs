using Adventures.NetStandard.Common.Interfaces;
using CsvEditSharp.Bindings;
using CsvEditSharp.Csv;
using CsvEditSharp.Interfaces;
using CsvEditSharp.ViewModels;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Unity;

namespace CsvEditSharp.Presenters
{
    /// <summary>
    /// The Presenter has access to both the View and ViewModel - this allows us to keep
    /// both of them decoupled while giving us direct access to the View (a limitation of MVVM)
    /// </summary>
    public class MainWindowPresenter : IPresenter
    {
        private IMainWindow _mainWindow;
        private IMainViewModel _mainViewModel;
        private CompletionWindow completionWindow;

        [Dependency] public StartupEventArgs StartupArgs { get; set; }

        // With MVPVM there is tight coupling in the presenter, all other
        // components are loosely coupled (and reusable)
        public MainWindow MainWindow => (MainWindow)_mainWindow;

        public MainWindowViewModel MainViewModel => (MainWindowViewModel)_mainViewModel;

        public MainWindowPresenter(IMainWindow window, IMainViewModel viewModel)
        {
            _mainWindow = window;
            _mainViewModel = viewModel;

            MainWindow.DataContext = MainViewModel;
        }

        public IWindow Initialize(EventArgs e = null)
        {

            // Command subscriptions
            MainViewModel.ReadCsvCommand = new DelegateCommand(para => ReadCsvAsync(para));
            MainViewModel.RunConfigCommand = new DelegateCommand(para => RunConfigurationAsync());

            // Bind F5 key to execute command
            InputBinding f5 = new InputBinding(MainViewModel.QueryCommand, new KeyGesture(Key.F5)); MainWindow.InputBindings.Add(f5);
            InputBinding f6 = new InputBinding(MainViewModel.ResetQueryCommand, new KeyGesture(Key.F6)); MainWindow.InputBindings.Add(f6);


            // MainWindow event subscriptions
            MainWindow.configEdit.TextArea.TextEntered += TextArea_TextEntered;
            MainWindow.configEdit.TextArea.TextEntering += TextArea_TextEntering;

            MainWindow.queryEdit.TextArea.TextEntered += TextArea_TextEntered;
            MainWindow.queryEdit.TextArea.TextEntering += TextArea_TextEntering;

            MainWindow.grdMainData.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
            MainWindow.KeyDown += MainWindow_KeyDown;

            // When the view is loaded we'll invoke the ReadCsvCommand
            // in case the user double clicked a file 
            MainWindow.Loaded += (s, para) => MainViewModel.ReadCsvCommand.Execute(para);
            MainWindow.Closed += MainWindow_Closed;

            return (IWindow) MainWindow;
        }

        private async void ReadCsvAsync(object para)
        {
            string currentFilePath = null;

            // para will not be null if invoked by Loaded event
            if (para != null)
            {
                // If there is a parameter then load the file
                if (StartupArgs.Args.Length > 0)
                    currentFilePath = StartupArgs.Args[0];
                else
                    // Otherwise, we'll exit the load event
                    return;
            }

            var openFileService = MainViewModel.ViewService.OpenFileSelectionService;
            MainViewModel.CurrentFilePath = currentFilePath == null
                ? openFileService.SelectFile("Select a CSV File", MainViewModel.CsvFileFilter, null)
                : currentFilePath;

            if (!File.Exists(MainViewModel.CurrentFilePath)) { return; }

            var configText = CsvConfigFileManager.Default
                .GetCsvConfigString(MainViewModel.CurrentFilePath, MainViewModel.SelectedTemplate);

            MainViewModel.CurrentConfigName = Path.GetFileName(CsvConfigFileManager.Default.CurrentConfigFilePath);
            MainViewModel.CurrentFileName = Path.GetFileName(MainViewModel.CurrentFilePath);

            MainViewModel.ConfigurationDoc.Text = configText;


            // Let UI refresh before long running task
            await Dispatcher.Yield(DispatcherPriority.ApplicationIdle);

            RunConfigurationAsync();
        }

        #region MainWindow event handlers 
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            MainViewModel.Dispose();
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control))
            {
                ICSharpCode.AvalonEdit.TextEditor editor = null;
                if (MainWindow.configEdit.TextArea.IsFocused)
                {
                    editor = MainWindow.configEdit;
                }
                else if (MainWindow.queryEdit.TextArea.IsFocused)
                {
                    editor = MainWindow.queryEdit;
                }
                if (editor == null) { return; }
                e.Handled = true;
                ShowCompletionWindow(editor);
            }
        }

        private async void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (MainViewModel == null) { return; }
            // Open code completion after the user has pressed dot:
            if (e.Text == ".")
            {
                var textArea = sender as ICSharpCode.AvalonEdit.Editing.TextArea;
                ICSharpCode.AvalonEdit.TextEditor editor = null;
                if (MainWindow.configEdit.TextArea == textArea)
                {
                    editor = MainWindow.configEdit;
                }
                else if (MainWindow.queryEdit.TextArea == textArea)
                {
                    editor = MainWindow.queryEdit;
                }
                else { return; }

                await ShowCompletionWindow(editor);
            }
        }

        private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (MainViewModel == null) { return; }

            var converter = MainViewModel.GetDataGridColumnConverter(e.PropertyName);
            if (converter == null) { return; }

            if (!string.IsNullOrEmpty(converter.HeaderName))
            {
                e.Column.Header = converter.HeaderName;
            }

            var textColumn = e.Column as DataGridTextColumn;
            if (textColumn != null)
            {
                textColumn.EditingElementStyle = (Style)MainWindow.Resources["textColumnStyle"];
            }

            var binding = (e.Column as DataGridBoundColumn)?.Binding as Binding;
            if (binding != null)
            {
                binding.Converter = converter;
                var validationRule = MainViewModel.GetDataGridColumnValidation(e.PropertyName);
                if (validationRule != null)
                {
                    binding.ValidationRules.Add(validationRule);
                }
            }
        }
        #endregion

        #region Support methods

        public async Task ShowCompletionWindow(ICSharpCode.AvalonEdit.TextEditor editor)
        {
            completionWindow = new CompletionWindow(editor.TextArea);
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

            var position = editor.TextArea.Caret.Offset + 1;
            var code = editor.Document.Text + " ";
            if (editor == MainWindow.queryEdit)
            {
                position += MainWindow.configEdit.Document.TextLength + 1;
                code = MainWindow.configEdit.Document.Text + Environment.NewLine + code;
            }
            var completionItems = await MainViewModel.GetCompletionListAsync(position, code);
            foreach (var item in completionItems)
            {
                data.Add(item);
            }

            completionWindow.Show();
            completionWindow.Closed += delegate
            {
                completionWindow = null;
            };
        }
        #endregion 

        private void RunConfigurationAsync()
        {
            MainViewModel.Host.Reset();
            MainViewModel.ErrorMessages.Clear();


            MainViewModel.Workspace.RunScriptAsync(MainViewModel.ConfigurationDoc.Text);

            try
            {
                using (var stream = new FileStream(MainViewModel.CurrentFilePath, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(stream, MainViewModel.Host.Encoding ?? Encoding.Default))
                {
                    MainViewModel.Host.Read(reader);
                }
            }
            catch (Exception e)
            {
                MainViewModel.ErrorMessages.Add(e.ToString());
            }

            MainViewModel.CsvRows = new ObservableCollection<object>(MainViewModel.Host.Records);
            MainViewModel.SelectedTab = 0;
        }


    }
}

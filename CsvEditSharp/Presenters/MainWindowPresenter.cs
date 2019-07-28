﻿using Adventures.NetStandard.Common;
using Adventures.NetStandard.Common.Interfaces;
using CsvEditSharp.Commands;
using CsvEditSharp.Converters;
using CsvEditSharp.Csv;
using CsvEditSharp.Interfaces;
using CsvEditSharp.ViewModels;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Unity;

namespace CsvEditSharp.Presenters
{
    /// <summary>
    /// The Presenter has access to both the View and ViewModel - this allows us to keep
    /// both of them decoupled while giving us direct access to the View (a limitation of MVVM)
    /// </summary>
    public class MainWindowPresenter : PresenterBase
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

        public override IWindow Initialize(EventArgs e = null)
        {
            // MainWindow event subscriptions
            MainWindow.configEdit.TextArea.TextEntered += TextArea_TextEntered;
            MainWindow.configEdit.TextArea.TextEntering += TextArea_TextEntering;

            MainWindow.queryEdit.TextArea.TextEntered += TextArea_TextEntered;
            MainWindow.queryEdit.TextArea.TextEntering += TextArea_TextEntering;

            MainWindow.grdMainData.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
            MainWindow.KeyDown += MainWindow_KeyDown;

            // When the view is loaded we'll invoke the ReadCsvCommand in case the user 
            // double clicked a file, the null lets command now we were not a button click
            MainWindow.Loaded += (s, para) =>
            {
                InvokeCommand(ReadCsvCommand.CommandName, null);

                // Bind F5-F6 key to execute commands
                var queryCommand = IocContainer.Resolve<ICommand>(QueryCommand.CommandName);
                var resetQueryCommand = IocContainer.Resolve<ICommand>(ResetQueryCommand.CommandName);

                var f5 = new InputBinding(queryCommand, new KeyGesture(Key.F5)); MainWindow.InputBindings.Add(f5);
                var f6 = new InputBinding(resetQueryCommand, new KeyGesture(Key.F6)); MainWindow.InputBindings.Add(f6);
            };

            MainWindow.Closed += MainWindow_Closed;

            MainViewModel.ErrorMessages.CollectionChanged += (_, __) =>
                MainViewModel.HasErrorMessages = MainViewModel.ErrorMessages.Count > 0;

            MainViewModel.Host = IocContainer.Resolve<CsvEditSharpConfigurationHost>();
            IocContainer.RegisterInstance<ICsvEditSharpConfigurationHost>(MainViewModel.Host);
            IocContainer.RegisterInstance<IList<string>>(MainViewModel.ErrorMessages);

            MainViewModel.Workspace = IocContainer.Resolve<CsvEditSharpWorkspace>();

            MainViewModel.ConfigurationDoc = new TextDocument(StringTextSource.Empty);
            MainViewModel.QueryDoc = new TextDocument(new StringTextSource("Query<FieldData>( records => records.Where(row => true));"));

            MainViewModel.CurrentFilePath = string.Empty;
            MainViewModel.CurrentFileName = "(Empty)";
            MainViewModel.CurrentConfigName = "(Empty)";
            MainViewModel.SelectedTemplate = MainViewModel.ConfigFileTemplates.First();
            MainViewModel.SelectedTab = 0;

            return MainWindow;
        }

        #region MainWindow event handlers 
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            MainViewModel.Dispose();
        }

        private async void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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
                await ShowCompletionWindow(editor);
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


    }
}

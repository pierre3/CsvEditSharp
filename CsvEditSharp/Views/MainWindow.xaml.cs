using CsvEditSharp.ViewModels;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CsvEditSharp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private CompletionWindow completionWindow;

        private MainWindowViewModel VM { get { return DataContext as MainWindowViewModel; } }

        public MainWindow(MainWindowViewModel vm) : base()
        {
            InitializeComponent();
            DataContext = vm;

            configEdit.TextArea.TextEntered += TextArea_TextEntered;
            configEdit.TextArea.TextEntering += TextArea_TextEntering;
            queryEdit.TextArea.TextEntered += TextArea_TextEntered;
            queryEdit.TextArea.TextEntering += TextArea_TextEntering;
        }

        protected override void OnClosed(EventArgs e)
        {
            VM?.Dispose();
            base.OnClosed(e);
        }

        private async void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (VM == null) { return; }
            // Open code completion after the user has pressed dot:
            if (e.Text == ".")
            {
                var textArea = sender as ICSharpCode.AvalonEdit.Editing.TextArea;
                ICSharpCode.AvalonEdit.TextEditor editor = null;
                if (configEdit.TextArea == textArea)
                {
                    editor = configEdit;
                }
                else if (queryEdit.TextArea == textArea)
                {
                    editor = queryEdit;
                }
                else { return; }

                await ShowCompletionWindow(editor);
            }
        }

        protected override async void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Space && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control))
            {
                ICSharpCode.AvalonEdit.TextEditor editor = null;
                if (configEdit.TextArea.IsFocused)
                {
                    editor = configEdit;
                }
                else if (queryEdit.TextArea.IsFocused)
                {
                    editor = queryEdit;
                }
                if (editor == null) { return; }
                e.Handled = true;
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

        private async Task ShowCompletionWindow(ICSharpCode.AvalonEdit.TextEditor editor)
        {
            completionWindow = new CompletionWindow(editor.TextArea);
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

            var position = editor.TextArea.Caret.Offset + 1;
            var code = editor.Document.Text + " ";
            if (editor == queryEdit)
            {
                position += configEdit.Document.TextLength + 1;
                code = configEdit.Document.Text + Environment.NewLine + code;
            }
            var completionItems = await VM.GetCompletionListAsync(position, code);
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

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (VM == null) { return; }

            var converter = VM.GetDataGridColumnConverter(e.PropertyName);
            if (converter == null) { return; }

            if (!string.IsNullOrEmpty(converter.HeaderName))
            {
                e.Column.Header = converter.HeaderName;
            }

            var textColumn = e.Column as DataGridTextColumn;
            if (textColumn != null)
            {
                textColumn.EditingElementStyle = (Style)Resources["textColumnStyle"];
            }

            var binding = (e.Column as DataGridBoundColumn)?.Binding as Binding;
            if (binding != null)
            {
                binding.Converter = converter;
                var validationRule = VM.GetDataGridColumnValidation(e.PropertyName);
                if (validationRule != null)
                {
                    binding.ValidationRules.Add(validationRule);
                }
            }
        }

    }
}

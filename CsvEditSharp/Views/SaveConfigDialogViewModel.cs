using CsvEditSharp.Bindings;
using CsvEditSharp.Csv;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace CsvEditSharp.ViewModels
{
    public class SaveConfigDialogViewModel : ErrorNotificationBindableBase
    {
        private string _templateName;
        private bool _isTemplate = true;

        public ICommand ApplyCommand { get; }

        public string TemplateName
        {
            get { return _templateName; }
            set
            {
                SetProperty(ref _templateName, value);
                ValidateTemplateName();
            }
        }

        public bool IsTemplate
        {
            get { return _isTemplate; }
            set { SetProperty(ref _isTemplate, value); }
        }

        public SaveConfigDialogViewModel()
        {
            ApplyCommand = new DelegateCommand(_ => { }, _ => !string.IsNullOrWhiteSpace(TemplateName) && !HasErrors);
        }

        private void ValidateTemplateName()
        {
            ClearErrorInfo(nameof(TemplateName));
            if (TemplateName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
            {
                AddErrorInfo(nameof(TemplateName), "Enter a valid name as file name.");
            }
            else if (CsvConfigFileManager.Default.SettingsList.Contains(TemplateName))
            {
                AddErrorInfo(nameof(TemplateName), $"{TemplateName} is already exist.");
            }
        }

    }
}

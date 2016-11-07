using CsvEditSharp.Bindings;
using CsvEditSharp.Models;
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
            set { SetProperty(ref _templateName, value); }
        }
       
        public bool IsTemplate
        {
            get { return _isTemplate; }
            set { SetProperty(ref _isTemplate, value); }
        }

        public SaveConfigDialogViewModel()
        {
            ApplyCommand = new DelegateCommand(_ => { }, _ => !HasErrors);
        }
        
        private void ValidateTemplateName()
        {
            ClearErrorInfo(nameof(TemplateName));
            if (string.IsNullOrWhiteSpace(TemplateName))
            {
                AddErrorInfo(nameof(TemplateName), $"[{TemplateName}]: Enter a \"Name\" box");
            }
            else if (TemplateName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
            {
                AddErrorInfo(nameof(TemplateName), $"[{TemplateName}]: Enter a valid name as file name.");
            }
            else if (CsvConfigFileManager.Default.SettingsList.Contains(TemplateName))
            {
                AddErrorInfo(nameof(TemplateName), $"[{TemplateName}]: {TemplateName} is already exist.");
            }
        }
        
    }
}

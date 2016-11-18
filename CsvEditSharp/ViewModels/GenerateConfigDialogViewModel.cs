using CsvEditSharp.Bindings;
using CsvEditSharp.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CsvEditSharp.ViewModels
{
    public class GenerateConfigDialogViewModel : ErrorNotificationBindableBase
    {
        public GenerateConfigSettings Settings { get; private set; }

        public static IReadOnlyList<EncodingInfo> Encodings { get; }
            = Encoding.GetEncodings();

        private string _templateName = "NewCSVConfigTemplate";
        private int _targetEncodingIndex;
        private bool _hasHeaderRecord;

        public string TemplateName
        {
            get { return _templateName; }
            set
            {
                SetProperty(ref _templateName, value);
                ValidateTemplateName();
            }
        }
        
        public ICommand ApplyCommand { get; }

        public int TargetEncodingIndex
        {
            get { return _targetEncodingIndex; }
            set { SetProperty(ref _targetEncodingIndex, value); }
        }

        public bool HasHeaderRecord
        {
            get { return _hasHeaderRecord; }
            set { SetProperty(ref _hasHeaderRecord, value); }
        }

        public GenerateConfigDialogViewModel()
        {
            ApplyCommand = new DelegateCommand(_ => Apply(), _ => !string.IsNullOrWhiteSpace(TemplateName) && !HasErrors);
        
            TargetEncodingIndex = Encodings
                .Select((x, i) => new { Info = x, Index = i })
                .FirstOrDefault(a => a.Info.CodePage == Encoding.UTF8.CodePage)?.Index ?? 0;

            HasHeaderRecord = true;
        }

        private void Apply()
        {
            Settings = new GenerateConfigSettings
            {
                TemplateName = TemplateName,
                TargetFileEncoding = Encodings[TargetEncodingIndex].GetEncoding(),
                HasHeaderRecord = HasHeaderRecord
            };
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

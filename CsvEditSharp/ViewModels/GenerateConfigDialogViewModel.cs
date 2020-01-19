using CsvEditSharp.Bindings;
using CsvEditSharp.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
namespace CsvEditSharp.ViewModels
{
    public class GenerateConfigDialogViewModel : ErrorNotificationBindableBase
    {
        public GenerateConfigSettings Settings { get; private set; }

        public static IReadOnlyList<EncodingInfo> Encodings { get; }
            = Encoding.GetEncodings();

        public static IReadOnlyList<CultureInfo> CultureInfoList { get; }
            = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        
        private string _templateName = "NewCSVConfigTemplate";
        private int _targetEncodingIndex;
        private int _cultureInfoIndex;
        private bool _hasHeaderRecord;
        private bool _autoTypeDetection;

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

        public int CultureInfoIndex
        {
            get { return _cultureInfoIndex; }
            set { SetProperty(ref _cultureInfoIndex, value); }
        }

        public bool HasHeaderRecord
        {
            get { return _hasHeaderRecord; }
            set { SetProperty(ref _hasHeaderRecord, value); }
        }

        public bool AutoTypeDetection
        {
            get { return _autoTypeDetection; }
            set { SetProperty(ref _autoTypeDetection, value); }
        }

        public GenerateConfigDialogViewModel(string templateName)
        {
            if (!string.IsNullOrWhiteSpace(templateName))
            {
                TemplateName = templateName;
            }
            ApplyCommand = new DelegateCommand(_ => Apply(), _ => !string.IsNullOrWhiteSpace(TemplateName) && !HasErrors);
        
            TargetEncodingIndex = Encodings
                .Select((x, i) => new { Info = x, Index = i })
                .FirstOrDefault(a => a.Info.CodePage == Encoding.UTF8.CodePage)?.Index ?? 0;

            CultureInfoIndex = CultureInfoList
                .Select((x, i) => new { Info = x, Index = i })
                .FirstOrDefault(a => a.Info.LCID == CultureInfo.CurrentCulture.LCID)?.Index ?? 0;
            HasHeaderRecord = true;
            AutoTypeDetection = true;
        }

        private void Apply()
        {
            Settings = new GenerateConfigSettings
            {
                TemplateName = TemplateName,
                TargetFileEncoding = Encodings[TargetEncodingIndex].GetEncoding(),
                CaltureInfo = CultureInfoList[CultureInfoIndex],
                HasHeaderRecord = HasHeaderRecord,
                AutoTypeDetection = AutoTypeDetection
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
                AddErrorInfo(nameof(TemplateName), $"\"{TemplateName}\" is already exist.");
            }
        }
        
    }
}

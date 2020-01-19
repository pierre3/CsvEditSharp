using CsvEditSharp.Services;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CsvEditSharp.Models
{
    public class CsvConfigFileManager
    {
        static readonly string AutoGenerateTemplateName = "(Auto Generate)";

        public static CsvConfigFileManager Default { get; private set; }

        public string ConfigFileDirectory { get; private set; }

        private IModalDialogService<GenerateConfigSettings> _dialogService;

        public ObservableCollection<string> SettingsList { get; private set; } = new ObservableCollection<string>();

        public string CurrentConfigFilePath { get; set; } = string.Empty;

        public CsvConfigFileManager(IModalDialogService<GenerateConfigSettings> dialogService, string configFileDirectory = null)
        {
            _dialogService = dialogService;

            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Assembly.GetEntryAssembly().GetName().Name);

            if (string.IsNullOrWhiteSpace(configFileDirectory))
            {
                ConfigFileDirectory = Path.Combine(appData, "Config");
            }
            else
            {
                ConfigFileDirectory = configFileDirectory;
            }

            GetConfigFiles();
        }

        public static void InitializeDefault(IModalDialogService<GenerateConfigSettings> dialogService,
            string configFileDirectory = null)
        {
            Default = new CsvConfigFileManager(dialogService, configFileDirectory);
        }

        public void GetConfigFiles()
        {
            Directory.CreateDirectory(ConfigFileDirectory);
            var files = Directory.GetFiles(ConfigFileDirectory, "*.config.csx");
            SettingsList.Clear();

            var items = new[] { AutoGenerateTemplateName }
                .Concat(files.Select(x => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(x))));
            foreach (var item in items)
            {
                SettingsList.Add(item);
            }
        }

        public string GetCsvConfigString(string targetFilePath, string templateName)
        {
            //Read a default config file
            var defaultConfigPath = Path.Combine(Path.GetDirectoryName(targetFilePath), "Default.config.csx");
            if (File.Exists(defaultConfigPath))
            {
                var configText = File.ReadAllText(defaultConfigPath, Encoding.UTF8);
                CurrentConfigFilePath = defaultConfigPath;
                return configText;
            }

            //Read a config file from selected template.
            if (SettingsList.Skip(1).Contains(templateName))
            {
                var templateFilePath = Path.Combine(ConfigFileDirectory, templateName + ".config.csx");
                if (!File.Exists(templateFilePath)) { return $"Error! \"{templateFilePath}\" is not found."; }
                var configText = File.ReadAllText(templateFilePath, Encoding.UTF8);
                CurrentConfigFilePath = templateFilePath;
                return configText;
            }
            var defaultName = Path.GetFileNameWithoutExtension(targetFilePath);
            //Generate a new config template.
            if (true != _dialogService.ShowModal(defaultName)) { return ""; }

            var configFileSettings = _dialogService.Result;
            return GenerateCsvConfig(targetFilePath, configFileSettings);
        }

        public string MakeCurrentConfigFilePath(string templateName)
        {
            return Path.Combine(ConfigFileDirectory, templateName + ".config.csx");
        }

        public void SaveConfigFile(string configText)
        {
            File.WriteAllText(CurrentConfigFilePath, configText);
        }

        public bool CanRemove(string name)
        {
            return (name != AutoGenerateTemplateName) && SettingsList.Contains(name);
        }

        public void RemoveConfigFile(string name)
        {
            SettingsList.Remove(name);
            var path = MakeCurrentConfigFilePath(name);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public bool CanRename(string oldName, string newName)
        {
            return (oldName != AutoGenerateTemplateName)
                && (!string.IsNullOrWhiteSpace(newName))
                && SettingsList.Contains(oldName)
                && !SettingsList.Contains(newName);
        }

        public void RenameConfigFile(string oldName, string newName)
        {
            var oldPath = MakeCurrentConfigFilePath(oldName);
            var newPath = MakeCurrentConfigFilePath(newName);
            if (File.Exists(oldPath))
            {
                File.Move(oldPath, newPath);
            }
            var index = SettingsList.IndexOf(oldName);
            SettingsList.Insert(index, newName);
            SettingsList.Remove(oldName);
        }

        private string GenerateCsvConfigText(string targetFilePath, Encoding targetFileEncoding, CultureInfo cultureInfo, bool hasHeaders, bool autoTypeDetection)
        {
            using (var reader = new StreamReader(targetFilePath, targetFileEncoding))
            {
                var parser = new CsvHelper.CsvParser(reader);
                string[] headers = null;
                if (hasHeaders)
                {
                    headers = parser.Read();
                }
                var row = parser.Read();
                var config = new T4.ConfigurationTemplateGenerator(targetFileEncoding.WebName, cultureInfo, autoTypeDetection ,row, headers);

                return config.TransformText();
            }
        }

        private string GenerateCsvConfig(string targetFilePath, GenerateConfigSettings newSettings)
        {
            if (!File.Exists(targetFilePath)) { throw new FileNotFoundException("Target CSV file not exist.", targetFilePath); }

            //Save to template file.
            var templateFilePath = Path.Combine(ConfigFileDirectory, newSettings.TemplateName + ".config.csx");
            if (File.Exists(templateFilePath))
            {
                throw new InvalidOperationException($"Config File \"{templateFilePath}\" already exists.");
            }

            //Generate a CSV config template.
            var configText = GenerateCsvConfigText(targetFilePath, newSettings.TargetFileEncoding, newSettings.CaltureInfo, newSettings.HasHeaderRecord, newSettings.AutoTypeDetection);
            File.WriteAllText(templateFilePath, configText);

            CurrentConfigFilePath = templateFilePath;
            SettingsList.Add(newSettings.TemplateName);
            return configText;
        }
    }
}
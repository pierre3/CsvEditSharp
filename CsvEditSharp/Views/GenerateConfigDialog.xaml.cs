using CsvEditSharp.ViewModels;
using System.Windows;

namespace CsvEditSharp.Views
{
    /// <summary>
    /// CsvConfigFileSettingDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class GenerateConfigDialog : Window
    {
        public GenerateConfigDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

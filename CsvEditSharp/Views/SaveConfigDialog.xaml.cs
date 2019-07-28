using System.Windows;

namespace CsvEditSharp.Views
{
    /// <summary>
    /// SaveConfigDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SaveConfigDialog : Window
    {
        public SaveConfigDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

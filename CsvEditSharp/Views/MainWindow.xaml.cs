using Adventures.NetStandard.Common.Interfaces;
using CsvEditSharp.Interfaces;
using System.Windows;

namespace CsvEditSharp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow(IMainViewModel vm) : base()
        {
            InitializeComponent();
        }
    }
}

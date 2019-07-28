using System.Windows;
using Unity;

namespace CsvEditSharp.Extensions
{
    public static class ObjectExtension
    {
        public static bool? ShowDialog<T>(this IUnityContainer iocContainer,  object vm=null)
            where T: Window
        {
            T dialog = iocContainer.Resolve<T>();
            dialog.Owner = App.Current.MainWindow;
            dialog.DataContext = vm;
            var result = dialog.ShowDialog();
            return result;
        }

    }
}

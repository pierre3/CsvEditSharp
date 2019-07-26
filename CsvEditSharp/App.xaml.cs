using CsvEditSharp.Models;
using CsvEditSharp.Services;
using CsvEditSharp.ViewModels;
using Prism.Events;
using System.Windows;
using Unity;
using Unity.Lifetime;

namespace CsvEditSharp
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private IUnityContainer _iocContainer = new UnityContainer();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _iocContainer
                .RegisterType<IViewServiceProvider,ViewServiceProvider>(new ContainerControlledLifetimeManager())
                .RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager())
                .RegisterInstance<StartupEventArgs>(e);

            var viewService = _iocContainer.Resolve<IViewServiceProvider>();
            CsvConfigFileManager.InitializeDefault(viewService.GenerateConfigDialogService);
            var vm = _iocContainer.Resolve<MainWindowViewModel>();
            _iocContainer.RegisterInstance(vm);

            MainWindow = _iocContainer.Resolve<MainWindow>();
            MainWindow.Show();
        }

    }
}

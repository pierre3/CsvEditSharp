using CsvEditSharp.Ioc;
using CsvEditSharp.Models;
using CsvEditSharp.Presenters;
using CsvEditSharp.Services;
using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;
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
                .RegisterType<IViewServiceProvider, ViewServiceProvider>(Lifetime.Singleton)
                .RegisterType<IEventAggregator, EventAggregator>(Lifetime.Singleton)

                .RegisterType<IMainViewModel, MainWindowViewModel>(Lifetime.Singleton)
                .RegisterType<IMainWindow, MainWindow>()

                .RegisterInstance<StartupEventArgs>(e);

            var viewService = _iocContainer.Resolve<IViewServiceProvider>();
            CsvConfigFileManager.InitializeDefault(viewService.GenerateConfigDialogService);

            var presenter = _iocContainer.Resolve<MainWindowPresenter>();
            MainWindow = presenter.Initialize();
            MainWindow.Show();
        }
    }
}

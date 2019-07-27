using Adventures.NetStandard.Common.Interfaces;
using Adventures.NetStandard.Common.Ioc;
using CsvEditSharp.Commands;
using CsvEditSharp.Csv;
using CsvEditSharp.Interfaces;
using CsvEditSharp.Presenters;
using CsvEditSharp.Services;
using CsvEditSharp.ViewModels;
using Prism.Events;
using System.Windows;
using System.Windows.Input;
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
        public IPresenter _presenter;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _iocContainer
                .RegisterType<IViewServiceProvider, ViewServiceProvider>(Lifetime.Singleton)
                .RegisterType<IEventAggregator, EventAggregator>(Lifetime.Singleton)

                .RegisterType<IMainViewModel, MainWindowViewModel>(Lifetime.Singleton)
                .RegisterType<IMainWindow, MainWindow>()

                .RegisterType<ICommand, ReadCsvCommand>(ReadCsvCommand.CommandName)
                .RegisterType<ICommand, RunConfigCommand>(RunConfigCommand.CommandName)

                .RegisterInstance<StartupEventArgs>(e);

            var viewService = _iocContainer.Resolve<IViewServiceProvider>();
            CsvConfigFileManager.InitializeDefault(viewService.GenerateConfigDialogService);

            _presenter = _iocContainer.Resolve<MainWindowPresenter>();
            MainWindow = (Window) _presenter.Initialize();
            MainWindow.Show();
        }
    }
}

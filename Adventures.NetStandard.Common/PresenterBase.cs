using Adventures.NetStandard.Common.Interfaces;
using CsvEditSharp.Events;
using Prism.Events;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace Adventures.NetStandard.Common
{
    public class PresenterBase : IPresenter
    {
        private IEventAggregator eventAggregator;
        private IUnityContainer _iocContainer;

        [Dependency]
        public IUnityContainer IocContainer
        {
            get { return _iocContainer; }
            set { _iocContainer = value; OnIocContainerSet(); }
        }

        private void OnIocContainerSet()
        {
            
        }

        [Dependency]
        public IEventAggregator EventAggregator
        {
            get { return eventAggregator; }
            set { eventAggregator = value; OnEventAggregatorset(); }
        }

        private void OnEventAggregatorset()
        {
            EventAggregator.GetEvent<CommandButtonEvent>().Subscribe(ButtonEventHandler);
        }

        private void ButtonEventHandler(CommandButtonEventArgs e)
        {
            switch (e.CommandType)
            {
                case Enums.CommandType.Execute:
                    ButtonExecute(e);                        
                    break;

                case Enums.CommandType.CanExecute:
                    e.CanExecute = CanButtonExecute(e);
                    break;

                case Enums.CommandType.NotDefined:
                default: break;
            }
        }

        private void ButtonExecute(CommandButtonEventArgs e)
        {
            var button = e.Button as Button;
            var command = IocContainer.Resolve<ICommand>(button.Name);
            command.Execute(e);
        }

        protected virtual bool CanButtonExecute(CommandButtonEventArgs e)
        {
            var button = e.Button as Button;
            var command = IocContainer.Resolve<ICommand>(button.Name);
            return command.CanExecute(e);
        }

        public virtual IWindow Initialize(EventArgs e = null)
        {
            return null;
        }

        public void InvokeCommand(string commandName, object para)
        {
            var runConfigCmd = IocContainer.Resolve<ICommand>(commandName);
            runConfigCmd.Execute(para);
        }
    }
}

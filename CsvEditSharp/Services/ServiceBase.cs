using CsvEditSharp.Interfaces;
using System;
using System.Windows;
using Unity;

namespace CsvEditSharp.Services
{


    public class ServiceBase : IModalDialogService
    {
        [Dependency] public IUnityContainer IocContainer { get; set; }

        [Dependency] public StartupEventArgs StartupArgs { get; set; }

        public virtual bool? ShowModal()
        {
            throw new NotImplementedException();
        }
    }

    public class ServiceBase<T> : IModalDialogService<T>
    {
        [Dependency] public IUnityContainer IocContainer { get; set; }

        [Dependency] public StartupEventArgs StartupArgs { get; set; }

        public T Result { get; set; }

        public virtual bool? ShowModal()
        {
            throw new NotImplementedException();
        }
    }
}

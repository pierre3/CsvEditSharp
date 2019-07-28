using CsvEditSharp.Extensions;
using CsvEditSharp.Interfaces;
using CsvEditSharp.Models;
using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;
using Unity;

namespace CsvEditSharp.Services
{
    public class SaveConfigDialogService : IModalDialogService<SaveConfigSettings>
    {
        [Dependency] public IUnityContainer IocContainer { get; set; }

        public SaveConfigSettings Result { get; private set; }
        public bool? ShowModal() { 

            var vm = new SaveConfigDialogViewModel();

            var result = IocContainer.ShowDialog<SaveConfigDialog>(vm);
            if (result == true)
            {
                Result = new SaveConfigSettings
                {
                    IsTemplate = vm.IsTemplate,
                    TemplateName = vm.TemplateName
                };
            }
            return result;
        }
    }
}

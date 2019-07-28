using CsvEditSharp.Extensions;
using CsvEditSharp.Models;
using CsvEditSharp.ViewModels;
using CsvEditSharp.Views;

namespace CsvEditSharp.Services
{
    public class SaveConfigDialogService : ServiceBase<SaveConfigSettings>
    {
        public override bool? ShowModal() { 

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

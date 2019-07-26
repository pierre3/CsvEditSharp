using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CsvEditSharp.ViewModels
{
    public interface IMainViewModel : IViewModel
    {
        ICommand QueryCommand { get;  }
        ICommand ResetQueryCommand { get; }
        ICommand ReadCsvCommand { get; }
    }
}

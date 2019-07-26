using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvEditSharp.Views
{
    public interface IMainView
    {
        object DataContext { get; set; }
    }
}

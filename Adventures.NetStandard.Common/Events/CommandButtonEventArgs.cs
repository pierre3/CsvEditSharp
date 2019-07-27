using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvEditSharp.Events
{
    public class CommandButtonEventArgs : EventArgs
    {
        public object Button { get; internal set; }
        public bool CanExecute { get; internal set; }
    }
}

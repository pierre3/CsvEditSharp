using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CsvEditSharp.Events
{
    public class CommandButtonEventArgs : EventArgs
    {
        public Button Button { get; internal set; }
        public bool CanExecute { get; internal set; }
    }
}

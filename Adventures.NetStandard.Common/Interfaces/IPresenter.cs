using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Adventures.NetStandard.Common.Interfaces
{
    public interface IPresenter
    {
        IWindow Initialize(EventArgs e = null);
    }
}

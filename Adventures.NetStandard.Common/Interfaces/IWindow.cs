using System;
using System.Collections.Generic;
using System.Text;

namespace Adventures.NetStandard.Common.Interfaces
{
    public interface IWindow
    {
        object DataContext { get; set; }
        void Show();

    }
}

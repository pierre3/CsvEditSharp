using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvEditSharp.Services;

namespace CsvEditSharp.Models
{
    public interface IConfigFileManager
    {
        void InitializeDefault();
    }
}

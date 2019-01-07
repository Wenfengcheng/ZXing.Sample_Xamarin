using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    public interface IDeviceService
    {
        Task<string> ScanAsync();
    }
}

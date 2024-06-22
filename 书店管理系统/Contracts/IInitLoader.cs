using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.Contracts
{
    internal interface IInitLoader
    {
        event Action? Loaded;
        Task InitAsync(object? parameter = null);
    }
}

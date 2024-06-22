using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Contracts;

namespace 书店管理系统.Services
{
    internal sealed class InitLoader : IInitLoader
    {
        public event Action? Loaded;

        public async Task InitAsync(object? parameter = null)
        {
            await Task.Delay(2000);
            Loaded?.Invoke();
            Loaded = null;
        }
    }
}

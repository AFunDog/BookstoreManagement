using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.Contracts
{
    internal interface IActivationService
    {
        Task LaunchingActivateAsync(IProgress<bool>? progress = null, object? args = null);
        Task LaunchedActivateAsync(IProgress<bool>? progress = null, object? args = null);
    }
}

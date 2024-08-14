using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.Core.Structs
{
    public readonly record struct LoadingProgress(double Progress, string Message);
}

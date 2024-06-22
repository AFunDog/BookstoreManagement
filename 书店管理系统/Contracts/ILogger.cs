using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.Contracts
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogError(string message);
    }
}

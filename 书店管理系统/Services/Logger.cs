using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Contracts;

namespace 书店管理系统.Services
{
    internal class Logger : ILogger
    {
        public void LogError(string message)
        {
            Debug.WriteLine($"Error : {message}");
        }

        public void LogInfo(string message)
        {
            Debug.WriteLine($"Info : {message}");
        }
    }
}

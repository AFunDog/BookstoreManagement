using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Core.Structs;

namespace 书店管理系统.Core.Extensions
{
    internal static class ActionResultExtension
    {
        public static ActionResult Log(this ActionResult result)
        {
            return result;
        }
    }
}

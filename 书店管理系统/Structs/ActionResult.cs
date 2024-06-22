using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.Structs
{
    public enum ResultType
    {
        OK,Warning,Error
    }

    public readonly record struct ActionResult(ResultType ResultType,string Message)
    {

    }
}

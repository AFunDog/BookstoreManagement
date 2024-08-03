using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.Core.Structs
{
    public enum LoginType
    {
        User,
        Admin
    }

    public record LoginInfo(LoginType LoginType, int Id) { }
}

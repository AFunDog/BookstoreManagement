using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.Structs
{
    public readonly record struct UserData(
        int Id,string Name,string Password,string Phone,string Address,string Email,
        DateTime CreateTime,DateTime UpdateTime,decimal Account
        )
    {
    }
}

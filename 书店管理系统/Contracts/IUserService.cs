using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Structs;

namespace 书店管理系统.Contracts
{
    


    public interface IUserService
    {
        event Action<UserData>? UserDataChanged;
        IEnumerable<UserData> UserDatas { get; }

        ActionResult AddUser(UserData user);
        ActionResult RemoveUser(UserData user);
        ActionResult UpdateUser(UserData user);
        ActionResult CheckUserLoginInfo(string userName, string password);
    }
}

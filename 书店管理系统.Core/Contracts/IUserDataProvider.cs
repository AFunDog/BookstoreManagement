using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Contracts
{
    public interface IUserDataProvider
    {
        IReadOnlyCollection<UserData> UserDatas { get; }
        ActionResult LoadUserDatas();
        ValueTask<ActionResult> LoadUserDatasAsync();
        ActionResult SaveUserDatas();
        ValueTask<ActionResult> SaveUserDatasAsync();
        ActionResult TryAddUserData(UserData userData);
        ActionResult TryRemoveUserData(int userId);
        ActionResult TryGetUserData(int userId, out UserData? userData);
    }
}

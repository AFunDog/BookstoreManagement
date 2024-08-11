using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Contracts
{
    public interface IUserService
    {
        event Action<UserData>? UserDataChanged;
        IReadOnlyCollection<UserData> UserDatas { get; }
        UserData? LoginUser { get; }
        ActionResult RegisterUser(UserData user);
        ActionResult LogoutUser(UserData user);
        Task<ActionResult> TryLoginAsync(string userName, string password, CancellationToken cancellationToken = default);
        Task<ActionResult> SaveUserDatasAsync(CancellationToken cancellationToken = default);
    }
}

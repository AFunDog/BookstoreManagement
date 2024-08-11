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
        Task<ActionResult> LoadUserDatasAsync(CancellationToken cancellationToken = default);
        Task<ActionResult> SaveUserDatasAsync(CancellationToken cancellationToken = default);
        Task<ActionResult> TryAddUserDataAsync(UserData userData, CancellationToken cancellationToken = default);
        Task<ActionResult> TryRemoveUserDataAsync(int userId, CancellationToken cancellationToken = default);
        Task<(UserData? userData, ActionResult result)> TryGetUserDataAsync(int userId, CancellationToken cancellationToken = default);
    }
}

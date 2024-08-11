using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDataProvider _userDataProvider;
        private int _loginUserId = -1;

        public event Action<UserData>? UserDataChanged;

        public IReadOnlyCollection<UserData> UserDatas => _userDataProvider.UserDatas;
        public UserData? LoginUser =>
            _userDataProvider.TryGetUserData(_loginUserId, out var data).ResultType == ResultType.Success ? data : null;

        public UserService(IUserDataProvider dataProvider)
        {
            _userDataProvider = dataProvider;
        }

        public ActionResult RegisterUser(UserData user)
        {
            if (UserDatas.FirstOrDefault(x => x.Id == user.Id || x.Name == user.Name) is not null)
            {
                return ActionResult.Error("用户已存在").TryReportResult("尝试添加用户", null, LibrarySystemManager.Logger);
            }
            if (user.Id == -1)
            {
                user.Id = UserDatas.Count != 0 ? UserDatas.Max(x => x.Id) + 1 : 0;
            }
            if (!user.IsUserDataValid)
            {
                return ActionResult.Error("用户数据无效").TryReportResult("尝试添加用户", null, LibrarySystemManager.Logger);
            }
            return _userDataProvider.TryAddUserData(user).TryReportResult("尝试添加用户", null, LibrarySystemManager.Logger);
        }

        public ActionResult LogoutUser(UserData user)
        {
            return _userDataProvider.TryRemoveUserData(user.Id).TryReportResult("尝试注销用户", null, LibrarySystemManager.Logger);
        }

        public async Task<ActionResult> TryLoginAsync(string userName, string password, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            var find = UserDatas.FirstOrDefault(u =>
                (u.Name == userName || u.Phone == userName || u.Email == userName || u.Id.ToString() == userName) && u.Password == password
            );
            if (find is not null)
            {
                _loginUserId = find.Id;
                return ActionResult.Success();
            }
            return new(ResultType.Error, "找不到对应的用户");
        }

        public async Task<ActionResult> SaveUserDatasAsync(CancellationToken cancellationToken = default)
        {
            return await _userDataProvider.SaveUserDatasAsync();
        }
    }
}

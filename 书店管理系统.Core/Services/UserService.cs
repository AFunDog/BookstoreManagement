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

        public event Action<UserData>? UserDataChanged;

        public IReadOnlyCollection<UserData> UserDatas => _userDataProvider.UserDatas;

        public UserService(IUserDataProvider dataProvider)
        {
            _userDataProvider = dataProvider;
        }

        public ActionResult AddUser(UserData user)
        {
            if (UserDatas.FirstOrDefault(x => x.Id == user.Id || x.Name == user.Name) is not null)
            {
                return new ActionResult(ResultType.Error, "用户已存在");
            }
            if (user.Id == -1)
            {
                user.Id = UserDatas.Count != 0 ? UserDatas.Max(x => x.Id) + 1 : 0;
            }
            if (!user.IsUserDataValid)
            {
                return new ActionResult(ResultType.Error, "用户数据无效");
            }
            return _userDataProvider.TryAddUserData(user);
        }

        public ActionResult RemoveUser(UserData user)
        {
            return _userDataProvider.TryRemoveUserData(user.Id);
        }

        public ActionResult CheckUserLoginInfo(string userName, string password)
        {
            var find = UserDatas.Where(u =>
                (u.Name == userName || u.Phone == userName || u.Email == userName || u.Id.ToString() == userName) && u.Password == password
            );
            if (find.Any())
            {
                return new(ResultType.OK, string.Empty);
            }
            return new(ResultType.Error, "找不到对应的用户");
        }
    }
}

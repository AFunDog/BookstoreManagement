using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Contracts;
using 书店管理系统.Structs;

namespace 书店管理系统.Services
{
    internal class UserService : IUserService
    {

        private readonly IUserDataProvider _userDataProvider;

        public event Action<UserData>? UserDataChanged;

        public IEnumerable<UserData> UserDatas => _userDataProvider.GetAllUserDatas();



        public UserService(IUserDataProvider dataProvider)
        {
            _userDataProvider = dataProvider;
        }

        public ActionResult AddUser(UserData user)
        {
            throw new NotImplementedException();
        }

        public ActionResult RemoveUser(UserData user)
        {
            throw new NotImplementedException();
        }

        public ActionResult UpdateUser(UserData user)
        {
            throw new NotImplementedException();
        }

        public ActionResult CheckUserLoginInfo(string userName, string password)
        {
            var find = UserDatas.Where(u => (u.Name == userName || u.Phone == userName || u.Email == userName || u.Id.ToString() == userName) && u.Password == password);
            if (find.Any())
            {
                return new(ResultType.OK, string.Empty);
            }
            return new(ResultType.Error, "找不到对应的用户");
        }
    }
}

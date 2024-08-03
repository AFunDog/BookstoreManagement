using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Services
{
    public sealed class SQLUserDataProvider : IUserDataProvider
    {
        public ICollection<UserData> UserDatas => throw new NotImplementedException();

        IReadOnlyCollection<UserData> IUserDataProvider.UserDatas => throw new NotImplementedException();

        public ValueTask<ActionResult> LoadUserDatasAsync()
        {
            throw new NotImplementedException();
        }

        public ActionResult LoadUserDatas()
        {
            throw new NotImplementedException();
        }

        public ActionResult SaveUserDatas()
        {
            throw new NotImplementedException();
        }

        public ValueTask<ActionResult> SaveUserDatasAsync()
        {
            throw new NotImplementedException();
        }

        public ActionResult TryAddUserData(UserData userData)
        {
            throw new NotImplementedException();
        }

        public ActionResult TryGetUserData(int userID, out UserData? userData)
        {
            throw new NotImplementedException();
        }

        public ActionResult TryRemoveUserData(int userId)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Contracts;
using 书店管理系统.Structs;

namespace 书店管理系统.Services
{
    internal sealed class LocalUserDataProvider : IUserDataProvider
    {
        public IEnumerable<UserData> GetAllUserDatas()
        {
            return [new UserData(0, "zengkun", "159241", "15336567166", "宁波", "1489782679@qq.com", DateTime.Now, DateTime.Now, 0)];
        }
        public bool TryAddUser(UserData userData)
        {
            throw new NotImplementedException();
        }

        public bool TryGetUserData(int userID, [NotNullWhen(true)] out UserData userData)
        {
            throw new NotImplementedException();
        }

        public bool TryRemoveUser(int userID)
        {
            throw new NotImplementedException();
        }

        public bool TryUpdateUser(UserData userData)
        {
            throw new NotImplementedException();
        }


    }
}

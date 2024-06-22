using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Structs;

namespace 书店管理系统.Contracts
{
    internal interface IUserDataProvider
    {
        IEnumerable<UserData> GetAllUserDatas();

        bool TryAddUser(UserData userData);

        bool TryRemoveUser(int userID);

        bool TryUpdateUser(UserData userData);

        bool TryGetUserData(int userID, [NotNullWhen(true)] out UserData userData);

    }
}

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Services
{
    public sealed class LocalUserDataProvider : IUserDataProvider
    {
        private const string filePath = ".userDatas";

        private readonly ObservableCollection<UserData> _userDatas = [];

        //[
        //    new(0, "zengkun", "159241", Gender.Male, "15336567166", "宁波", "1489782679@qq.com", DateTime.Now, DateTime.Now, 0, new("zh-cn"))
        //];

        public IReadOnlyCollection<UserData> UserDatas => _userDatas;

        public ActionResult LoadUserDatas()
        {
            if (!File.Exists(filePath))
                return new(ResultType.Error, $"文件 {filePath} 不存在");
            foreach (var data in MessagePackSerializer.Deserialize<IEnumerable<UserData>>(File.ReadAllBytes(filePath)))
            {
                _userDatas.Add(data);
            }
            return new(ResultType.OK, string.Empty);
        }

        public async ValueTask<ActionResult> LoadUserDatasAsync()
        {
            if (!File.Exists(filePath))
                return new(ResultType.Error, $"文件 {filePath} 不存在");
            using FileStream fileStream = File.OpenRead(filePath);
            foreach (var data in await MessagePackSerializer.DeserializeAsync<IEnumerable<UserData>>(fileStream))
            {
                _userDatas.Add(data);
            }
            return new(ResultType.OK, string.Empty);
        }

        public ActionResult SaveUserDatas()
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
            File.WriteAllBytes(filePath, MessagePackSerializer.Serialize(_userDatas));
            return new(ResultType.OK, string.Empty);
        }

        public async ValueTask<ActionResult> SaveUserDatasAsync()
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
            using FileStream fileStream = File.OpenWrite(filePath);
            await MessagePackSerializer.SerializeAsync(fileStream, _userDatas);
            return new(ResultType.OK, string.Empty);
        }

        public ActionResult TryAddUserData(UserData userData)
        {
            _userDatas.Add(userData);
            return new(ResultType.OK, string.Empty);
        }

        public ActionResult TryGetUserData(int userId, out UserData? userData)
        {
            userData = _userDatas.FirstOrDefault(x => x.Id == userId);
            if (userData is not null)
            {
                return new(ResultType.OK, string.Empty);
            }
            else
            {
                return new ActionResult(ResultType.Error, $"{userId} 不存在");
            }
        }

        public ActionResult TryRemoveUserData(int userId)
        {
            if (_userDatas.FirstOrDefault(x => x.Id == userId) is UserData userData)
            {
                _userDatas.Remove(userData);
                return new ActionResult(ResultType.OK, string.Empty);
            }
            else
            {
                return new ActionResult(ResultType.Error, $"{userId} 不存在");
            }
        }
    }
}

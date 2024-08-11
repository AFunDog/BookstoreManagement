using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Serilog;
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

        public async Task<ActionResult> LoadUserDatasAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
                return ActionResult
                    .Error($"文件 {filePath} 不存在")
                    .TryReportResult($"读取用户数据错误 文件 {filePath} 不存在", null, LibrarySystemManager.Logger);

            using FileStream fileStream = File.OpenRead(filePath);
            foreach (var data in await MessagePackSerializer.DeserializeAsync<IEnumerable<UserData>>(fileStream))
            {
                _userDatas.Add(data);
            }
            return ActionResult.Success().TryReportResult("加载用户数据完成", null, LibrarySystemManager.Logger);
        }

        public async Task<ActionResult> SaveUserDatasAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
            using FileStream fileStream = File.OpenWrite(filePath);
            await MessagePackSerializer.SerializeAsync(fileStream, _userDatas);
            return ActionResult.Success().TryReportResult("保存用户数据完成", null, LibrarySystemManager.Logger);
        }

        public async Task<ActionResult> TryAddUserDataAsync(UserData userData, CancellationToken cancellationToken = default)
        {
            _userDatas.Add(userData);
            return ActionResult.Success();
        }

        public async Task<(UserData? userData, ActionResult result)> TryGetUserDataAsync(
            int userId,
            CancellationToken cancellationToken = default
        )
        {
            var userData = _userDatas.FirstOrDefault(x => x.Id == userId);
            return userData is not null ? new(userData, ActionResult.Success()) : new(null, ActionResult.Error($"{userId} 不存在"));
        }

        public async Task<ActionResult> TryRemoveUserDataAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (_userDatas.FirstOrDefault(x => x.Id == userId) is UserData userData)
            {
                _userDatas.Remove(userData);
                return new ActionResult(ResultType.Success, string.Empty);
            }
            else
            {
                return new ActionResult(ResultType.Error, $"{userId} 不存在");
            }
        }
    }
}

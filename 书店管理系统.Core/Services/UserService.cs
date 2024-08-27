using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.VisualStudio.Threading;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Services
{
    internal sealed class UserService(IDataProvider<UserData> userDataProvider) : IUserService
    {
        private ObservableCollection<UserData>? _userDatas = null;
        public ObservableCollection<UserData> InternalUserDatas => _userDatas ?? throw new InvalidOperationException("还未加载用户数据，读取用户数据失败");
        public IReadOnlyCollection<IReadOnlyUserData> UserDatas =>
            IsAdminState ? InternalUserDatas : throw new InvalidOperationException("非管理员权限无法访问其他用户数据");
        public UserData? LoginUser { get; private set; }

        public bool IsAdminState { get; private set; }

        public async Task<ActionResult> StartLoadUserDataAsync(CancellationToken cancellationToken = default)
        {
            await userDataProvider.LoadDataAsync(
                new Progress<ObservableCollection<UserData>>(datas => _userDatas = datas),
                cancellationToken
            );
            return ActionResult.Success("用户数据加载完毕").Log();
        }

        public async Task<ActionResult> StartSaveUserDataAsync(CancellationToken cancellationToken = default)
        {
            await userDataProvider.SaveDataAsync(cancellationToken);
            return ActionResult.Success("用户数据保存完毕").Log();
        }

        public async Task<ActionResult> LoginAsAdminAsync(string password, CancellationToken cancellationToken = default)
        {
            if (IsAdminState)
                return ActionResult.Warning("已经是管理员身份").Log();
            if (LoginUser is not null)
                return ActionResult.Warning("已经登录，要切换管理员身份请先退出登录").Log();
            if (password == "admin")
            {
                IsAdminState = true;
                return ActionResult.Success("管理员登录成功").Log();
            }
            else
            {
                return ActionResult.Error("管理员密码错误").Log();
            }
        }

        public async Task<ActionResult> LoginAsUserAsync(string userName, string password, CancellationToken cancellationToken = default)
        {
            if (IsAdminState)
                return ActionResult.Warning("已经是管理员身份").Log();
            if (LoginUser is not null)
                return ActionResult.Warning("已经登录，要切换用户身份请先退出登录").Log();
            if (
                InternalUserDatas.FirstOrDefault(u =>
                    u.Password == password && (u.Name == userName || u.Phone == userName || u.Email == userName)
                )
                is UserData loginUser
            )
            {
                LoginUser = loginUser;
                return ActionResult.Success($"用户 {LoginUser.Name} 登录成功").Log();
            }
            else
            {
                return ActionResult.Error("用户名或密码错误").Log();
            }
        }

        public async Task<ActionResult> LogoutAsync(CancellationToken cancellationToken = default)
        {
            if (IsAdminState)
            {
                IsAdminState = false;
                return ActionResult.Success("管理员退出登录成功").Log();
            }
            else if (LoginUser is not null)
            {
                var loginUser = LoginUser;
                LoginUser = null;
                return ActionResult.Success($"用户 {loginUser.Name} 退出登录成功").Log();
            }
            return ActionResult.Error("还未登录，请先登录再注销").Log();
        }

        public async Task<ActionResult> RegisterUserAsync(
            string name,
            string password,
            Gender gender = Gender.Unknown,
            string phone = "",
            string address = "",
            string email = "",
            CancellationToken cancellationToken = default
        )
        {
            if (InternalUserDatas.Any(u => u.Name == name))
                return ActionResult.Error("用户名已存在").Log();
            UserData registerData =
                new(AllocateUserId(), name, password, gender, phone, address, email, DateTimeOffset.Now, DateTimeOffset.Now, 0);
            if (!registerData.IsValid)
                return ActionResult.Error("新用户数据无效").Log();
            InternalUserDatas.Add(registerData);
            return ActionResult.Success($"用户 {registerData.Id} {registerData.Name} 注册成功").Log();
        }

        public async Task<ActionResult> RemoveUserAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!IsAdminState && LoginUser is null)
                return ActionResult.Error("未登录 没有权限执行操作").Log();
            if (!IsAdminState && LoginUser is not null && id != LoginUser.Id)
                return ActionResult.Error("不是管理员无法删除其他用户，没有权限").Log();

            if (InternalUserDatas.FirstOrDefault(u => u.Id == id) is UserData target)
            {
                if (InternalUserDatas.Remove(target))
                {
                    return ActionResult.Success($"用户 {id} 已被成功移除").Log();
                }
                else
                {
                    return ActionResult.Error($"用户 {id} 移除失败 错误原因未知").Log();
                }
            }
            else
            {
                return ActionResult.Error($"Id为{id}用户不存在，请检查Id是否无误").Log();
            }
        }

        public async Task<ActionResult> EditUserBasicDataAsync(
            int id,
            string? newName = null,
            string? newpassword = null,
            Gender? newGender = null,
            string? newPhone = null,
            string? newAddress = null,
            string? newEmail = null,
            CancellationToken cancellationToken = default
        )
        {
            if (LoginUser is null && !IsAdminState)
                return ActionResult.Error("未登录 没有权限执行操作").Log();

            if (InternalUserDatas.FirstOrDefault(u => u.Id == id) is UserData oldData)
            {
                if (IsAdminState || LoginUser is not null && LoginUser.Id == id)
                {
                    var newData = oldData.Adapt<UserData>();

                    if (newName is not null)
                        newData.Name = newName;
                    if (newpassword is not null)
                        newData.Password = newpassword;
                    if (newGender is not null)
                        newData.Gender = newGender.Value;
                    if (newPhone is not null)
                        newData.Phone = newPhone;
                    if (newAddress is not null)
                        newData.Address = newAddress;
                    if (newEmail is not null)
                        newData.Email = newEmail;
                    if (!newData.IsValid)
                        return ActionResult.Error("新用户数据无效").Log();
                    newData.Adapt(oldData);
                    oldData.UpdateTime = DateTimeOffset.Now;
                    return ActionResult.Success($"用户 {id} 的基本数据修改完毕").Log();
                }
                else
                {
                    return ActionResult.Error("无法修改其他用户的数据，权限不足").Log();
                }
            }
            else
            {
                return ActionResult.Error($"找不到Id为{id}用户数据").Log();
            }
        }

        private int AllocateUserId() => InternalUserDatas.Any() ? InternalUserDatas.Max(u => u.Id) + 1 : 1;
    }
}

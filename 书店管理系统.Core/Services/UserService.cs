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

        public async Task StartLoadUserDataAsync(CancellationToken cancellationToken = default)
        {
            await userDataProvider.LoadDataAsync(
                new Progress<ObservableCollection<UserData>>(datas => _userDatas = datas),
                cancellationToken
            );
            LibrarySystemManager.Instance.Logger.Debug("用户数据加载完毕");
        }

        public async Task StartSaveUserDataAsync(CancellationToken cancellationToken = default)
        {
            await userDataProvider.SaveDataAsync(cancellationToken);
            LibrarySystemManager.Instance.Logger.Debug("用户数据保存完毕");
        }

        public async Task LoginAsAdminAsync(string password, CancellationToken cancellationToken = default)
        {
            if (IsAdminState)
                throw new InvalidOperationException("已经是管理员身份");
            if (LoginUser is not null)
                throw new InvalidOperationException("已经登录，要切换管理员身份请先退出登录");
            if (password == "admin")
            {
                IsAdminState = true;
            }
            else
            {
                throw new KeyNotFoundException("管理员密码错误");
            }
        }

        public async Task LoginAsUserAsync(string userName, string password, CancellationToken cancellationToken = default)
        {
            if (IsAdminState)
                throw new InvalidOperationException("已经是管理员身份");
            if (LoginUser is not null)
                throw new InvalidOperationException("已经登录，要切换用户身份请先退出登录");
            if (
                InternalUserDatas.FirstOrDefault(u =>
                    u.Password == password && (u.Name == userName || u.Phone == userName || u.Email == userName)
                )
                is UserData loginUser
            )
            {
                LoginUser = loginUser;
            }
            else
            {
                throw new KeyNotFoundException("用户名或密码错误");
            }
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            if (!IsAdminState && LoginUser is null)
                throw new InvalidOperationException("还未登录，请先登录再注销");
            IsAdminState = false;
            LoginUser = null;
        }

        public async Task RegisterUserAsync(
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
                throw new ArgumentException("用户名已存在");
            UserData registerData = new(AllocateUserId(), name, password, gender, phone, address, email, DateTime.Now, DateTime.Now, 0);
            if (!registerData.IsValid)
                throw new ArgumentException("新用户数据无效");
            InternalUserDatas.Add(registerData);
        }

        public async Task RemoveUserAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!IsAdminState && LoginUser is null)
                throw new InvalidOperationException("未登录 没有权限执行操作");
            if (!IsAdminState && LoginUser is not null && id != LoginUser.Id)
                throw new InvalidOperationException("不是管理员无法删除其他用户，没有权限");

            if (InternalUserDatas.Remove(InternalUserDatas.First(u => u.Id == id)))
            {
                return;
            }
            else
            {
                throw new ArgumentException($"Id为{id}用户不存在，请检查Id是否无误");
            }
        }

        public async Task EditUserBasicDataAsync(
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
                throw new InvalidOperationException("未登录 没有权限执行操作");

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
                        throw new ArgumentException("新用户数据无效");
                    newData.Adapt(oldData);
                }
                else
                {
                    throw new InvalidOperationException("无法修改其他用户的数据，权限不足");
                }

                oldData.UpdateTime = DateTime.Now;
            }
            else
            {
                throw new ArgumentException($"找不到Id为{id}用户数据");
            }
        }

        private int AllocateUserId() => InternalUserDatas.Any() ? InternalUserDatas.Max(u => u.Id) + 1 : 1;
    }
}

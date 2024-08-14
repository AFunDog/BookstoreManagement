using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Contracts
{
    public interface IUserService
    {
        /// <summary>
        /// 所有用户的数据
        /// </summary>
        /// <remarks>
        /// 内部属性，交由内部服务使用
        /// </remarks>
        internal ObservableCollection<UserData> InternalUserDatas { get; }

        /// <summary>
        /// 所有用户的数据
        /// </summary>
        /// <remarks>
        /// 只有处于管理员状态才能查看的数据
        /// </remarks>
        IReadOnlyCollection<IReadOnlyUserData> UserDatas { get; }

        /// <summary>
        /// 登录的用户信息
        /// </summary>
        /// <remarks>
        /// 若未登录或处于管理员状态则为 null
        /// </remarks>
        UserData? LoginUser { get; }

        /// <summary>
        /// 是否处于管理员登录状态
        /// </summary>
        bool IsAdminState { get; }

        /// <summary>
        /// 开始加载用户数据任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartLoadUserDataAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 开始保存用户数据任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartSaveUserDataAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 以用户身份登录
        /// </summary>
        /// <param name="userName">用户名 / 用户Id / 手机号 / 邮箱</param>
        /// <param name="password">用户密码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task LoginAsUserAsync(string userName, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// 以管理员身份登录
        /// </summary>
        /// <param name="password">管理员密码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task LoginAsAdminAsync(string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task LogoutAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <param name="name">注册用户名</param>
        /// <param name="password">密码</param>
        /// <param name="gender">性别</param>
        /// <param name="phone">手机号码</param>
        /// <param name="address">地址</param>
        /// <param name="email">邮箱</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RegisterUserAsync(
            string name,
            string password,
            Gender gender = Gender.Unknown,
            string phone = "",
            string address = "",
            string email = "",
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <remarks>
        /// 只有管理员和本用户才能删除用户信息
        /// </remarks>
        /// <param name="id">用户Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveUserAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 修改用户的基本信息
        /// </summary>
        /// <remarks>
        /// 此操作只能在有一定权限的情况下修改
        /// <list type="bullet">
        /// <item>
        /// 在用户模式下，只能修改自己的基础信息
        /// </item>
        /// <item>
        /// 在管理员模式下，可以修改任意用户的基础信息
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="id">用户Id</param>
        /// <param name="newName">新用户名</param>
        /// <param name="newpassword">新密码</param>
        /// <param name="newGender">新性别</param>
        /// <param name="newPhone">新手机号</param>
        /// <param name="newAddress">新地址</param>
        /// <param name="newEmail">新邮箱</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task EditUserBasicDataAsync(
            int id,
            string? newName = null,
            string? newpassword = null,
            Gender? newGender = null,
            string? newPhone = null,
            string? newAddress = null,
            string? newEmail = null,
            CancellationToken cancellationToken = default
        );
    }
}

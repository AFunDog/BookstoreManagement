using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Services
{
    internal sealed class DealService(
        IDataProvider<BookDealData> bookDealDataProvider,
        IDataProvider<RechargeDealData> rechargeDealDataProvider,
        IUserService userService
    ) : IDealService
    {
        ObservableCollection<BookDealData>? _bookDealDatas;
        ObservableCollection<RechargeDealData>? _rechargeDealDatas;
        internal ObservableCollection<BookDealData> InternalBookDealDatas =>
            _bookDealDatas ?? throw new InvalidOperationException("还未加载交易数据，读取交易数据失败");
        internal ObservableCollection<RechargeDealData> InternalRechargeDealDatas =>
            _rechargeDealDatas ?? throw new InvalidOperationException("还未加载充值数据，读取充值数据失败");
        public IReadOnlyCollection<IReadOnlyBookDealData> BookDealDatas =>
            userService.IsAdminState
                ? InternalBookDealDatas
                : userService.LoginUser is not null
                    ? new ObservableCollection<BookDealData>(InternalBookDealDatas.Where(d => d.Uid == userService.LoginUser.Id))
                    : throw new InvalidOperationException("未登录无法查看书籍交易数据，请先登录");
        public IReadOnlyCollection<IReadOnlyRechargeDealData> RechargeDealDatas =>
            userService.IsAdminState
                ? InternalRechargeDealDatas
                : userService.LoginUser is not null
                    ? new ObservableCollection<RechargeDealData>(InternalRechargeDealDatas.Where(d => d.Uid == userService.LoginUser.Id))
                    : throw new InvalidOperationException("未登录无法查看书籍交易数据，请先登录");

        public async Task StartLoadDealDataAsync(CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(
                bookDealDataProvider.LoadDataAsync(
                    new Progress<ObservableCollection<BookDealData>>(d => _bookDealDatas = d),
                    cancellationToken
                ),
                rechargeDealDataProvider.LoadDataAsync(
                    new Progress<ObservableCollection<RechargeDealData>>(d => _rechargeDealDatas = d),
                    cancellationToken
                )
            );

            LibrarySystemManager.Instance.Logger.Debug("交易数据加载完毕");
        }

        public async Task StartSaveDealDataAsync(CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(
                bookDealDataProvider.SaveDataAsync(cancellationToken),
                rechargeDealDataProvider.SaveDataAsync(cancellationToken)
            );
            LibrarySystemManager.Instance.Logger.Debug("交易数据保存完毕");
        }

        async Task IDealService.AddNewBookDealAsync(int uid, long ISBN, decimal price, int amount, CancellationToken cancellationToken)
        {
            InternalBookDealDatas.Add(new(AllocateBookDealId(), DateTime.Now, uid, ISBN, price, amount));
        }

        public async Task RemoveBookDealAsync(int id, CancellationToken cancellationToken)
        {
            if (!userService.IsAdminState)
                throw new InvalidOperationException("非管理员无法删除交易数据");
            InternalBookDealDatas.Remove(InternalBookDealDatas.First(d => d.Id == id));
        }

        public async Task AddNewRechargeDealAsync(int uid, decimal addMoney, CancellationToken cancellationToken = default)
        {
            if (userService.LoginUser is null && !userService.IsAdminState)
                throw new InvalidOperationException("还未登录，无法进行充值操作");
            if (!userService.InternalUserDatas.Any(u => u.Id == uid))
                throw new ArgumentException("无效的用户ID");
            InternalRechargeDealDatas.Add(new(AllocateRechargeDealId(), DateTime.Now, default, uid, addMoney, false));
        }

        public async Task PassRechargeDealAsync(int id, CancellationToken cancellationToken)
        {
            if (!userService.IsAdminState)
                throw new InvalidOperationException("只有管理员才能审批通过充值交易数据");
            var rechargeDeal = InternalRechargeDealDatas.First(d => d.Id == id);
            UserData targetUser = (UserData)userService.UserDatas.First(u => u.Id == rechargeDeal.Uid);
            targetUser.Account += rechargeDeal.AddMoney;
            rechargeDeal.DealPassTime = DateTime.Now;
            rechargeDeal.IsPass = true;
        }

        public async Task RemoveRechargeDealAsync(int id, CancellationToken cancellationToken)
        {
            if (!userService.IsAdminState)
                throw new InvalidOperationException("非管理员无法删除充值数据");
            InternalRechargeDealDatas.Remove(InternalRechargeDealDatas.First(d => d.Id == id));
        }

        private int AllocateBookDealId() => InternalBookDealDatas.Any() ? InternalBookDealDatas.Max(d => d.Id) + 1 : 1;

        private int AllocateRechargeDealId() => InternalRechargeDealDatas.Any() ? InternalRechargeDealDatas.Max(d => d.Id) + 1 : 1;
    }
}

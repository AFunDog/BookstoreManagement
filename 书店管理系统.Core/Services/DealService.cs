using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Core.Structs;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Extensions;
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

        public async Task<ActionResult> StartLoadDealDataAsync(CancellationToken cancellationToken = default)
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

            return ActionResult.Success("交易数据加载完毕").Log();
        }

        public async Task<ActionResult> StartSaveDealDataAsync(CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(
                bookDealDataProvider.SaveDataAsync(cancellationToken),
                rechargeDealDataProvider.SaveDataAsync(cancellationToken)
            );
            return ActionResult.Success("交易数据保存完毕").Log();
        }

        async Task<ActionResult> IDealService.AddNewBookDealAsync(
            int uid,
            long ISBN,
            decimal price,
            int amount,
            CancellationToken cancellationToken
        )
        {
            var boolDeal = new BookDealData(AllocateBookDealId(), DateTimeOffset.Now, uid, ISBN, price, amount);
            InternalBookDealDatas.Add(boolDeal);
            return ActionResult.Success($"Id为 {boolDeal.Id} 的交易数据添加成功").Log();
        }

        public async Task<ActionResult> RemoveBookDealAsync(int id, CancellationToken cancellationToken)
        {
            if (!userService.IsAdminState)
                return ActionResult.Error("非管理员无法删除交易数据").Log();
            if (InternalBookDealDatas.FirstOrDefault(d => d.Id == id) is BookDealData bookDeal)
            {
                InternalBookDealDatas.Remove(bookDeal);
                return ActionResult.Success($"Id为 {bookDeal.Id} 的交易数据删除完毕").Log();
            }
            else
            {
                return ActionResult.Error($"Id为 {id} 的交易数据不存在").Log();
            }
        }

        public async Task<ActionResult> AddNewRechargeDealAsync(int uid, decimal addMoney, CancellationToken cancellationToken = default)
        {
            if (userService.LoginUser is null && !userService.IsAdminState)
                return ActionResult.Error("还未登录，无法进行充值操作").Log();
            if (!userService.InternalUserDatas.Any(u => u.Id == uid))
                return ActionResult.Error($"Id为 {uid} 的用户不存在").Log();
            var rechargeDeal = new RechargeDealData(AllocateRechargeDealId(), DateTimeOffset.Now, default, uid, addMoney, false);
            InternalRechargeDealDatas.Add(rechargeDeal);
            return ActionResult.Success($"Id为 {rechargeDeal.Id} 的充值数据添加成功").Log();
        }

        public async Task<ActionResult> PassRechargeDealAsync(int id, CancellationToken cancellationToken)
        {
            if (!userService.IsAdminState)
                return ActionResult.Error("只有管理员才能审批通过充值交易数据").Log();
            if (InternalRechargeDealDatas.FirstOrDefault(d => d.Id == id) is RechargeDealData rechargeDeal)
            {
                if (userService.UserDatas.FirstOrDefault(u => u.Id == rechargeDeal.Uid) is UserData targetUser)
                {
                    targetUser.Account += rechargeDeal.AddMoney;
                    rechargeDeal.DealPassTime = DateTimeOffset.Now;
                    rechargeDeal.IsPass = true;
                    return ActionResult.Success($"Id为 {id} 的充值交易数据审批通过").Log();
                }
                else
                {
                    return ActionResult.Error($"Id为 {rechargeDeal.Uid} 的用户不存在").Log();
                }
            }
            else
            {
                return ActionResult.Error($"Id为 {id} 的充值交易数据不存在").Log();
            }
        }

        public async Task<ActionResult> RemoveRechargeDealAsync(int id, CancellationToken cancellationToken)
        {
            if (!userService.IsAdminState)
                return ActionResult.Error("非管理员无法删除充值数据").Log();
            if (InternalRechargeDealDatas.FirstOrDefault(d => d.Id == id) is RechargeDealData rechargeDeal)
            {
                InternalRechargeDealDatas.Remove(rechargeDeal);
                return ActionResult.Success($"Id为 {rechargeDeal.Id} 的充值交易数据删除完毕").Log();
            }
            else
            {
                return ActionResult.Error($"Id为 {id} 的充值交易数据不存在").Log();
            }
        }

        private int AllocateBookDealId() => InternalBookDealDatas.Any() ? InternalBookDealDatas.Max(d => d.Id) + 1 : 1;

        private int AllocateRechargeDealId() => InternalRechargeDealDatas.Any() ? InternalRechargeDealDatas.Max(d => d.Id) + 1 : 1;
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Services
{
    internal sealed class BookService(IDataProvider<BookData> bookDataProvider, IUserService userService, IDealService dealService)
        : IBookService
    {
        private ObservableCollection<BookData>? _bookDatas;
        private ObservableCollection<BookData> InternalBookDatas => _bookDatas ?? throw new InvalidOperationException("还未加载书籍数据，读取书籍数据失败");
        public IReadOnlyCollection<IReadOnlyBookData> BookDatas =>
            (userService.LoginUser is not null || userService.IsAdminState)
                ? InternalBookDatas
                : throw new InvalidOperationException("未登录无法查看书籍信息，请先登录");

        public async Task<ActionResult> StartLoadBookDataAsync(CancellationToken cancellationToken = default)
        {
            await bookDataProvider.LoadDataAsync(new Progress<ObservableCollection<BookData>>(d => _bookDatas = d), cancellationToken);
            return ActionResult.Success("书籍数据记载完毕").Log();
        }

        public async Task<ActionResult> StartSaveBookDataAsync(CancellationToken cancellationToken = default)
        {
            await bookDataProvider.SaveDataAsync(cancellationToken);
            return ActionResult.Success("书籍数据保存完毕").Log();
        }

        public async Task<ActionResult> AddBookAsync(BookData bookData, CancellationToken cancellationToken = default)
        {
            if (!bookData.IsValid)
                return ActionResult.Error("书籍数据无效").Log();
            if (!userService.IsAdminState)
                return ActionResult.Error("只有管理员才能上架新书籍").Log();
            InternalBookDatas.Add(bookData);
            return ActionResult.Success("书籍上架成功").Log();
        }

        public async Task<ActionResult> BuyBookAsync(long ISBN, int count, CancellationToken cancellationToken = default)
        {
            if (userService.LoginUser is null)
                return ActionResult.Error("请以用户模式购买书籍").Log();
            if (InternalBookDatas.FirstOrDefault(b => b.ISBN == ISBN) is BookData bookData && bookData.Amount >= count)
            {
                if (userService.LoginUser.Account >= count * bookData.Price)
                {
                    decimal orginalAccount = userService.LoginUser.Account;

                    userService.LoginUser.Account -= count * bookData.Price;
                    bookData.Amount -= count;
                    var res = await dealService.AddNewBookDealAsync(
                        userService.LoginUser.Id,
                        bookData.ISBN,
                        bookData.Price,
                        count,
                        cancellationToken
                    );
                    if (res.IsSucceed)
                    {
                        return ActionResult.Success($"{ISBN} 书籍购买成功 单价 {bookData.Price} 数量 {count} 总花费 {count * bookData.Price}").Log();
                    }
                    else
                    {
                        userService.LoginUser.Account = orginalAccount;
                        bookData.Amount += count;
                        return ActionResult.Error($"购买书籍出现错误 {res.Message}").Log();
                    }
                }
                else
                {
                    return ActionResult.Error("当前余额不足，请充值").Log();
                }
            }
            else
            {
                return ActionResult.Warning("该书籍库存不足，或不存在，如有需要请通知管理员补货").Log();
            }
        }

        public async Task<ActionResult> ChangeBookPriceAsync(long ISBN, decimal newPrice, CancellationToken cancellationToken = default)
        {
            // TODO 添加记录书籍价格变动的功能
            if (!userService.IsAdminState)
                return ActionResult.Error("只有管理员才能修改书籍价格").Log();
            if (newPrice < 0)
                return ActionResult.Error("价格不能为负数").Log();
            BookData bookData = InternalBookDatas.First(b => b.ISBN == ISBN);
            var orginalPrice = bookData.Price;
            bookData.Price = newPrice;
            return ActionResult.Success($"修改书籍价格成功 原价格 {orginalPrice} 新价格 {newPrice}").Log();
        }

        public async Task<ActionResult> EditBookDataAsync(
            long ISBN,
            string? newBookName = null,
            string? newAuthor = null,
            string? newPublisher = null,
            DateTimeOffset? newPublicationDate = null,
            string[]? newCategory = null,
            string? newDescription = null,
            CancellationToken cancellationToken = default
        )
        {
            if (!userService.IsAdminState)
                return ActionResult.Error("只有管理员才能修改书籍基本信息").Log();
            BookData orginalData = InternalBookDatas.First(b => b.ISBN == ISBN);
            BookData newData = orginalData.Adapt<BookData>();
            newData.BookName = newBookName ?? newData.BookName;
            newData.Author = newAuthor ?? newData.Author;
            newData.Publisher = newPublisher ?? newData.Publisher;
            newData.PublicationDate = newPublicationDate ?? newData.PublicationDate;
            newData.Category = newCategory ?? newData.Category;
            newData.Description = newDescription ?? newData.Description;
            if (!newData.IsValid)
                return ActionResult.Error("新书籍信息无效").Log();
            newData.Adapt(orginalData);
            return ActionResult.Success("修改书籍信息成功").Log();
        }

        public async Task<ActionResult> RemoveBookAsync(long ISBN, CancellationToken cancellationToken = default)
        {
            if (!userService.IsAdminState)
                return ActionResult.Error("只有管理员才能下架书籍").Log();
            InternalBookDatas.Remove(InternalBookDatas.First(b => b.ISBN == ISBN));
            return ActionResult.Success($"下架 {ISBN} 书籍成功").Log();
        }

        public async Task<ActionResult> SupplyBookAsync(long ISBN, int count, CancellationToken cancellationToken = default)
        {
            if (!userService.IsAdminState)
                return ActionResult.Error("只有管理员才能补充书籍数量").Log();
            var data = InternalBookDatas.First(b => b.ISBN == ISBN);
            if (data.Amount + count < 0)
                return ActionResult.Error($"下架数量超过库存量 下架数量 {-count} 库存数量 {data.Amount}").Log();
            data.Amount += count;
            return ActionResult.Success($"成功{(count >= 0 ? "补充" : "下架")} {ISBN} 书籍 {count} 本 现有库存数量 {data.Amount}").Log();
        }
    }
}

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

        public async Task StartLoadBookDataAsync(CancellationToken cancellationToken = default)
        {
            await bookDataProvider.LoadDataAsync(new Progress<ObservableCollection<BookData>>(d => _bookDatas = d), cancellationToken);
            LibrarySystemManager.Instance.Logger.Debug("书籍数据记载完毕");
        }

        public async Task StartSaveBookDataAsync(CancellationToken cancellationToken = default)
        {
            await bookDataProvider.SaveDataAsync(cancellationToken);
            LibrarySystemManager.Instance.Logger.Debug("书籍数据保存完毕");
        }

        public async Task AddBookAsync(BookData bookData, CancellationToken cancellationToken = default)
        {
            if (!bookData.IsValid)
                throw new ArgumentException("书籍数据无效");
            if (!userService.IsAdminState)
                throw new InvalidOperationException("只有管理员才能上架新书籍");
            InternalBookDatas.Add(bookData);
        }

        public async Task BuyBookAsync(long ISBN, int count, CancellationToken cancellationToken = default)
        {
            if (userService.LoginUser is null)
                throw new InvalidOperationException("请以用户模式购买书籍");
            if (InternalBookDatas.First(b => b.ISBN == ISBN) is BookData bookData && bookData.Amount >= count)
            {
                if (userService.LoginUser.Account >= count * bookData.Price)
                {
                    decimal orginalAccount = userService.LoginUser.Account;
                    try
                    {
                        userService.LoginUser.Account -= count * bookData.Price;
                        bookData.Amount -= count;
                        await dealService.AddNewBookDealAsync(
                            userService.LoginUser.Id,
                            bookData.ISBN,
                            bookData.Price,
                            count,
                            cancellationToken
                        );
                    }
                    catch
                    {
                        userService.LoginUser.Account = orginalAccount;
                        bookData.Amount += count;
                        throw;
                    }
                }
                else
                {
                    throw new InvalidOperationException("当前余额不足，请充值");
                }
            }
            else
            {
                throw new InvalidOperationException("该书籍库存不足，如有需要请通知管理员补货");
            }
        }

        public async Task ChangeBookPriceAsync(long ISBN, decimal newPrice, CancellationToken cancellationToken = default)
        {
            // TODO 添加记录书籍价格变动的功能
            if (!userService.IsAdminState)
                throw new InvalidOperationException("只有管理员才能修改书籍价格");
            if (newPrice < 0)
                throw new ArgumentException("价格不能为负数");
            BookData bookData = InternalBookDatas.First(b => b.ISBN == ISBN);
            bookData.Price = newPrice;
        }

        public async Task EditBookDataAsync(
            long ISBN,
            string? newBookName = null,
            string? newAuthor = null,
            string? newPublisher = null,
            DateTime? newPublicationDate = null,
            string[]? newCategory = null,
            string? newDescription = null,
            CancellationToken cancellationToken = default
        )
        {
            if (!userService.IsAdminState)
                throw new InvalidOperationException("只有管理员才能修改书籍基本信息");
            BookData orginalData = InternalBookDatas.First(b => b.ISBN == ISBN);
            BookData newData = orginalData.Adapt<BookData>();
            newData.BookName = newBookName ?? newData.BookName;
            newData.Author = newAuthor ?? newData.Author;
            newData.Publisher = newPublisher ?? newData.Publisher;
            newData.PublicationDate = newPublicationDate ?? newData.PublicationDate;
            newData.Category = newCategory ?? newData.Category;
            newData.Description = newDescription ?? newData.Description;
            if (!newData.IsValid)
                throw new ArgumentException("新书籍信息无效");
            newData.Adapt(orginalData);
        }

        public async Task RemoveBookAsync(long ISBN, CancellationToken cancellationToken = default)
        {
            if (!userService.IsAdminState)
                throw new InvalidOperationException("只有管理员才能下架书籍");
            InternalBookDatas.Remove(InternalBookDatas.First(b => b.ISBN == ISBN));
        }

        public async Task SupplyBookAsync(long ISBN, int count, CancellationToken cancellationToken = default)
        {
            if (!userService.IsAdminState)
                throw new InvalidOperationException("只有管理员才能补充书籍数量");
            var data = InternalBookDatas.First(b => b.ISBN == ISBN);
            if (data.Amount + count < 0)
                throw new ArgumentException("下架数量超过库存量");
            data.Amount += count;
        }
    }
}

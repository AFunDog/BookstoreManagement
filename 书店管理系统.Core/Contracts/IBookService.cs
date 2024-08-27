using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Contracts
{
    public interface IBookService
    {
        /// <summary>
        /// 所有书籍的数据
        /// </summary>
        /// <remarks>
        /// 必须处于登录状态才能查看
        /// </remarks>
        IReadOnlyCollection<IReadOnlyBookData> BookDatas { get; }

        /// <summary>
        /// 开始加载书籍数据任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> StartLoadBookDataAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 开始保存书籍数据任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> StartSaveBookDataAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 上架新书籍
        /// </summary>
        /// <remarks>
        /// 只有管理员才可以操作
        /// </remarks>
        /// <param name="bookData">新书籍信息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> AddBookAsync(BookData bookData, CancellationToken cancellationToken = default);

        /// <summary>
        /// 下架书籍
        /// </summary>
        /// <remarks>
        /// 只有管理员才可以操作
        /// </remarks>
        /// <param name="ISBN">书籍的ISBN</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> RemoveBookAsync(long ISBN, CancellationToken cancellationToken = default);

        /// <summary>
        /// 修改书籍的基本信息
        /// </summary>
        /// <remarks>
        /// 只有管理员才可以操作
        /// </remarks>
        /// <param name="ISBN">书籍ISBN</param>
        /// <param name="newBookName">新书籍名称</param>
        /// <param name="newAuthor">新书籍作者</param>
        /// <param name="newPublisher">新出版社</param>
        /// <param name="newPublicationDate">新出版日期</param>
        /// <param name="newCategory">新书籍类别</param>
        /// <param name="newDescription">新书籍描述</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> EditBookDataAsync(
            long ISBN,
            string? newBookName = null,
            string? newAuthor = null,
            string? newPublisher = null,
            DateTimeOffset? newPublicationDate = null,
            string[]? newCategory = null,
            string? newDescription = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// 修改书籍的价格
        /// </summary>
        /// <remarks>
        /// 只有管理员才可以操作
        /// </remarks>
        /// <param name="ISBN">书籍ISBN</param>
        /// <param name="newPrice">新书籍价格</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> ChangeBookPriceAsync(long ISBN, decimal newPrice, CancellationToken cancellationToken = default);

        /// <summary>
        /// 购买书籍
        /// </summary>
        /// <param name="ISBN">书籍ISBN</param>
        /// <param name="count">购买数量</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> BuyBookAsync(long ISBN, int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// 补充书籍数量
        /// </summary>
        /// <remarks>
        /// 只有管理员才可以操作
        /// </remarks>
        /// <param name="ISBN">书籍ISBN</param>
        /// <param name="count">补充数量。若为负数则为下架指定书籍数量</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> SupplyBookAsync(long ISBN, int count, CancellationToken cancellationToken = default);
    }
}

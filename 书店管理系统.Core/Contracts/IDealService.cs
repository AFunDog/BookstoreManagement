using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Contracts
{
    public interface IDealService
    {
        /// <summary>
        /// 所有书籍交易的数据
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// 管理员可以查看所有用户的书籍交易数据
        /// </item>
        /// <item>
        /// 用户只能查看自己的书籍交易数据
        /// </item>
        /// </list>
        /// </remarks>
        IReadOnlyCollection<IReadOnlyBookDealData> BookDealDatas { get; }

        /// <summary>
        /// 所有用户充值交易的数据
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// 管理员可以查看并审批通过所有用户的充值交易数据
        /// </item>
        /// <item>
        /// 用户只能查看自己的充值交易数据
        /// </item>
        /// </list>
        /// </remarks>
        IReadOnlyCollection<IReadOnlyRechargeDealData> RechargeDealDatas { get; }

        /// <summary>
        /// 开始加载交易数据任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> StartLoadDealDataAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 开始保存交易数据任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> StartSaveDealDataAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加新的书籍交易信息
        /// </summary>
        /// <remarks>
        /// 内部方法，由内部服务调用，一般不考虑书籍是否有效
        /// </remarks>
        /// <param name="uid">交易发起用户Id</param>
        /// <param name="ISBN">交易书籍ISBN</param>
        /// <param name="price">交易时书籍单价</param>
        /// <param name="amount">交易书籍数量</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal Task<ActionResult> AddNewBookDealAsync(
            int uid,
            long ISBN,
            decimal price,
            int amount,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// 删除指定的书籍交易信息
        /// </summary>
        /// <remarks>
        /// 只有管理员才能进行操作
        /// </remarks>
        /// <param name="id">书籍交易信息Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> RemoveBookDealAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加新的充值交易信息
        /// </summary>
        /// <remarks>
        /// <para>一般由用户发起，管理员进行审核</para>
        /// <para>此交易信息在添加后会交由管理员审核，审核通过才会生效</para>
        /// </remarks>
        /// <param name="uid">目标用户Id</param>
        /// <param name="addMoney">充值金额</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> AddNewRechargeDealAsync(int uid, decimal addMoney, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批准通过充值交易
        /// </summary>
        /// <remarks>
        /// 只有管理员才能批准通过信息
        /// </remarks>
        /// <param name="id">充值交易信息Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> PassRechargeDealAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除指定的充值交易信息
        /// </summary>
        /// <remarks>
        /// 只有管理员才能进行操作
        /// </remarks>
        /// <param name="id">充值交易信息Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ActionResult> RemoveRechargeDealAsync(int id, CancellationToken cancellationToken = default);
    }
}

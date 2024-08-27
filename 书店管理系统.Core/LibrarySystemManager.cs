using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Core.BasicObjects;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;
using Serilog;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Services;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core
{
    public sealed class LibrarySystemManager : DisposableObject
    {
        /// <summary>
        /// 当前书籍管理系统版本
        /// </summary>
        public const string Version = "0.0.1";
        private static LibrarySystemManager? _instance = null;

        /// <summary>
        /// 书籍管理系统实例
        /// </summary>
        /// <remarks>
        /// 在进行第一次初始化后可用
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public static LibrarySystemManager Instance => _instance ?? throw new InvalidOperationException("书籍管理系统未初始化或已经退出");
        private readonly IServiceProvider _serviceProvider;
        internal static ILogger Logger { get; private set; } =
            new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Debug().CreateLogger();
        public IUserService UserService { get; }
        public IBookService BookService { get; }
        public IDealService DealService { get; }

        /// <summary>
        /// 初始化书籍管理系统
        /// </summary>
        /// <remarks>
        /// 不允许多次初始化
        /// </remarks>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<ActionResult> InitAsync(
            IProgress<LoadingProgress>? progress = null,
            ILogger? logger = null,
            CancellationToken cancellationToken = default
        )
        {
            if (_instance is not null)
                return ActionResult.Error("书籍管理系统已经进行了初始化，再次初始化无效").Log();

            if (logger is not null)
                Logger = logger;

            Stopwatch sw = Stopwatch.StartNew();

            progress?.Report(new(0, "开始初始化"));
            _instance = new();
            progress?.Report(new(25, "正在加载加载用户数据"));
            await Instance.UserService.StartLoadUserDataAsync(cancellationToken);
            progress?.Report(new(75, "正在加载加载书籍数据"));
            await Instance.BookService.StartLoadBookDataAsync(cancellationToken);
            progress?.Report(new(75, "正在加载加载交易数据"));
            await Instance.DealService.StartLoadDealDataAsync(cancellationToken);
            progress?.Report(new(100, "初始化完成"));

            sw.Stop();
            return ActionResult.Success($"书店管理系统初始化完毕 用时 {sw.ElapsedMilliseconds} ms").Log();
        }

        /// <summary>
        /// 退出书籍管理系统
        /// </summary>
        /// <remarks>
        /// 在已经进入系统后可以退出
        /// </remarks>
        public static void ExitSystem()
        {
            try
            {
                Instance.Dispose();
            }
            catch (InvalidOperationException e)
            {
                Logger.Error(e, "书店管理系统未初始化或已经退出");
            }
        }

        internal LibrarySystemManager()
        {
            Logger.Information("书店管理系统开始初始化");
            _serviceProvider = new ServiceCollection()
                .AddSingleton<IDataProvider<UserData>, LocalDataProvider<UserData>>()
                .AddSingleton<IDataProvider<BookData>, LocalDataProvider<BookData>>()
                .AddSingleton<IDataProvider<BookDealData>, LocalDataProvider<BookDealData>>()
                .AddSingleton<IDataProvider<RechargeDealData>, LocalDataProvider<RechargeDealData>>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IBookService, BookService>()
                .AddSingleton<IDealService, DealService>()
                .BuildServiceProvider();

            UserService = _serviceProvider.GetRequiredService<IUserService>();
            BookService = _serviceProvider.GetRequiredService<IBookService>();
            DealService = _serviceProvider.GetRequiredService<IDealService>();
        }

        internal void InitOtherConfig() { }

        protected override async void DisposeManagedResource()
        {
            await Task.WhenAll(
                UserService.StartSaveUserDataAsync(),
                BookService.StartSaveBookDataAsync(),
                DealService.StartSaveDealDataAsync()
            );
        }

        protected override void DisposeUnmanagedResource() { }

        protected override void OnDisposed()
        {
            Logger.Information("书店管理系统退出");
        }
    }
}

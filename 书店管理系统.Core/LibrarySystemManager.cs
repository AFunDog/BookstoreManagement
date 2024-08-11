using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Services;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core
{
    public sealed class LibrarySystemManager
    {
        enum ProcessState
        {
            /// <summary>
            /// 加载前
            /// </summary>
            BeforeLoding,

            /// <summary>
            /// 正在加载
            /// </summary>
            OnLoading,

            /// <summary>
            /// 登录中
            /// </summary>
            OnLogining,

            /// <summary>
            /// 处于用户模式
            /// </summary>
            UserMode,

            /// <summary>
            /// 处于管理员模式
            /// </summary>
            AdminMode,

            /// <summary>
            /// 已退出
            /// </summary>
            Exited,

            /// <summary>
            /// 等待
            /// </summary>
            Wait,

            /// <summary>
            /// 错误
            /// </summary>
            Error,
        }

        private readonly SystemStartConfiguration _systemStartConfiguration;
        private ProcessState _systemProcess = ProcessState.BeforeLoding;
        private static LibrarySystemManager? _instance;

        public static ILogger Logger { get; } = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Debug().CreateLogger();
        public static LibrarySystemManager Instance => _instance ??= CreateInstance();
        public IUserService UserService { get; }
        public IBookService BookService { get; }
        private readonly IUserDataProvider _userDataProvider;
        private readonly IBookDataProvider _bookDataProvider;

        private IServiceProvider ServiceProvider { get; }

        public static LibrarySystemManager CreateInstance(SystemStartConfiguration? configuration = null)
        {
            _instance ??= new(configuration);
            return _instance;
        }

        internal LibrarySystemManager(SystemStartConfiguration? configuration = null)
        {
            _instance = this;
            _systemStartConfiguration =
                configuration ?? new SystemStartConfiguration(new ILibrarySystemProcess.EmptyLibrarySystemProcess());
            Logger.Information("书店管理系统 初始化");

            var builder = new ServiceCollection().AddSingleton<IUserService, UserService>().AddSingleton<IBookService, BookService>();

            if (_systemStartConfiguration.UseSQLDataSource)
            {
                //builder.AddSingleton<IBookDataProvider, >();
            }
            else
            {
                builder.AddSingleton<IBookDataProvider, LocalBookDataProvider>().AddSingleton<IUserDataProvider, LocalUserDataProvider>();
            }

            ServiceProvider = builder.BuildServiceProvider();

            _userDataProvider =
                ServiceProvider.GetService<IUserDataProvider>() ?? throw new InvalidOperationException("UserDataProvider is null");
            _bookDataProvider =
                ServiceProvider.GetService<IBookDataProvider>() ?? throw new InvalidOperationException("BookDataProvider is null");
            UserService = ServiceProvider.GetService<IUserService>() ?? throw new InvalidOperationException("UserService is null");
            BookService = ServiceProvider.GetService<IBookService>() ?? throw new InvalidOperationException("BookService is null");
        }

        public async Task StartLoadingDataAsync(IProgress<ActionResult>? progress = null, CancellationToken cancellationToken = default)
        {
            _systemProcess = ProcessState.OnLoading;
            Stopwatch sw = Stopwatch.StartNew();
            await Task.WhenAll(_userDataProvider.LoadUserDatasAsync(), _bookDataProvider.LoadBookDatasAsync());
            await _systemStartConfiguration.LibrarySystemProcess.StartLoadingAsync();
            sw.Stop();
            ActionResult.Success().TryReportResult("数据加载完毕", progress, Logger);
            Logger.Information("用时{time}Ms", sw.ElapsedMilliseconds);
        }

        public async Task StartLoginAsync(IProgress<ActionResult>? progress = null, CancellationToken cancellationToken = default)
        {
            _systemProcess = ProcessState.OnLogining;
            ActionResult.Success().TryReportResult("进入登录模式", progress, Logger);
            await _systemStartConfiguration.LibrarySystemProcess.StartLoginAsync();
        }

        public async Task TryLoginAsync(
            string username,
            string password,
            IProgress<ActionResult>? progress = null,
            CancellationToken cancellationToken = default
        )
        {
            const string Admin = nameof(Admin);
            Logger.Information("{username} {password} 尝试登录", username, password);

            if (username == Admin && password == "admin")
            {
                ActionResult.Success().TryReportResult("管理员登录", progress, Logger);
                _systemProcess = ProcessState.AdminMode;
                await _systemStartConfiguration.LibrarySystemProcess.ToAdminModeAsync();
            }
            else
            {
                var result = await UserService.TryLoginAsync(username, password);
                if (result.IsSucceed)
                {
                    _systemProcess = ProcessState.UserMode;
                    result.TryReportResult("用户登录", progress, Logger);
                    await _systemStartConfiguration.LibrarySystemProcess.ToUserModeAsync();
                }
                else
                {
                    result.TryReportResult("用户登录", progress, Logger);
                }
            }
        }

        public async Task TryExitLoginAsync(IProgress<ActionResult>? progress = null, CancellationToken cancellationToken = default)
        {
            switch (_systemProcess)
            {
                case ProcessState.AdminMode:
                    _systemProcess = ProcessState.Wait;
                    await _systemStartConfiguration.LibrarySystemProcess.ExitAdminModeAsync();
                    ActionResult.Success().TryReportResult("退出管理员模式", progress, Logger);
                    break;
                case ProcessState.UserMode:
                    _systemProcess = ProcessState.Wait;
                    await _systemStartConfiguration.LibrarySystemProcess.ExitUserModeAsync();
                    ActionResult.Success().TryReportResult("退出用户模式", progress, Logger);
                    break;
                default:
                    ActionResult.Warning("并未处于登录状态").TryReportResult("尝试退出登录失败", progress, Logger);
                    break;
            }
        }

        public async Task CloseAsync()
        {
            if (_systemProcess == ProcessState.UserMode)
            {
                await _systemStartConfiguration.LibrarySystemProcess.ExitUserModeAsync();
            }
            if (_systemProcess == ProcessState.AdminMode)
            {
                await _systemStartConfiguration.LibrarySystemProcess.ExitAdminModeAsync();
            }
            ActionResult.Success().TryReportResult("退出系统", null, Logger);
            await Task.WhenAll(
                _userDataProvider.SaveUserDatasAsync(),
                _bookDataProvider.SaveBookDatasAsync(),
                _systemStartConfiguration.LibrarySystemProcess.ExitSystemAsync()
            );
            _systemProcess = ProcessState.Exited;
        }

        ~LibrarySystemManager()
        {
            Logger.Information("{name} 退出", nameof(LibrarySystemManager));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Animations;
using CoreServices.Localization;
using CoreServices.WinUI.Contracts;
using CoreServices.WinUI.Services;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Serilog;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;
using 书店管理系统.Core;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;
using 书店管理系统.WinUI;
using 书店管理系统.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static App Instance => (Current as App) ?? new();

        public static T GetService<T>()
            where T : notnull => Instance.ServiceProvider.GetRequiredService<T>();

        public static ILogger Logger { get; } =
            new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().WriteTo.Debug().CreateLogger();

        public IServiceProvider ServiceProvider { get; }

        private StartWindow? _startWindow;
        private MainWindow? _mainWindow;
        internal StartWindow StartWindow => _startWindow ?? throw new InvalidOperationException();
        internal MainWindow MainWindow => _mainWindow ?? throw new InvalidOperationException();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            //_ = LibrarySystemManager.CreateInstance(new(new LibrarySystemProcess()));

            var builder = new ServiceCollection()
                // 注册基础服务
                .AddSingleton<INavigateService, NavigateService>()
                .AddSingleton<ILocalizeService, LocalizeService>()
                // 注册视图模型
                .AddTransient<LoginUserViewModel>()
                .AddTransient<LoginAdminViewModel>()
                .AddTransient<AdminMainViewModel>()
                .AddTransient<UserMainViewModel>()
                .AddTransient<UserBuyBookViewModel>();

            ServiceProvider = builder.BuildServiceProvider();
            #region DEBUG控制台

#if DEBUG
            Vanara.PInvoke.Kernel32.AllocConsole();
            Log.Warning("请不要主动关闭此控制台");
#endif
            #endregion
            UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            throw new ArgumentException(e.Message);
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                StartGUI();
                await LibrarySystemManager.InitAsync(new Progress<LoadingProgress>(p => StartWindow.Progress = p), Logger);
                Logger.Information("系统初始化完毕");
            }
            catch (Exception e)
            {
                Logger.Information(e, "");
                OnExited();
            }
        }

        internal bool IsExiting { get; private set; } = false;

        internal void OnExited()
        {
            if (IsExiting)
                return;
            IsExiting = true;
            Logger.Information("系统正在退出");

            ExitGUI();

            LibrarySystemManager.ExitSystem();
            #region 关闭DEBUG控制台

#if DEBUG
            Vanara.PInvoke.Kernel32.FreeConsole();
#endif
            #endregion
            Exit();
            IsExiting = false;
        }

        internal async Task LoginAsync()
        {
            const double AnimtaionDurationMs = 500;

            App.Instance.MainWindow.Login();
            await Task.Delay(1000);
            App.Instance.MainWindow.SetWindowOpacity(0);
            App.Instance.MainWindow.Show();

            await Task.WhenAll(ToHideStartWindowAnimation(), ToShowMainWindowAnimation());

            async Task ToHideStartWindowAnimation()
            {
                await Task.Yield();
                Stopwatch sw = Stopwatch.StartNew();
                var easingFunc = EasingType.Cubic.ToEasingFunction();
                while (sw.ElapsedMilliseconds < AnimtaionDurationMs)
                {
                    App.Instance.StartWindow.SetWindowOpacity(
                        (byte)(255 * (1 - easingFunc!.Ease(sw.ElapsedMilliseconds / AnimtaionDurationMs)))
                    );
                    await Task.Delay(1);
                }
                App.Instance.StartWindow.SetWindowOpacity(0);
                sw.Stop();
                App.Instance.StartWindow.Hide();
                App.Instance.StartWindow.BackToLoginUserPage();
            }
            async Task ToShowMainWindowAnimation()
            {
                await Task.Yield();
                Stopwatch sw = Stopwatch.StartNew();
                var easingFunc = EasingType.Cubic.ToEasingFunction();
                while (sw.ElapsedMilliseconds < AnimtaionDurationMs)
                {
                    App.Instance.MainWindow.SetWindowOpacity((byte)(255 * easingFunc!.Ease(sw.ElapsedMilliseconds / AnimtaionDurationMs)));
                    await Task.Delay(1);
                }
                App.Instance.MainWindow.SetWindowOpacity(255);
                sw.Stop();
            }
        }

        internal async Task LogoutAsync()
        {
            const double AnimtaionDurationMs = 500;
            App.Instance.StartWindow.Show();
            await Task.WhenAll(ToShowStartWindowAnimation(), ToHideMainWindowAnimation());

            async Task ToShowStartWindowAnimation()
            {
                await Task.Yield();
                Stopwatch sw = Stopwatch.StartNew();
                var easingFunc = EasingType.Cubic.ToEasingFunction();
                while (sw.ElapsedMilliseconds < AnimtaionDurationMs)
                {
                    App.Instance.StartWindow.SetWindowOpacity(
                        (byte)(255 * (easingFunc!.Ease(sw.ElapsedMilliseconds / AnimtaionDurationMs)))
                    );
                    await Task.Delay(1);
                }
                App.Instance.StartWindow.SetWindowOpacity(255);
                sw.Stop();
            }
            async Task ToHideMainWindowAnimation()
            {
                await Task.Yield();
                Stopwatch sw = Stopwatch.StartNew();
                var easingFunc = EasingType.Cubic.ToEasingFunction();
                while (sw.ElapsedMilliseconds < AnimtaionDurationMs)
                {
                    App.Instance.MainWindow.SetWindowOpacity(
                        (byte)(255 * (1 - easingFunc!.Ease(sw.ElapsedMilliseconds / AnimtaionDurationMs)))
                    );
                    await Task.Delay(1);
                }
                App.Instance.MainWindow.SetWindowOpacity(0);
                sw.Stop();
                App.Instance.MainWindow.Logout();
                App.Instance.MainWindow.Hide();
            }
        }

        private void StartGUI()
        {
            _startWindow = new StartWindow();
            _mainWindow = new MainWindow();
            _mainWindow.SetWindowOpacity(0);
            _mainWindow.Activate();
            _mainWindow.Hide();
            _startWindow.Activate();
            _startWindow.Closed += (_, _) => OnExited();
            _mainWindow.Closed += (_, _) => OnExited();
        }

        private void ExitGUI()
        {
            _startWindow?.Close();
            _mainWindow?.Close();
        }
    }
}

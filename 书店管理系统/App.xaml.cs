using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CoreServices.Localization;
using CoreServices.WinUI.Contracts;
using CoreServices.WinUI.Services;
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
using 书店管理系统.Contracts;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Services;
using 书店管理系统.Services;
using 书店管理系统.ViewModels;
using 书店管理系统.Windows;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统
{
    // 使用本地用户数据
    using UserDataProvider = LocalUserDataProvider;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static App Instance => (Current as App) ?? new();

        public static T GetService<T>()
        {
            if ((Current as App)!.ServiceProvider.GetService<T>() is T service)
            {
                return service;
            }
            throw new ArgumentException($"{typeof(T)} 服务未找到");
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            var builder = new ServiceCollection()
                // 注册基础服务
                .AddTransient<IActivationService, ActivationService>()
                .AddSingleton<INavigateService, NavigateService>()
                .AddSingleton<ILocalizeService, LocalizeService>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IUserDataProvider, UserDataProvider>()
                .AddSingleton<IBookDataProvider, LocalBookDataProvider>()
                .AddSingleton<IBookService, BookService>()
                // 注册视图模型
                .AddTransient<LoginWindowViewModel>()
                .AddTransient<MainWindowViewModel>()
                .AddTransient<AdminLoginViewModel>()
                .AddTransient<UserLoginViewModel>()
                .AddTransient<UserMainViewModel>()
                .AddTransient<AdminMainViewModel>()
                .AddTransient<AdminUserManageViewModel>()
                .AddTransient<AdminBookManageViewModel>();

            ServiceProvider = builder.BuildServiceProvider();
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Debug().CreateLogger();

            UnhandledException += App_UnhandledException;
            App.GetService<IActivationService>().LaunchingActivateAsync().Wait();
        }

        private MainWindow? _mainWindow;
        private LoginWindow? _loginWindow;
        public MainWindow? MainWindow
        {
            get => _mainWindow;
            set
            {
                if (_mainWindow is not null)
                    _mainWindow.Closed -= MainWindow_Closed;
                _mainWindow = value;
                if (_mainWindow is not null)
                    _mainWindow.Closed += MainWindow_Closed;
            }
        }
        public LoginWindow? LoginWindow
        {
            get => _loginWindow;
            set
            {
                if (_loginWindow is not null)
                    _loginWindow.Closed -= LoginWindow_Closed;
                _loginWindow = value;
                if (_loginWindow is not null)
                    _loginWindow.Closed += LoginWindow_Closed;
            }
        }

        private void LoginWindow_Closed(object sender, WindowEventArgs args)
        {
            if (_mainWindow is null)
                Exit();
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            if (_loginWindow is null)
                Exit();
        }

        public IServiceProvider ServiceProvider { get; }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            throw new ArgumentException(e.Message);
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            LoginWindow = new LoginWindow();
            LoginWindow.Activate();
        }
    }
}

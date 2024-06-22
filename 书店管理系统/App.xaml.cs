using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using 书店管理系统.Contracts;
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
                .AddTransient<IInitLoader, InitLoader>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IDragMove, DragMove>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IUserDataProvider, UserDataProvider>()
                // 注册视图模型
                .AddTransient<LoginWindowViewModel>()
                .AddTransient<AdminLoginViewModel>()
                .AddTransient<UserLoginViewModel>();

            ServiceProvider = builder.BuildServiceProvider();
            UnhandledException += App_UnhandledException;
        }


        public IServiceProvider ServiceProvider
        {
            get;
        }
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
            var window = new LoginWindow();
            window.Activate();
        }
    }
}

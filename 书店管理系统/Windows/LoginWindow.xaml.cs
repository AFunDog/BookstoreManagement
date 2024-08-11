using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Animations;
using CoreServices.Localization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Serilog;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;
using 书店管理系统.Contracts;
using 书店管理系统.Core;
using 书店管理系统.Core.Structs;
using 书店管理系统.ViewModels;
using 书店管理系统.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginWindow : WindowEx
    {
        public LoginWindowViewModel ViewModel { get; set; }

        public LoginWindow()
        {
            ViewModel = App.GetService<LoginWindowViewModel>();
            ExtendsContentIntoTitleBar = true;
            this.InitializeComponent();
            SetTitleBar(WindowTitleBar);
            //App.GetService<IDragMove>().Register(WindowTitleBar, AppWindow);

            ContentFrame.Navigate(typeof(LoadingPage));
            var progress = new Progress<ActionResult>(async r =>
            {
                if (r.IsSucceed)
                {
                    await LibrarySystemManager.Instance.StartLoginAsync();
                }
            });

#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            LibrarySystemManager.Instance.StartLoadingDataAsync(progress);
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        }

        public void ToLoginState()
        {
            ContentFrame.Navigate(typeof(UserLoginPage), null, new DrillInNavigationTransitionInfo());
        }

        public void TryNavigateToPage(Type pageType, NavigationTransitionInfo transitionInfo)
        {
            ContentFrame.Navigate(pageType, null, transitionInfo);
        }

        private async void LoginWindow_Closed(object sender, WindowEventArgs args)
        {
            Log.Debug("LoginWindow_Closed");
            await LibrarySystemManager.Instance.CloseAsync();
        }
    }
}

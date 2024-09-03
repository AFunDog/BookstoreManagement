using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Animations;
using Microsoft.Extensions.DependencyInjection;
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
using 书店管理系统.Core.Structs;
using 书店管理系统.WinUI.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartWindow : Window
    {
        private LoadingProgress _progress;
        public LoadingProgress Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                if (contentFrame.Content is LoadingPage page)
                {
                    page.LoadingProgress = _progress.Progress;
                    page.LoadingMessage = _progress.Message;
                }
                if (value.Progress == 100)
                {
                    ToLoginUserPage();
                }
            }
        }

        public StartWindow()
        {
            this.InitializeComponent();
            this.SetWindowSize(960, 540);
            this.SetIsResizable(false);
            ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(windowTitleBar);
            contentFrame.Navigate(typeof(LoadingPage));
        }

        private async void ToLoginUserPage()
        {
            await Task.Delay(500);
            contentFrame.Navigate(typeof(LoginUserPage), null, new DrillInNavigationTransitionInfo());
        }

        internal void BackToLoginUserPage()
        {
            contentFrame.Navigate(
                typeof(LoginUserPage),
                null,
                new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft }
            );
        }

        internal void ToLoginAdminPage()
        {
            contentFrame.Navigate(
                typeof(LoginAdminPage),
                null,
                new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight }
            );
        }

        private void OnClosed(object sender, WindowEventArgs args)
        {
            App.Logger.Debug("开始窗口关闭");
        }
    }
}

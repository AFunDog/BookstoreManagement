using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CoreLibrary.Toolkit.WinUI.Contracts;
using CoreLibrary.Toolkit.WinUI.Controls;
using CoreLibrary.Toolkit.WinUI.Structs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Vanara.PInvoke;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;
using 书店管理系统.Core;
using 书店管理系统.WinUI.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.WinUI
{
    sealed partial class MainWindowModel : ObservableObject
    {
        [ObservableProperty]
        private string _subTitle = "";

        [ObservableProperty]
        private ICollection<IPageItem> _headerPageItems = [];

        [ObservableProperty]
        private ICollection<IPageItem> _footerPageItems = [];
    }

    public sealed partial class MainWindow : Window
    {
        internal MainWindowModel Model { get; } = new();

        public MainWindow()
        {
            this.InitializeComponent();
            this.SetWindowSize(1280, 720);
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(windowTitleBar);
            this.SetIsResizable(false);

            navigationView.Loaded += (s, e) =>
            {
                CustomNavigationView source = (s as CustomNavigationView)!;
                App.GetService<INavigateService>().AttachService(source.ContentFrame);
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        internal void Login()
        {
            if (LibrarySystemManager.Instance.UserService.IsAdminState)
            {
                LoginAsAdmin();
            }
            else
            {
                LoginAsUser();
            }
        }

        private void LoginAsAdmin()
        {
            Model.SubTitle = "管理员界面";
            Model.HeaderPageItems = [new PageItem("控制面板", "\uE70C", typeof(AdminMainPage))];
            Model.FooterPageItems =
            [
                new SeparatorItem(),
                new AsyncCommandItem(
                    "退出登录",
                    "\uE751",
                    async () =>
                    {
                        await LibrarySystemManager.Instance.UserService.LogoutAsync();
                        await App.Instance.LogoutAsync();
                    },
                    () =>
                        LibrarySystemManager.Instance.UserService.IsAdminState
                        || LibrarySystemManager.Instance.UserService.LoginUser is not null
                )
            ];
            App.GetService<INavigateService>().Navigate(typeof(AdminMainPage));
        }

        private void LoginAsUser()
        {
            Model.SubTitle = "用户界面";
            Model.HeaderPageItems =
            [
                new PageItem("主页", "\uEA4A", typeof(UserMainPage)),
                new SeparatorItem(),
                new PageItem("书籍列表", "\uE8F1", typeof(UserBuyBookPage))
            ];
            Model.FooterPageItems =
            [
                new SeparatorItem(),
                new AsyncCommandItem(
                    "退出登录",
                    "\uE751",
                    async () =>
                    {
                        await LibrarySystemManager.Instance.UserService.LogoutAsync();
                        await App.Instance.LogoutAsync();
                    },
                    () =>
                        LibrarySystemManager.Instance.UserService.IsAdminState
                        || LibrarySystemManager.Instance.UserService.LoginUser is not null
                )
            ];
            App.GetService<INavigateService>().Navigate(typeof(UserMainPage));
        }

        internal void Logout()
        {
            Model.HeaderPageItems = [];
            Model.FooterPageItems = [];
        }

        private void OnClosed(object sender, WindowEventArgs args)
        {
            App.Logger.Debug("主窗口关闭");
        }
    }
}

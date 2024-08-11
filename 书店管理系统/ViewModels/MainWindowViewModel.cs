using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CoreServices.WinUI.Contracts;
using CoreServices.WinUI.Structs;
using 书店管理系统.Core;
using 书店管理系统.Core.Structs;
using 书店管理系统.Views;

namespace 书店管理系统.ViewModels
{
    public partial class MainWindowViewModel : ObservableRecipient
    {
        private LoginType _loginType;

        [ObservableProperty]
        private ObservableCollection<IPageItem> _headerPageItems = [];

        [ObservableProperty]
        private ObservableCollection<IPageItem> _footerPageItems = [];

        public void SetLoginType(LoginType loginType)
        {
            _loginType = loginType;
            switch (_loginType)
            {
                case LoginType.Admin:
                    HeaderPageItems =
                    [
                        new PageItem("控制面板", "\uE80F", typeof(AdminMainPage)),
                        new SeparatorItem(),
                        new PageItem("用户管理", "\uE716", typeof(AdminUserManagePage)),
                        new PageItem("书籍管理", "\uE8F1", typeof(AdminBookManagePage)),
                    ];
                    FooterPageItems =
                    [
                        new AsyncCommandItem(
                            "退出登录",
                            "\uF0B0",
                            async () =>
                            {
                                await LibrarySystemManager.Instance.TryExitLoginAsync();
                                await LibrarySystemManager.Instance.StartLoginAsync();
                            }
                        )
                    ];
                    break;
                case LoginType.User:
                    HeaderPageItems =
                    [
                        new PageItem("用户主页", "\uE80F", typeof(UserMainPage)),
                        new SeparatorItem(),
                        new PageItem("图书列表", "\uE8F1", typeof(UserBuyBookPage))
                    ];
                    FooterPageItems =
                    [
                        new AsyncCommandItem(
                            "退出登录",
                            "\uF0B0",
                            async () =>
                            {
                                await LibrarySystemManager.Instance.TryExitLoginAsync();
                                await LibrarySystemManager.Instance.StartLoginAsync();
                            }
                        )
                    ];
                    break;
                default:
                    break;
            }
        }
    }
}

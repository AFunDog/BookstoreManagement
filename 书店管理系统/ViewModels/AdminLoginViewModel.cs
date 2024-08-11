using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI;
using 书店管理系统.Core;
using 书店管理系统.Core.Structs;
using 书店管理系统.Views;

namespace 书店管理系统.ViewModels
{
    public partial class AdminLoginViewModel : ObservableRecipient
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AdminLoginCommand))]
        private string _adminPassword = string.Empty;

        [ObservableProperty]
        private string _adminLoginInfo = string.Empty;

        [ObservableProperty]
        private bool _isInfoShow = false;

        [ObservableProperty]
        private InfoBarSeverity _severity = InfoBarSeverity.Informational;

        [RelayCommand]
        private void BackTo()
        {
            App.Instance.LoginWindow!.TryNavigateToPage(
                typeof(UserLoginPage),
                new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft }
            );
            InfoBar infoBar = new InfoBar();
        }

        [RelayCommand(CanExecute = nameof(CanAdminLogin))]
        private async Task AdminLogin()
        {
            var process = new Progress<ActionResult>();
            await LibrarySystemManager.Instance.TryLoginAsync(
                "Admin",
                AdminPassword,
                new Progress<ActionResult>(r =>
                {
                    if (r.IsSucceed)
                    {
                        AdminLoginInfo = "登录成功";
                        IsInfoShow = true;
                        Severity = InfoBarSeverity.Success;
                    }
                    else
                    {
                        AdminLoginInfo = "登录失败,密钥错误";
                        IsInfoShow = true;
                        Severity = InfoBarSeverity.Error;
                    }
                })
            );
        }

        private bool CanAdminLogin() => !string.IsNullOrEmpty(AdminPassword);
    }
}

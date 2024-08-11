using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Serilog;
using 书店管理系统.Core;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;
using 书店管理系统.Views;

namespace 书店管理系统.ViewModels
{
    public partial class UserLoginViewModel : ObservableRecipient
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UserLoginCommand))]
        private string _userName = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UserLoginCommand))]
        private string _password = string.Empty;

        private readonly IUserService _userService;

        public UserLoginViewModel(IUserService userService)
        {
            _userService = userService;
        }

        [RelayCommand(CanExecute = nameof(CanUserLogin))]
        private async Task UserLogin()
        {
            await LibrarySystemManager.Instance.TryLoginAsync(UserName, Password, new Progress<ActionResult>(r => { }));
        }

        private bool CanUserLogin() => !(string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password));

        [RelayCommand]
        private void ToAdminLoginPage()
        {
            App.Instance.LoginWindow!.TryNavigateToPage(
                typeof(AdminLoginPage),
                new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight }
            );
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Contracts;
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
        private readonly ILogger _logger;

        public UserLoginViewModel(IUserService userService, ILogger logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [RelayCommand(CanExecute = nameof(CanUserLogin))]
        private void UserLogin()
        {
            var res = _userService.CheckUserLoginInfo(UserName, Password);
            if (res.ResultType == Structs.ResultType.OK)
            {
                _logger.LogInfo("登录成功");
            }
            else
            {
                _logger.LogError($"登录失败 {res.Message}");
            }
        }
        private bool CanUserLogin() => !(string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password));

        [RelayCommand]
        private void ToAdminLoginPage()
        {
            Messenger.Send(new Tuple<Type, NavigationTransitionInfo>(typeof(AdminLoginPage), 
                new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight }), 
                LoginWindowViewModel.NavigateTo);
        }
    }
}

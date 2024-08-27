using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using 书店管理系统.Core;

namespace 书店管理系统.WinUI.ViewModels
{
    internal sealed partial class LoginUserViewModel : ObservableObject
    {
        [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _username = "";

        [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _password = "";

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async Task Login()
        {
            var res = await LibrarySystemManager.Instance.UserService.LoginAsUserAsync(Username, Password);
            if (res.IsSucceed)
            {
                await App.Instance.LoginAsync();
            }
        }

        private bool CanLogin() => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);

        [RelayCommand]
        private void ToAdminPage()
        {
            App.Instance.StartWindow.ToLoginAdminPage();
        }
    }
}

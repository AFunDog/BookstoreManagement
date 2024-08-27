using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using 书店管理系统.Core;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.WinUI.ViewModels
{
    internal sealed partial class UserMainViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserData _loginUser = LibrarySystemManager.Instance.UserService.LoginUser!;

        [RelayCommand]
        private async Task AddRechargeRequest(decimal money)
        {
            await LibrarySystemManager.Instance.DealService.AddNewRechargeDealAsync(LoginUser.Id, money);
        }
    }
}

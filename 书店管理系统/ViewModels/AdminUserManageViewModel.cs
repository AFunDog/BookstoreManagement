using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapster;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Serilog;
using 书店管理系统.Controls;
using 书店管理系统.Controls.ContentDialogs;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.ViewModels
{
    public sealed partial class AdminUserManageViewModel : ObservableRecipient
    {
        private readonly IUserService _userService;

        [ObservableProperty]
        private IReadOnlyCollection<UserData> _userDatas;

        public AdminUserManageViewModel(IUserService userService)
        {
            _userService = userService;
            _userDatas = _userService.UserDatas;
        }

        [RelayCommand]
        private async Task RegisterUser(XamlRoot xamlRoot)
        {
            var dialog = new EditUserDataContentDialog() { XamlRoot = xamlRoot };
            var result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    Log.Information("添加用户 {@res}", _userService.RegisterUser(dialog.UserData));
                    break;
                default:
                    break;
            }
            OnPropertyChanged(nameof(UserDatas));
        }

        [RelayCommand]
        private async Task RemoveUser(MenuFlyout flyout)
        {
            var dialog = new AffirmRemoveUserContentDialog() { XamlRoot = flyout.XamlRoot };
            var data = (UserData)flyout.Target.DataContext;
            var result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    Log.Information("删除用户 {@res}", _userService.LogoutUser(data));
                    break;
                default:
                    break;
            }
            OnPropertyChanged(nameof(UserDatas));
        }

        [RelayCommand]
        private async Task EditUser(MenuFlyout flyout)
        {
            var data = (UserData)flyout.Target.DataContext;
            var clone = data.Adapt<UserData>();
            var dialog = new EditUserDataContentDialog() { XamlRoot = flyout.XamlRoot }.SetEditMode(clone);
            var result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    dialog.UserData.Adapt(data);
                    Log.Information("修改用户 {@res}", dialog.UserData.Id);
                    break;
                default:
                    break;
            }
            OnPropertyChanged(nameof(UserDatas));
        }
    }
}

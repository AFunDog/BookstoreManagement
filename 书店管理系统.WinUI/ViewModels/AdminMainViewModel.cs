using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using 书店管理系统.Core;
using 书店管理系统.Core.Structs;
using 书店管理系统.WinUI.Controls.ContentDialogs;

namespace 书店管理系统.WinUI.ViewModels
{
    internal sealed partial class AdminMainViewModel : ObservableObject
    {
        [ObservableProperty]
        private IReadOnlyCollection<IReadOnlyUserData> _userDatas = LibrarySystemManager.Instance.UserService.UserDatas;

        [ObservableProperty]
        private IReadOnlyCollection<IReadOnlyBookData> _bookDatas = LibrarySystemManager.Instance.BookService.BookDatas;

        [ObservableProperty]
        private IReadOnlyCollection<IReadOnlyBookDealData> _bookDealDatas = LibrarySystemManager.Instance.DealService.BookDealDatas;

        [ObservableProperty]
        private IReadOnlyCollection<IReadOnlyRechargeDealData> _rechargeDealDatas = LibrarySystemManager
            .Instance
            .DealService
            .RechargeDealDatas;

        [ObservableProperty]
        private UserData _selectedUserData = new();

        [ObservableProperty]
        private BookData _selectedBookData = new();

        [ObservableProperty]
        private BookDealData _selectedBookDealData = new();

        [ObservableProperty]
        private RechargeDealData _selectedRechargeDealData = new();

        private static XamlRoot AppXamlRoot => App.Instance.MainWindow.Content.XamlRoot;

        [RelayCommand]
        private async Task DeleteUserData()
        {
            if ((await LibrarySystemManager.Instance.UserService.RemoveUserAsync(SelectedUserData.Id)).IsSucceed) { }
        }

        [RelayCommand]
        private async Task DeleteBookData()
        {
            if ((await LibrarySystemManager.Instance.BookService.RemoveBookAsync(SelectedBookData.ISBN)).IsSucceed) { }
        }

        [RelayCommand]
        private async Task ChangeBookPrice(decimal price)
        {
            if ((await LibrarySystemManager.Instance.BookService.ChangeBookPriceAsync(SelectedBookData.ISBN, price)).IsSucceed) { }
        }

        [RelayCommand]
        private async Task SupplyBookAmount(double count)
        {
            if ((await LibrarySystemManager.Instance.BookService.SupplyBookAsync(SelectedBookData.ISBN, (int)count)).IsSucceed) { }
        }

        [RelayCommand]
        private async Task PassRechargeDeal()
        {
            if ((await LibrarySystemManager.Instance.DealService.PassRechargeDealAsync(SelectedRechargeDealData.Id)).IsSucceed) { }
        }

        [RelayCommand]
        private async Task DeleteRechargeDeal()
        {
            if ((await LibrarySystemManager.Instance.DealService.RemoveRechargeDealAsync(SelectedRechargeDealData.Id)).IsSucceed) { }
        }
    }
}

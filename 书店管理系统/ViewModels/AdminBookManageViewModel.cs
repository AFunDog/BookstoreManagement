using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Serilog;
using 书店管理系统.Controls.ContentDialogs;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.ViewModels
{
    public sealed partial class AdminBookManageViewModel : ObservableRecipient
    {
        private readonly IBookService _bookService;

        [ObservableProperty]
        private IReadOnlyCollection<BookData> _bookDatas;

        public AdminBookManageViewModel(IBookService bookService)
        {
            _bookService = bookService;
            _bookDatas = _bookService.BookDatas;
        }

        [RelayCommand]
        private async Task AddBook(XamlRoot xamlRoot)
        {
            var dialog = new EditBookDataContentDialog() { XamlRoot = xamlRoot };
            var result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    Log.Information("添加图书 {@res}", _bookService.AddBook(dialog.BookData));
                    break;
                default:
                    break;
            }
            OnPropertyChanged(nameof(BookDatas));
        }

        [RelayCommand]
        private async Task RemoveBook(MenuFlyout flyout)
        {
            var dialog = new AffirmRemoveBookContentDialog() { XamlRoot = flyout.XamlRoot };
            var data = (BookData)flyout.Target.DataContext;
            var result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    Log.Information("删除图书 {@res}", _bookService.RemoveBook(data));
                    break;
                default:
                    break;
            }
            OnPropertyChanged(nameof(BookDatas));
        }
    }
}

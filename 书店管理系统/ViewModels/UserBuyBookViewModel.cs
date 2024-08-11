using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.ViewModels
{
    public sealed partial class UserBuyBookViewModel : ObservableObject
    {
        private readonly IBookService _bookService;

        [ObservableProperty]
        private IReadOnlyCollection<BookData> _bookDatas;

        public UserBuyBookViewModel(IBookService bookService)
        {
            _bookService = bookService;
            _bookDatas = _bookService.BookDatas;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using 书店管理系统.Core;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.WinUI.ViewModels
{
    internal sealed partial class UserBuyBookViewModel : ObservableObject
    {
        [ObservableProperty]
        private IReadOnlyCollection<IReadOnlyBookData> _bookDatas = LibrarySystemManager.Instance.BookService.BookDatas;
    }
}

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
    internal sealed partial class AdminMainViewModel : ObservableRecipient
    {
        private readonly IUserService _userService;

        public IReadOnlyCollection<UserData> UserDatas => _userService.UserDatas;

        [ObservableProperty]
        private int _userCount;

        public AdminMainViewModel(IUserService userService)
        {
            _userService = userService;
            _userCount = _userService.UserDatas.Count;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.ViewModels
{
    public sealed partial class UserMainViewModel : ObservableRecipient
    {
        private readonly IUserService _userService;

        [ObservableProperty]
        private UserData _loginUserData;

        public UserMainViewModel(IUserService userService)
        {
            _userService = userService;
            _loginUserData = _userService.LoginUser!;
        }
    }
}

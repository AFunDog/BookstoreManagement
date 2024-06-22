using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using 书店管理系统.Views;

namespace 书店管理系统.ViewModels
{
    public partial class AdminLoginViewModel : ObservableRecipient
    {
        [RelayCommand]
        private void BackTo()
        {
            Messenger.Send(new Tuple<Type, NavigationTransitionInfo>(typeof(UserLoginPage),
                new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft }),
                LoginWindowViewModel.NavigateTo);
            
        }

    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.ViewModels
{
    public partial class LoginWindowViewModel : ObservableRecipient
    {
        public const string NavigateTo = nameof(NavigateTo);
        public Frame Frame { get; set; } = new();

        public LoginWindowViewModel()
        {
            Messenger.Register<LoginWindowViewModel, Tuple<Type, NavigationTransitionInfo>, string>(this, NavigateTo, TryNavigateToPage);
        }

        private void TryNavigateToPage(LoginWindowViewModel recipient, Tuple<Type, NavigationTransitionInfo> message)
        {
            Frame.Navigate(message.Item1, null, message.Item2);
        }
    }
}

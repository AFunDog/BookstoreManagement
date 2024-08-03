using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.ViewModels
{
    public partial class LoginWindowViewModel : ObservableRecipient
    {
        public const string NavigateTo = nameof(NavigateTo);
        public const string LoginTo = nameof(LoginTo);

        public Frame Frame { get; set; } = new();

        public LoginWindowViewModel()
        {
            Messenger.Register<LoginWindowViewModel, Tuple<Type, NavigationTransitionInfo>, string>(
                this,
                NavigateTo,
                TryNavigateToPage
            );
            Messenger.Register<LoginWindowViewModel, LoginInfo, string>(this, LoginTo, TryLoginTo);
        }

        private async void TryLoginTo(LoginWindowViewModel recipient, LoginInfo message)
        {
            await Task.Delay(1000);
            App.Instance.LoginWindow!.Closed += (_, _) =>
            {
                App.Instance.MainWindow = new Windows.MainWindow(message);
                App.Instance.MainWindow.Activate();
                AnimationBuilder
                    .Create()
                    .Opacity(from: 0, to: 1, duration: TimeSpan.FromMilliseconds(400))
                    .Start(App.Instance.MainWindow.Content);
            };
            App.Instance.LoginWindow!.DispatcherQueue.TryEnqueue(async () =>
            {
                await AnimationBuilder
                    .Create()
                    .Opacity(to: 0)
                    .StartAsync(App.Instance.LoginWindow!.Content);
                App.Instance.LoginWindow!.Close();
                App.Instance.LoginWindow = null;
            });
        }

        private void TryNavigateToPage(
            LoginWindowViewModel recipient,
            Tuple<Type, NavigationTransitionInfo> message
        )
        {
            Frame.Navigate(message.Item1, null, message.Item2);
        }
    }
}

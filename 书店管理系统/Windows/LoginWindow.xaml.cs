﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CoreServices.Localization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;
using 书店管理系统.Contracts;
using 书店管理系统.ViewModels;
using 书店管理系统.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginWindow : WindowEx
    {
        public LoginWindowViewModel ViewModel { get; set; }

        public LoginWindow()
        {
            ViewModel = App.GetService<LoginWindowViewModel>();
            ExtendsContentIntoTitleBar = true;
            this.InitializeComponent();
            ViewModel.Frame = ContentFrame;
            SetTitleBar(WindowTitleBar);
            //App.GetService<IDragMove>().Register(WindowTitleBar, AppWindow);

            ContentFrame.Navigate(typeof(LoadingPage));
            var progress = new Progress<bool>(v =>
            {
                if (v)
                {
                    Loader_Loaded();
                }
            });
            App.GetService<IActivationService>().LaunchedActivateAsync(progress: progress);
        }

        private void Loader_Loaded()
        {
            ContentFrame.Navigate(typeof(UserLoginPage), null, new DrillInNavigationTransitionInfo());
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoginWindow_Closed(object sender, WindowEventArgs args) { }
    }
}

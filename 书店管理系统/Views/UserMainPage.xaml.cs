﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using 书店管理系统.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserMainPage : Page
    {
        public UserMainViewModel ViewModel { get; set; } = App.GetService<UserMainViewModel>();

        public UserMainPage()
        {
            this.InitializeComponent();
        }

        private void UserInfoCard_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            userInfoCard.Background = App.Instance.Resources["LayerOnAcrylicFillColorDefaultBrush"] as Brush;
        }

        private void UserInfoCard_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            userInfoCard.Background = new SolidColorBrush(Colors.Transparent);
        }
    }
}

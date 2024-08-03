using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CoreServices.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.Appointments;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.UI.Core;
using WinUIEx;
using 书店管理系统.Contracts;
using 书店管理系统.Core.Structs;
using 书店管理系统.ViewModels;
using 书店管理系统.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindowViewModel ViewModel { get; set; }

        public MainWindow(LoginInfo loginInfo)
        {
            ViewModel = App.GetService<MainWindowViewModel>();
            ViewModel.SetLoginType(loginInfo.LoginType);
            ExtendsContentIntoTitleBar = true;
            this.InitializeComponent();
            SetTitleBar(WindowTitleBar);
            MainNavigationView.Loaded += (s, e) =>
            {
                switch (loginInfo.LoginType)
                {
                    case LoginType.Admin:
                        WindowTitleBar.SubTitle = "管理员界面";
                        MainNavigationView.ContentFrame.Navigate(typeof(AdminMainPage));
                        break;
                    case LoginType.User:
                        WindowTitleBar.SubTitle = "用户界面";
                        MainNavigationView.ContentFrame.Navigate(typeof(UserMainPage));
                        break;
                    default:
                        break;
                }
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

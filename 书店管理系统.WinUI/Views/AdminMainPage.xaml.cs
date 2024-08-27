using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using 书店管理系统.Core.Structs;
using 书店管理系统.WinUI.Controls.ContentDialogs;
using 书店管理系统.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminMainPage : Page
    {
        internal AdminMainViewModel ViewModel { get; set; } = App.GetService<AdminMainViewModel>();

        public AdminMainPage()
        {
            this.InitializeComponent();
        }

        private void OnUserListItemPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            FrameworkElement source = (FrameworkElement)sender;
            ViewModel.SelectedUserData = (UserData)(source).DataContext;
            ShowFlyout(source, userMenuFlyout, e);
        }

        private void OnBookListItemPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            FrameworkElement source = (FrameworkElement)sender;
            ViewModel.SelectedBookData = (BookData)source.DataContext;
            ShowFlyout(source, bookMenuFlyout, e);
        }

        private void OnRechargeListItemPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            FrameworkElement source = (FrameworkElement)sender;
            ViewModel.SelectedRechargeDealData = (RechargeDealData)source.DataContext;
            ShowFlyout(source, rechargeMenuFlyout, e);
        }

        private void OnDeleteUserMenuItemClicked(object sender, RoutedEventArgs e) =>
            ShowFlyout(userMenuFlyout.Target, ((FrameworkElement)sender).ContextFlyout);

        private void OnDeleteBookMenuItemClicked(object sender, RoutedEventArgs e) =>
            ShowFlyout(bookMenuFlyout.Target, ((FrameworkElement)sender).ContextFlyout);

        private void OnEditBookPriceMenuItemClicked(object sender, RoutedEventArgs e) =>
            ShowFlyout(bookMenuFlyout.Target, editBookPriceFlyout);

        private void OnEditBookPriceCancelButtonClicked(object sender, RoutedEventArgs e) => editBookPriceFlyout.Hide();

        private void OnSupplyBookAmountFlyoutItemClicked(object sender, RoutedEventArgs e) =>
            ShowFlyout(bookMenuFlyout.Target, supplyBookAmountFlyout);

        private void OnSupplyBookCancelButtonClicked(object sender, RoutedEventArgs e) => supplyBookAmountFlyout.Hide();

        private void OnPassRechargeMenuItemClicked(object sender, RoutedEventArgs e) =>
            ShowFlyout(rechargeMenuFlyout.Target, ((FrameworkElement)sender).ContextFlyout);

        private void OnDeleteRechargeMenuItemClicked(object sender, RoutedEventArgs e) =>
            ShowFlyout(rechargeMenuFlyout.Target, ((FrameworkElement)sender).ContextFlyout);

        private static void ShowFlyout(FrameworkElement target, FlyoutBase flyout, PointerRoutedEventArgs e)
        {
            var pointer = e.GetCurrentPoint(target);
            if (pointer.Properties.IsRightButtonPressed)
            {
                var pos = pointer.Position;
                ShowFlyout(target, flyout, pos);
                e.Handled = true;
            }
        }

        private static void ShowFlyout(FrameworkElement target, FlyoutBase flyout, Point pos) =>
            flyout.ShowAt(target, new FlyoutShowOptions() { Position = pos with { Y = pos.Y + 16, X = pos.X + 16 } });

        private static void ShowFlyout(FrameworkElement target, FlyoutBase flyout) => flyout.ShowAt(target);

        private async void OnStartAddNewUserButtonClicked(object sender, RoutedEventArgs e) =>
            await new RegisterUserContentDialog() { XamlRoot = XamlRoot }.ShowAsync();

        private async void OnStartAddNewBookButtonClicked(object sender, RoutedEventArgs e) =>
            await new AddBookContentDialog() { XamlRoot = XamlRoot }.ShowAsync();

        private async void OnEditUserDataButtonClicked(object sender, RoutedEventArgs e) =>
            await new EditUserDataContentDialog(ViewModel.SelectedUserData) { XamlRoot = XamlRoot }.ShowAsync();

        private async void OnEditBookDataButtonClicked(object sender, RoutedEventArgs e) =>
            await new EditBookDataContenDialog(ViewModel.SelectedBookData) { XamlRoot = XamlRoot }.ShowAsync();
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using 书店管理系统.Core;
using 书店管理系统.Core.Structs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.WinUI.Controls
{
    sealed partial class BookBuyCardModel : ObservableObject
    {
        [ObservableProperty, NotifyCanExecuteChangedFor(nameof(BuyCommand))]
        private BookData _bookData = new();

        partial void OnBookDataChanged(BookData? oldValue, BookData newValue)
        {
            void OnBookDataPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) =>
                BuyCommand.NotifyCanExecuteChanged();
            if (oldValue is not null)
                oldValue.PropertyChanged -= OnBookDataPropertyChanged;
            newValue.PropertyChanged += OnBookDataPropertyChanged;
        }

        [ObservableProperty, NotifyCanExecuteChangedFor(nameof(BuyCommand))]
        private double _count = 1;

        [RelayCommand(CanExecute = nameof(CanBuy))]
        private async Task Buy()
        {
            await LibrarySystemManager.Instance.BookService.BuyBookAsync(BookData.ISBN, (int)Count);
        }

        private bool CanBuy() => BookData.Amount >= Count;
    }

    public sealed partial class BookBuyCard : UserControl
    {
        //public static DependencyProperty BookDataProperty = DependencyProperty.Register(
        //    nameof(BookData),
        //    typeof(BookData),
        //    typeof(BookBuyCard),
        //    new PropertyMetadata(new())
        //);

        public BookData BookData
        {
            get => Model.BookData;
            set => Model.BookData = value;
        }

        BookBuyCardModel Model { get; } = new();

        public BookBuyCard()
        {
            this.InitializeComponent();
        }

        private void OnCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            buyCountInputFlyout.Hide();
        }
    }
}

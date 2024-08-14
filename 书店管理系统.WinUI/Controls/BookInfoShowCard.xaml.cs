using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Animations;
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
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.Controls
{
    public sealed partial class BookInfoShowCard : UserControl, INotifyPropertyChanged
    {
        public static DependencyProperty BookDataProperty { get; } =
            DependencyProperty.Register(
                "BookData",
                typeof(BookData),
                typeof(BookInfoShowCard),
                new PropertyMetadata(new BookData(), OnBookDataChanged)
            );

        public BookData BookData
        {
            get => (BookData)GetValue(BookDataProperty);
            set => SetValue(BookDataProperty, value);
        }

        private bool _isBookCanBought = true;
        private bool IsBookCanBought
        {
            get => _isBookCanBought;
            set
            {
                _isBookCanBought = value;
                PropertyChanged?.Invoke(this, new(nameof(IsBookCanBought)));
            }
        }

        public BookInfoShowCard()
        {
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private static void OnBookDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BookInfoShowCard source = (BookInfoShowCard)d;
            source.IsBookCanBought = source.BookData.IsValid;
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AnimationBuilder.Create().Translation(Axis.Y, to: -4).Start(this);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            AnimationBuilder.Create().Translation(Axis.Y, to: 0).Start(this);
        }

        private void BuyBookButtonClicked(object sender, RoutedEventArgs e)
        {
            IsBookCanBought = false;
        }
    }
}

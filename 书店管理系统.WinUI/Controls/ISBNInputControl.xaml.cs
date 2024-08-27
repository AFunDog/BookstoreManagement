using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.WinUI;
using global::Windows.System;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Serilog;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.WinUI.Controls
{
    public sealed partial class ISBNInputControl : UserControl, INotifyPropertyChanged
    {
        public static DependencyProperty ISBNProperty { get; } =
            DependencyProperty.Register("ISBN", typeof(long), typeof(ISBNInputControl), new PropertyMetadata(0L));
        public static DependencyProperty HeaderProperty { get; } =
            DependencyProperty.Register("Header", typeof(string), typeof(ISBNInputControl), new PropertyMetadata(string.Empty));

        public event PropertyChangedEventHandler? PropertyChanged;

        public long ISBN
        {
            get { return (long)GetValue(ISBNProperty); }
            set
            {
                SetValue(ISBNProperty, value);
                App.Logger.Debug("ISBN : {isbn} {pos}", ISBN, CursorPosition);
            }
        }
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set { SetValue(HeaderProperty, value); }
        }
        private int _cursorPosition = -1;
        private int CursorPosition
        {
            get => _cursorPosition;
            set
            {
                if (_cursorPosition is >= 0 and < ISBNLength)
                {
                    ((TextBlock)rootPanel.Children[_cursorPosition]).Foreground =
                        App.Instance.Resources["TextFillColorPrimaryBrush"] as Brush;
                }

                if (value is >= -1 and < ISBNLength)
                {
                    _cursorPosition = value;
                    PropertyChanged?.Invoke(this, new(nameof(CursorPosition)));
                }

                if (_cursorPosition is >= 0 and < ISBNLength - 1)
                {
                    ((TextBlock)rootPanel.Children[_cursorPosition]).Foreground =
                        App.Instance.Resources["AccentTextFillColorPrimaryBrush"] as Brush;
                }

                App.Logger.Debug("CursorPosition : {pos}", _cursorPosition);
            }
        }
        const int ISBNLength = 13;

        private ObservableCollection<int> ISBNString { get; } = new ObservableCollection<int>(new int[ISBNLength]);
        private Visibility HeaderVisibility => string.IsNullOrEmpty(Header) ? Visibility.Collapsed : Visibility.Visible;

        public ISBNInputControl()
        {
            this.InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitISBNString();
            ISBNString.CollectionChanged += (s, e) =>
            {
                var isbn = 0L;
                for (int i = 0; i <= CursorPosition; i++)
                {
                    isbn = isbn * 10 + ISBNString[i];
                }
                ISBN = isbn;
            };
        }

        private void InitISBNString()
        {
            var isbn = ISBN;
            var index = 0;
            while (isbn > 0 && index < ISBNLength)
            {
                ISBNString[index] = (int)(isbn % 10);
                isbn /= 10;
                index++;
            }
            CursorPosition = index - 1;
        }

        private readonly InputCursor _enterCursor = InputCursor.CreateFromCoreCursor(new(global::Windows.UI.Core.CoreCursorType.IBeam, 0));

        private void OnISBNPanelPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ProtectedCursor = _enterCursor;
        }

        private void OnISBNPanelPointerExited(object sender, PointerRoutedEventArgs e)
        {
            ProtectedCursor = null;
        }

        private void OnISBNPanelKeyDown(object sender, KeyRoutedEventArgs e)
        {
            App.Logger.Debug("KeyDown {key} {pos}", e.Key, CursorPosition);

            if (e.Key is >= VirtualKey.Number0 and <= VirtualKey.Number9 && CursorPosition < ISBNLength - 1)
            {
                CursorPosition++;
                ISBNString[CursorPosition] = (e.Key - VirtualKey.Number0);
            }
            else if (e.Key is >= VirtualKey.NumberPad0 and <= VirtualKey.NumberPad9 && CursorPosition < ISBNLength - 1)
            {
                CursorPosition++;
                ISBNString[CursorPosition] = (e.Key - VirtualKey.NumberPad0);
            }
            else if (e.Key is VirtualKey.Delete or VirtualKey.Back && CursorPosition >= 0)
            {
                CursorPosition--;
                ISBNString[CursorPosition + 1] = 0;
            }
        }

        private void OnISBNItemPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            App.Logger.Debug("OnISBNItemPointerPressed index : {index}", ((FrameworkElement)sender).DataContext);
        }

        private void OnISBNPanelPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            App.Logger.Debug("ISBN rootPanel TryFocus {res}", rootPanel.Focus(FocusState.Programmatic));
        }
    }
}

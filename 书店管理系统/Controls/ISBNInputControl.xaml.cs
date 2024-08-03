using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace 书店管理系统.Controls
{
    public sealed partial class ISBNInputControl : UserControl, INotifyPropertyChanged
    {
        public static DependencyProperty ISBNProperty { get; } =
            DependencyProperty.Register("ISBN", typeof(long), typeof(ISBNInputControl), new PropertyMetadata(0));
        public static DependencyProperty HeaderProperty { get; } =
            DependencyProperty.Register("Header", typeof(string), typeof(ISBNInputControl), new PropertyMetadata(string.Empty));

        public event PropertyChangedEventHandler? PropertyChanged;

        public long ISBN
        {
            get { return (long)GetValue(ISBNProperty); }
            set
            {
                SetValue(ISBNProperty, value);
                PropertyChanged?.Invoke(this, new(nameof(ISBNString)));
            }
        }
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set { SetValue(HeaderProperty, value); }
        }

        private IEnumerable<int> ISBNString => ISBN.ToString().Select(x => (int)x);
        private Visibility HeaderVisibility => string.IsNullOrEmpty(Header) ? Visibility.Collapsed : Visibility.Visible;

        public ISBNInputControl()
        {
            this.InitializeComponent();
        }

        private void OnISBNTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox source = (TextBox)sender;
            Log.Debug("TextChanged {Length}", source.Text.Length);
        }

        private void OnISBNSelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox source = (TextBox)sender;
        }
    }
}

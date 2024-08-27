using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.ComponentModel;
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

namespace 书店管理系统.WinUI.Controls.ContentDialogs
{
    sealed partial class AddBookContentDialogModel : ObservableObject
    {
        internal AddBookContentDialogModel()
        {
            BookData.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CanAdd));
            };
        }

        internal bool CanAdd => BookData.IsValid && !WaitForCommand;

        [ObservableProperty]
        private BookData _bookData = new();

        [ObservableProperty]
        private DateTimeOffset _defaultDate = DateTimeOffset.Now;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CanAdd))]
        private bool _waitForCommand = false;
    }

    public sealed partial class AddBookContentDialog : ContentDialog
    {
        AddBookContentDialogModel Model { get; } = new();

        public AddBookContentDialog()
        {
            this.InitializeComponent();
        }

        private async void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Model.WaitForCommand = true;
            if ((await LibrarySystemManager.Instance.BookService.AddBookAsync(Model.BookData)).IsSucceed is false)
            {
                args.Cancel = true;
            }
            Model.WaitForCommand = false;
        }
    }
}

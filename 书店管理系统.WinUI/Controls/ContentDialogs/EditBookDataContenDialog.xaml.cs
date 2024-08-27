using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.ComponentModel;
using Mapster;
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
    sealed partial class EditBookDataContenDialogModel(BookData orginalData) : ObservableObject
    {
        internal bool CanEdit =>
            !string.IsNullOrEmpty(NewBookName)
            && !string.IsNullOrEmpty(NewAuthor)
            && !string.IsNullOrEmpty(NewPublisher)
            && !WaitForCommand;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CanEdit))]
        private string _newBookName = orginalData.BookName;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CanEdit))]
        private string _newAuthor = orginalData.Author;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CanEdit))]
        private string _newPublisher = orginalData.Publisher;

        [ObservableProperty]
        private DateTimeOffset _newPublicationDate = orginalData.PublicationDate;

        [ObservableProperty]
        private string _newDescription = orginalData.Description;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CanEdit))]
        private bool _waitForCommand = false;
    }

    public sealed partial class EditBookDataContenDialog : ContentDialog
    {
        private readonly long _ISBN;
        EditBookDataContenDialogModel Model { get; }

        public EditBookDataContenDialog(BookData orginalData)
        {
            Model = new(orginalData);
            _ISBN = orginalData.ISBN;
            this.InitializeComponent();
        }

        private async void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Model.WaitForCommand = true;

            if (
                (
                    await LibrarySystemManager.Instance.BookService.EditBookDataAsync(
                        _ISBN,
                        Model.NewBookName,
                        Model.NewAuthor,
                        Model.NewPublisher,
                        Model.NewPublicationDate,
                        null,
                        Model.NewDescription
                    )
                ).IsSucceed
                is false
            )
            {
                args.Cancel = true;
            }
            Model.WaitForCommand = false;
        }
    }
}

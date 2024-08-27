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
    sealed partial class EditUserDataContentDialogModel(UserData orginalData) : ObservableObject
    {
        internal bool CanEdit => !string.IsNullOrEmpty(NewName) && !string.IsNullOrEmpty(NewPassword) && !WaitForCommand;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CanEdit))]
        private string _newName = orginalData.Name;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CanEdit))]
        private string _newPassword = orginalData.Password;

        [ObservableProperty]
        private Gender _newGender = orginalData.Gender;

        [ObservableProperty]
        private string _newPhone = orginalData.Phone;

        [ObservableProperty]
        private string _newAddress = orginalData.Address;

        [ObservableProperty]
        private string _newEmail = orginalData.Email;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CanEdit))]
        private bool _waitForCommand = false;
    }

    public sealed partial class EditUserDataContentDialog : ContentDialog
    {
        private readonly int _id;
        EditUserDataContentDialogModel Model { get; }

        public EditUserDataContentDialog(UserData orginalData)
        {
            _id = orginalData.Id;
            Model = new(orginalData);
            this.InitializeComponent();
        }

        private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Model.WaitForCommand = true;

            if (
                LibrarySystemManager
                    .Instance.UserService.EditUserBasicDataAsync(
                        _id,
                        Model.NewName,
                        Model.NewPassword,
                        Model.NewGender,
                        Model.NewPhone,
                        Model.NewAddress,
                        Model.NewEmail
                    )
                    .Result.IsSucceed
            ) { }
            else
            {
                args.Cancel = true;
            }

            Model.WaitForCommand = false;
        }
    }
}

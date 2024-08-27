using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
using 书店管理系统.Core.Structs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.WinUI.Controls.ContentDialogs
{
    sealed partial class RegisterUserContentDialogModel : ObservableObject
    {
        internal bool CanRegister => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password) && !WaitForCommand;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CanRegister))]
        private string _name = "";

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CanRegister))]
        private string _password = "";

        [ObservableProperty]
        private Gender _gender = Gender.Unknown;

        [ObservableProperty]
        private string _phone = "";

        [ObservableProperty]
        private string _address = "";

        [ObservableProperty]
        private string _email = "";

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CanRegister))]
        private bool _waitForCommand = false;
    }

    public sealed partial class RegisterUserContentDialog : ContentDialog
    {
        RegisterUserContentDialogModel Model { get; } = new();

        public RegisterUserContentDialog()
        {
            this.InitializeComponent();
        }

        private async void EditBasicInfoCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if (editBasicInfoCheckBox.IsChecked is null or false)
            {
                await AnimationBuilder.Create().Size(Axis.X, to: 0, layer: FrameworkLayer.Xaml).StartAsync(editPanel);
            }
            else
            {
                await AnimationBuilder.Create().Size(Axis.X, to: 224, layer: FrameworkLayer.Xaml).StartAsync(editPanel);
            }
        }

        private async void OnPrimaryButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Model.WaitForCommand = true;
            ActionResult res;
            if (editBasicInfoCheckBox.IsChecked is null or false)
            {
                res = await LibrarySystemManager.Instance.UserService.RegisterUserAsync(Model.Name, Model.Password);
            }
            else
            {
                res = await LibrarySystemManager.Instance.UserService.RegisterUserAsync(
                    Model.Name,
                    Model.Password,
                    Model.Gender,
                    Model.Phone,
                    Model.Address,
                    Model.Email
                );
            }
            if (!res.IsSucceed)
                args.Cancel = true;
            Model.WaitForCommand = false;
        }
    }
}

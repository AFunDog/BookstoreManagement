using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.Foundation;
using Windows.Foundation.Collections;
using 书店管理系统.Core.Structs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.Controls
{
    public sealed partial class UserInfoBorder : UserControl
    {
        public static DependencyProperty UserDatasProperty = DependencyProperty.Register(
            nameof(UserDatas),
            typeof(IReadOnlyCollection<UserData>),
            typeof(UserInfoBorder),
            new(new ObservableCollection<UserData>())
        );

        public IReadOnlyCollection<UserData> UserDatas
        {
            get => (IReadOnlyCollection<UserData>)GetValue(UserDatasProperty);
            set => SetValue(UserDatasProperty, value);
        }

        public UserInfoBorder()
        {
            this.InitializeComponent();
        }
    }
}

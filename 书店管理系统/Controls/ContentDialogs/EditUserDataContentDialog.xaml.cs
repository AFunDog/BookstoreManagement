using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
using Vanara.Extensions.Reflection;
using Windows.Foundation;
using Windows.Foundation.Collections;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.Controls.ContentDialogs
{
    public sealed partial class EditUserDataContentDialog : ContentDialog, INotifyPropertyChanged
    {
        public enum EditModeType
        {
            Edit,
            Register,
            //View
        }

        private double Increment => 0.01;
        private IReadOnlyCollection<CultureInfo> Cultures { get; } = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
        private PropertyChangedEventHandler? _propertyChanged;
        public EditModeType EditMode { get; set; } = EditModeType.Register;
        public UserData UserData { get; set; } =
            new(
                -1,
                string.Empty,
                string.Empty,
                Gender.Male,
                string.Empty,
                string.Empty,
                string.Empty,
                DateTime.Now,
                DateTime.Now,
                0,
                CultureInfo.CurrentCulture
            );

        public EditUserDataContentDialog()
        {
            this.InitializeComponent();
        }

        public EditUserDataContentDialog SetEditMode(UserData target)
        {
            UserData = target;
            EditMode = EditModeType.Edit;
            Title = "更新用户";
            return this;
        }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                UserData.PropertyChanged += value;
                _propertyChanged += value;
            }
            remove
            {
                UserData.PropertyChanged -= value;
                _propertyChanged -= value;
            }
        }

        private void OnLanguageComboxLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox source = (ComboBox)sender;
            var item = source.Items.FirstOrDefault(x =>
                x is CultureInfo culture && CultureInfo.CreateSpecificCulture(culture.Name).Name == CultureInfo.CurrentCulture.Name
            );
            source.SelectedItem = item;
        }

        private bool Non(bool? value) => !value ?? false;

        private void OnRandomCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            Span<char> chars = stackalloc char[26 * 2 + 10];
            int i = 0;
            for (int ptr = 0; ptr < 26; ptr++)
            {
                chars[i] = (char)('A' + ptr);
                chars[i + 1] = (char)('a' + ptr);
                i += 2;
            }
            for (int ptr = 0; ptr < 10; ptr++)
            {
                chars[i++] = (char)('0' + ptr);
            }
            Random random = new((int)DateTime.Now.Ticks);
            UserData.Password = new string(random.GetItems<char>(chars, 8));
            Log.Debug("生成的用户密码 {Password}", UserData.Password);
        }
    }
}

using System;
using System.Collections.Generic;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.WinUI.Controls
{
    public sealed partial class DecimalInputControl : NumberBox
    {
        public static DependencyProperty DecimalProperty { get; } =
            DependencyProperty.Register(nameof(Decimal), typeof(decimal), typeof(DecimalInputControl), new PropertyMetadata(0m));

        public decimal Decimal
        {
            get => (decimal)GetValue(DecimalProperty);
            set => SetValue(DecimalProperty, value);
        }

        private double Increment => 0.01;

        public DecimalInputControl()
        {
            this.InitializeComponent();
        }
    }
}

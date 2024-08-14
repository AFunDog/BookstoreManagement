using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;

namespace 书店管理系统.Behaviors
{
    /// <summary>
    /// 让用户在输完用户名称后按下回车可以跳转到密码输入
    /// </summary>
    public class UserNameTextBoxKeyBehavior : Behavior<TextBox>
    {
        public UIElement? Next { get; set; }

        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        private void AssociatedObject_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == global::Windows.System.VirtualKey.Enter)
            {
                if (!string.IsNullOrEmpty(AssociatedObject.Text))
                {
                    Next?.Focus(FocusState.Programmatic);
                }
                e.Handled = true;
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }
    }
}

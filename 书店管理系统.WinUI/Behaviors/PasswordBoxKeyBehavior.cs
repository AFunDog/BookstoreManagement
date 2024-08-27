using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;

namespace 书店管理系统.Behaviors
{
    /// <summary>
    /// 让用户在输入完成密码按下回车后可以执行登录按钮的操作
    /// </summary>
    public class PasswordBoxKeyBehavior : Behavior<PasswordBox>
    {
        public Button? TargetButton { get; set; }

        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        private void AssociatedObject_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (TargetButton is null)
                return;

            if (
                e.Key == global::Windows.System.VirtualKey.Enter
                && !string.IsNullOrEmpty(AssociatedObject.Password)
                && TargetButton.Command is not null
                && TargetButton.Command.CanExecute(null)
            )
            {
                TargetButton.Command.Execute(null);
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }
    }
}

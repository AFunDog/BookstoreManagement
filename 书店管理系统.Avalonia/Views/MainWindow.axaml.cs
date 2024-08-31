using Avalonia.Controls;
using Avalonia.Input;

namespace 书店管理系统.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.Pointer.Type == PointerType.Mouse)
            {
                this.BeginMoveDrag(e);
            }
        }
    }
}

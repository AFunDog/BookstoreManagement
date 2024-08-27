using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using 书店管理系统.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadingPage : Page, INotifyPropertyChanged
    {
        private double _loadingProgress = 0;
        public double LoadingProgress
        {
            get { return _loadingProgress; }
            set
            {
                _loadingProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadingProgress)));
            }
        }

        private string _loadingMessage = "";
        public string LoadingMessage
        {
            get { return _loadingMessage; }
            set
            {
                _loadingMessage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadingMessage)));
            }
        }

        public LoadingPage()
        {
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

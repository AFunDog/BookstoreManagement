﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using 书店管理系统.Core.Structs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace 书店管理系统.Controls.ContentDialogs
{
    public sealed partial class EditBookDataContentDialog : ContentDialog, INotifyPropertyChanged
    {
        public enum EditModeType
        {
            Edit,
            NewBook,
            //ViewBook,
        }

        private double Increment => 0.01;

        public event PropertyChangedEventHandler? PropertyChanged;
        public EditModeType EditMode { get; set; } = EditModeType.NewBook;
        public BookData BookData { get; set; } = new(1, string.Empty, string.Empty, string.Empty, default, [], string.Empty, 0, 0);

        public EditBookDataContentDialog()
        {
            this.InitializeComponent();
            PropertyChanged += EditBookDataContentDialog_PropertyChanged;
            BookData.PropertyChanged += EditBookDataContentDialog_PropertyChanged;
        }

        private void EditBookDataContentDialog_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Log.Debug(
                "PropertyChanged: {Property} {@value}",
                e.PropertyName,
                sender!.GetType().GetProperty(e.PropertyName!)!.GetValue(sender)
            );
        }

        public EditBookDataContentDialog SetEditMode(BookData target)
        {
            BookData = target;
            EditMode = EditModeType.Edit;
            Title = "编辑书籍信息";
            PropertyChanged?.Invoke(this, new(nameof(BookData)));
            return this;
        }
    }
}

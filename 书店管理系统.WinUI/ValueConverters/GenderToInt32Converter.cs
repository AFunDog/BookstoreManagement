using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.WinUI.ValueConverters
{
    internal sealed class GenderToInt32Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Gender)value;
        }
    }
}

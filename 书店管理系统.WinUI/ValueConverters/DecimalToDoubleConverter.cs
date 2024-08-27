using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace 书店管理系统.WinUI.ValueConverters
{
    internal sealed class DecimalToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return decimal.ToDouble((decimal)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return decimal.TryParse(value.ToString(), out var result) ? result : 0;
        }
    }
}

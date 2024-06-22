using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.Contracts
{
    internal interface IDragMove
    {
        void Register(UIElement element, AppWindow window);
        void Unregister(UIElement uIElement);
    }
}

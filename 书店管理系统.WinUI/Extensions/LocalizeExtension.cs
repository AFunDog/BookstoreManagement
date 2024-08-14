
using CoreServices.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.Extensions
{
    public static class LocalizeExtension
    {
        public static string Localize(this string uid) => App.GetService<ILocalizeService>().Localize(uid);
    }
}

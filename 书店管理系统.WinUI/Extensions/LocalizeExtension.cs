using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace 书店管理系统.Extensions
{
    public static class LocalizeExtension
    {
        public static string Localize(this string uid) => App.Instance.ServiceProvider.GetRequiredService<ILocalizeService>().Localize(uid);
    }
}

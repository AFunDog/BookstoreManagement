using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Animations;
using CoreServices.Localization;
using Mapster;
using Serilog;
using WinUIEx;
using WinUIEx.Messaging;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;
using 书店管理系统.Windows;

namespace 书店管理系统
{
    internal sealed class LibrarySystemProcess : ILibrarySystemProcess
    {
        public async Task StartLoadingAsync()
        {
            await Task.WhenAll(InitLocalizeServiceAsync());
        }

        /// <summary>
        /// 加载本地化文件
        /// </summary>
        /// <returns></returns>
        private async Task InitLocalizeServiceAsync()
        {
            ILocalizeService localizeService = App.GetService<ILocalizeService>();
            Log.Debug("{0} 开始执行", nameof(InitLocalizeServiceAsync));
            string LocDir = @"Localizations";
            if (!Directory.Exists(LocDir))
                Directory.CreateDirectory(LocDir);
            foreach (var locfile in Directory.GetFiles(LocDir, "*.json"))
            {
                try
                {
                    CultureInfo culture = new(Path.GetFileNameWithoutExtension(locfile));
                    var locs = JsonSerializer.Deserialize<Dictionary<string, string>>(File.OpenRead(locfile));
                    if (locs is null)
                        continue;
                    foreach ((var uid, var text) in locs)
                    {
                        localizeService.SetLocalization(culture, uid, text);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    continue;
                }
            }
            await Task.CompletedTask;
        }

        public async Task StartLoginAsync()
        {
            if (App.Instance.LoginWindow is null)
                return;
            App.Instance.LoginWindow.Show();
            App.Instance.LoginWindow.ToLoginState();
            App.Instance.LoginWindow.DispatcherQueue.TryEnqueue(async () =>
            {
                await AnimationBuilder.Create().Opacity(to: 1).StartAsync(App.Instance.LoginWindow!.Content);
            });

            await Task.CompletedTask;
        }

        public async Task ToAdminModeAsync()
        {
            await TryLoginToAsync(new(LoginType.Admin, 0));
        }

        public async Task ToUserModeAsync()
        {
            await TryLoginToAsync(new(LoginType.User, App.GetService<IUserService>().LoginUser!.Id));
        }

        private async Task TryLoginToAsync(LoginInfo message)
        {
            await Task.Delay(1000);
            if (App.Instance.LoginWindow is null)
                return;
            App.Instance.LoginWindow.DispatcherQueue.TryEnqueue(async () =>
            {
                await AnimationBuilder.Create().Opacity(to: 0).StartAsync(App.Instance.LoginWindow!.Content);
                App.Instance.LoginWindow!.Hide();
                App.Instance.MainWindow = new MainWindow(message);
                App.Instance.MainWindow.Activate();
                AnimationBuilder
                    .Create()
                    .Opacity(from: 0, to: 1, duration: TimeSpan.FromMilliseconds(400))
                    .Start(App.Instance.MainWindow.Content);
            });
        }

        public async Task ExitAdminModeAsync()
        {
            await Task.Yield();
            App.Instance.MainWindow!.ExitWithShutdownApp = false;
            App.Instance.MainWindow.Close();
            App.Instance.MainWindow!.ExitWithShutdownApp = true;
        }

        public async Task ExitUserModeAsync()
        {
            await Task.Yield();
            App.Instance.MainWindow!.ExitWithShutdownApp = false;
            App.Instance.MainWindow.Close();
            App.Instance.MainWindow!.ExitWithShutdownApp = true;
        }

        public async Task ExitSystemAsync()
        {
            await Task.CompletedTask;
        }
    }
}

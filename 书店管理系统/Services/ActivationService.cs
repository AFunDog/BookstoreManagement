using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CoreServices.Localization;
using Microsoft.VisualStudio.Threading;
using Serilog;
using 书店管理系统.Contracts;
using 书店管理系统.Core.Contracts;

namespace 书店管理系统.Services
{
    internal class ActivationService(
        ILocalizeService localizeService,
        IUserDataProvider userDataProvider,
        IBookDataProvider bookDataProvider
    ) : IActivationService
    {
        public async Task LaunchingActivateAsync(IProgress<bool>? progress = null, object? args = null)
        {
            await Task.Yield().ConfigureAwait(false);
            Log.Debug("{0} 开始执行", nameof(LaunchingActivateAsync));
            Stopwatch sw = Stopwatch.StartNew();
            await Task.WhenAll(InitLocalizeServiceAsync(), LoadUserDatasAsync(), LoadBookDatasAsync()).ConfigureAwait(false);
            sw.Stop();
            Log.Debug("{0} 结束执行 用时 {1}ms", nameof(LaunchingActivateAsync), sw.ElapsedMilliseconds);
            progress?.Report(true);
        }

        public async Task LaunchedActivateAsync(IProgress<bool>? progress = null, object? args = null)
        {
            await Task.Yield();
            Log.Debug("{0} 开始执行", nameof(LaunchedActivateAsync));
            Stopwatch sw = Stopwatch.StartNew();
            List<Task> tasks = [];
            await Task.WhenAll(tasks);
            sw.Stop();
            Log.Debug("{0} 结束执行 用时 {1}ms", nameof(LaunchedActivateAsync), sw.ElapsedMilliseconds);
            progress?.Report(true);
        }

        /// <summary>
        /// 加载本地化文件
        /// </summary>
        /// <returns></returns>
        private async Task InitLocalizeServiceAsync()
        {
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

        /// <summary>
        /// 加载用户数据
        /// </summary>
        /// <returns></returns>
        private async Task LoadUserDatasAsync()
        {
            Log.Debug("{0} 开始执行", nameof(LoadUserDatasAsync));
            await userDataProvider.LoadUserDatasAsync();
        }

        /// <summary>
        /// 加载书籍数据
        /// </summary>
        /// <returns></returns>
        private async Task LoadBookDatasAsync()
        {
            Log.Debug("{0} 开始执行", nameof(LoadBookDatasAsync));
            await bookDataProvider.LoadBookDatasAsync();
        }
    }
}

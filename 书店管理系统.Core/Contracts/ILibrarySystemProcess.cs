using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Contracts
{
    public interface ILibrarySystemProcess
    {
        Task StartLoadingAsync();
        Task StartLoginAsync();
        Task ToAdminModeAsync();
        Task ToUserModeAsync();
        Task ExitAdminModeAsync();
        Task ExitUserModeAsync();
        Task ExitSystemAsync();

        internal class EmptyLibrarySystemProcess : ILibrarySystemProcess
        {
            public Task StartLoadingAsync() => Task.CompletedTask;

            public Task StartLoginAsync() => Task.CompletedTask;

            public Task ToAdminModeAsync() => Task.CompletedTask;

            public Task ToUserModeAsync() => Task.CompletedTask;

            public Task ExitAdminModeAsync() => Task.CompletedTask;

            public Task ExitUserModeAsync() => Task.CompletedTask;

            public Task ExitSystemAsync() => Task.CompletedTask;
        }
    }
}

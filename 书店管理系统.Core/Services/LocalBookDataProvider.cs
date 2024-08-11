using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Serilog;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Services
{
    public sealed class LocalBookDataProvider : IBookDataProvider
    {
        private const string filePath = ".bookDatas";

        private readonly ObservableCollection<BookData> _bookDatas = [];

        //[
        //    new(
        //        978_7_111_64217_6,
        //        "离散数学及其应用",
        //        "[美]肯尼思",
        //        "机械工业出版社",
        //        DateTime.Parse("2020-1"),
        //        ["离散数学", "高等学校", "教材"],
        //        "本书是经典的离散数学教材",
        //        new("zh-cn"),
        //        79,
        //        1
        //    )
        //];

        public IReadOnlyCollection<BookData> BookDatas => _bookDatas;

        public async Task<ActionResult> LoadBookDatasAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
                return ActionResult
                    .Error($"文件 {filePath} 不存在")
                    .TryReportResult($"读取书籍数据错误 文件 {filePath} 不存在", null, LibrarySystemManager.Logger);
            using FileStream fileStream = File.OpenRead(filePath);
            foreach (var data in await MessagePackSerializer.DeserializeAsync<IEnumerable<BookData>>(fileStream))
            {
                _bookDatas.Add(data);
            }
            return ActionResult.Success().TryReportResult("读取书籍数据完毕", null, LibrarySystemManager.Logger);
        }

        public async Task<ActionResult> SaveBookDatasAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
            using FileStream fileStream = File.OpenWrite(filePath);
            await MessagePackSerializer.SerializeAsync(fileStream, _bookDatas);
            return ActionResult.Success().TryReportResult("保存书籍数据完毕", null, LibrarySystemManager.Logger);
        }

        public async Task<ActionResult> TryAddBookDataAsync(BookData bookData, CancellationToken cancellationToken = default)
        {
            _bookDatas.Add(bookData);
            return ActionResult.Success();
        }

        public async Task<(BookData? bookData, ActionResult result)> TryGetBookDataAsync(
            long ISBN,
            CancellationToken cancellationToken = default
        )
        {
            var bookData = _bookDatas.FirstOrDefault(b => b.ISBN == ISBN);
            return bookData is not null ? new(bookData, ActionResult.Success()) : new(null, ActionResult.Error($"未找到ISBN 为 {ISBN} 的书籍"));
        }

        public async Task<ActionResult> TryRemoveBookDataAsync(long ISBN, CancellationToken cancellationToken = default)
        {
            if (_bookDatas.FirstOrDefault(x => x.ISBN == ISBN) is BookData bookData)
            {
                _bookDatas.Remove(bookData);
                return new(ResultType.Success, string.Empty);
            }
            else
            {
                return new(ResultType.Error, $"未找到ISBN 为 {ISBN} 的书籍");
            }
        }
    }
}

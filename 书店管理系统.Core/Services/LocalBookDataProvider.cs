using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
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

        public ActionResult LoadBookDatas()
        {
            if (!File.Exists(filePath))
                return new(ResultType.Error, $"文件 {filePath} 不存在");
            foreach (var data in MessagePackSerializer.Deserialize<IEnumerable<BookData>>(File.ReadAllBytes(filePath)))
            {
                _bookDatas.Add(data);
            }
            return new(ResultType.OK, string.Empty);
        }

        public async ValueTask<ActionResult> LoadBookDatasAsync()
        {
            if (!File.Exists(filePath))
                return new(ResultType.Error, $"文件 {filePath} 不存在");
            using FileStream fileStream = File.OpenRead(filePath);
            foreach (var data in await MessagePackSerializer.DeserializeAsync<IEnumerable<BookData>>(fileStream))
            {
                _bookDatas.Add(data);
            }
            return new(ResultType.OK, string.Empty);
        }

        public ActionResult SaveBookDatas()
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
            File.WriteAllBytes(filePath, MessagePackSerializer.Serialize(_bookDatas));
            return new(ResultType.OK, string.Empty);
        }

        public async ValueTask<ActionResult> SaveBookDatasAsync()
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
            using FileStream fileStream = File.OpenWrite(filePath);
            await MessagePackSerializer.SerializeAsync(fileStream, _bookDatas);
            return new(ResultType.OK, string.Empty);
        }

        public ActionResult TryAddBookData(BookData bookData)
        {
            _bookDatas.Add(bookData);
            return new(ResultType.OK, string.Empty);
        }

        public ActionResult TryGetBookData(long ISBN, [NotNullWhen(true)] out BookData? bookData)
        {
            bookData = _bookDatas.FirstOrDefault(b => b.ISBN == ISBN);
            return bookData is not null ? new(ResultType.OK, string.Empty) : new(ResultType.Error, $"未找到ISBN 为 {ISBN} 的书籍");
        }

        public ActionResult TryRemoveBookData(long ISBN)
        {
            if (_bookDatas.FirstOrDefault(x => x.ISBN == ISBN) is BookData bookData)
            {
                _bookDatas.Remove(bookData);
                return new(ResultType.OK, string.Empty);
            }
            else
            {
                return new(ResultType.Error, $"未找到ISBN 为 {ISBN} 的书籍");
            }
        }
    }
}

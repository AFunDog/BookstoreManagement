using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Contracts
{
    public interface IBookDataProvider
    {
        IReadOnlyCollection<BookData> BookDatas { get; }
        ActionResult LoadBookDatas();
        ValueTask<ActionResult> LoadBookDatasAsync();
        ActionResult SaveBookDatas();
        ValueTask<ActionResult> SaveBookDatasAsync();
        ActionResult TryAddBookData(BookData bookData);
        ActionResult TryRemoveBookData(long ISBN);
        ActionResult TryGetBookData(long ISBN, [NotNullWhen(true)] out BookData? bookData);
    }
}

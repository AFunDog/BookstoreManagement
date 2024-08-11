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
        Task<ActionResult> LoadBookDatasAsync(CancellationToken cancellationToken = default);
        Task<ActionResult> SaveBookDatasAsync(CancellationToken cancellationToken = default);
        Task<ActionResult> TryAddBookDataAsync(BookData bookData, CancellationToken cancellationToken = default);
        Task<ActionResult> TryRemoveBookDataAsync(long ISBN, CancellationToken cancellationToken = default);
        Task<(BookData? bookData, ActionResult result)> TryGetBookDataAsync(long ISBN, CancellationToken cancellationToken = default);
    }
}

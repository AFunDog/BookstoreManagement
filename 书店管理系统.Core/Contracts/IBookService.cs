using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Contracts
{
    public interface IBookService
    {
        event Action<BookData>? BookDataChanged;
        IReadOnlyCollection<BookData> BookDatas { get; }

        Task<ActionResult> SaveBookDatasAsync(CancellationToken cancellationToken = default);
        Task<ActionResult> TryAddBook(BookData book, CancellationToken cancellationToken = default);
        Task<ActionResult> TryEditBookAmount(long ISBN, int addAmount, CancellationToken cancellationToken = default);
        Task<ActionResult> RemoveBookAsync(BookData book, CancellationToken cancellationToken = default);
        Task<ActionResult> TryGetBookDataAsync(long ISBN, out BookData? bookData);
    }
}

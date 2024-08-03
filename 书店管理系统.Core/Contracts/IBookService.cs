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

        ActionResult AddBook(BookData book);
        ActionResult EditBookAmount(long ISBN, int addAmount);
        ActionResult RemoveBook(BookData book);
        ActionResult TryGetBookData(long ISBN, out BookData? bookData);
    }
}

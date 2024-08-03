using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.Core.Services
{
    public class BookService : IBookService
    {
        private readonly IBookDataProvider _bookDataProvider;

        public IReadOnlyCollection<BookData> BookDatas => _bookDataProvider.BookDatas;

        public event Action<BookData>? BookDataChanged;

        public BookService(IBookDataProvider bookDataProvider)
        {
            _bookDataProvider = bookDataProvider;
        }

        public ActionResult AddBook(BookData book)
        {
            if (!book.IsBookDataValid)
            {
                return new ActionResult(ResultType.Error, "书籍数据无效");
            }
            if (_bookDataProvider.TryGetBookData(book.ISBN, out var bookData).ResultType == ResultType.OK)
            {
                return new ActionResult(ResultType.Error, $"此 {book.ISBN} 的书籍已存在");
            }
            _bookDataProvider.TryAddBookData(book);
            return new ActionResult(ResultType.OK, string.Empty);
        }

        public ActionResult RemoveBook(BookData book)
        {
            return _bookDataProvider.TryRemoveBookData(book.ISBN);
        }

        public ActionResult EditBookAmount(long ISBN, int addAmount)
        {
            if (_bookDataProvider.TryGetBookData(ISBN, out var bookData).ResultType == ResultType.OK && bookData is not null)
            {
                if (bookData.Amount + addAmount < 0)
                {
                    return new ActionResult(ResultType.Error, "库存书籍数量不够");
                }
                bookData.Amount += addAmount;
                return new ActionResult(ResultType.OK, string.Empty);
            }
            return new ActionResult(ResultType.Error, $"此 {ISBN} 的书籍不存在");
        }

        public ActionResult TryGetBookData(long ISBN, out BookData? bookData)
        {
            return _bookDataProvider.TryGetBookData(ISBN, out bookData);
        }
    }
}

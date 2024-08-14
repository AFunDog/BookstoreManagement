using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CoreLibrary.Core.BasicObjects;
using Mapster;
using MessagePack;
using MessagePack.Formatters;
using 书店管理系统.Core.Others.MessagePackFormatters;

namespace 书店管理系统.Core.Structs
{
    public interface IReadOnlyBookData : INotifyPropertyChanged
    {
        long ISBN { get; }
        string BookName { get; }
        string Author { get; }
        string Publisher { get; }
        DateTime PublicationDate { get; }
        string[] Category { get; }
        string Description { get; }
        decimal Price { get; }
        int Amount { get; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public sealed partial class BookData(
        long ISBN,
        string bookName,
        string author,
        string publisher,
        DateTime publicationDate,
        string[] category,
        string description,
        decimal price,
        int amount
    ) : DataBaseModel, IReadOnlyBookData
    {
        [IgnoreDataMember]
        [AdaptIgnore]
        public bool IsValid =>
            ISBN != 0
            && !string.IsNullOrEmpty(BookName)
            && !string.IsNullOrEmpty(Author)
            && !string.IsNullOrEmpty(Publisher)
            && Price >= 0
            && Amount >= 0;

        public BookData()
            : this(0, string.Empty, string.Empty, string.Empty, default, [], string.Empty, 0, 0) { }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private long _ISBN = ISBN;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string _bookName = bookName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string _author = author;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string _publisher = publisher;

        [ObservableProperty]
        private DateTime _publicationDate = publicationDate;

        [ObservableProperty]
        private string[] _category = category;

        [ObservableProperty]
        private string _description = description;

        // TODO 存在严重的数据转换问题，解决方法使用TypeAdapterConfig
        //[ObservableProperty]
        //[property: Key(7), MessagePackFormatter(typeof(CultureInfoFormatter))]
        //private CultureInfo _language = language;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private decimal _price = price;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private int _amount = amount;
    }
}

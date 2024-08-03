using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MessagePack;
using MessagePack.Formatters;
using 书店管理系统.Core.Others.MessagePackFormatters;

namespace 书店管理系统.Core.Structs
{
    [MessagePackObject]
    public sealed partial class BookData(
        long ISBN,
        string bookName,
        string author,
        string publisher,
        DateTime publicationDate,
        string[] category,
        string description,
        CultureInfo language,
        decimal price,
        int amount
    ) : ObservableObject
    {
        [IgnoreDataMember]
        public bool IsBookDataValid =>
            ISBN != 0
            && !string.IsNullOrEmpty(BookName)
            && !string.IsNullOrEmpty(Author)
            && !string.IsNullOrEmpty(Publisher)
            && Price >= 0
            && Amount >= 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBookDataValid))]
        [property: Key(0)]
        private long _ISBN = ISBN;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBookDataValid))]
        [property: Key(1)]
        private string _bookName = bookName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBookDataValid))]
        [property: Key(2)]
        private string _author = author;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBookDataValid))]
        [property: Key(3)]
        private string _publisher = publisher;

        [ObservableProperty]
        [property: Key(4)]
        private DateTime _publicationDate = publicationDate;

        [ObservableProperty]
        [property: Key(5)]
        private string[] _category = category;

        [ObservableProperty]
        [property: Key(6)]
        private string _description = description;

        [ObservableProperty]
        [property: Key(7), MessagePackFormatter(typeof(CultureInfoFormatter))]
        private CultureInfo _language = language;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBookDataValid))]
        [property: Key(8)]
        private decimal _price = price;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBookDataValid))]
        [property: Key(9)]
        private int _amount = amount;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CoreLibrary.Core.BasicObjects;
using Mapster;
using MessagePack;

namespace 书店管理系统.Core.Structs
{
    public interface IReadOnlyBookDealData : INotifyPropertyChanged
    {
        int Id { get; }
        DateTimeOffset DealTime { get; }
        int Uid { get; }
        long ISBN { get; }
        decimal Price { get; }
        int Amount { get; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public sealed partial class BookDealData(int id, DateTimeOffset dealTime, int uid, long ISBN, decimal price, int amount)
        : DataBaseModel,
            IReadOnlyBookDealData
    {
        [IgnoreMember]
        [AdaptIgnore]
        public bool IsValid => Id != -1 && Uid != -1 && ISBN != -1 && Price != -1 && Amount != -1;

        public BookDealData()
            : this(-1, default, -1, 0, -1, -1) { }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private int _id = id;

        [ObservableProperty]
        private DateTimeOffset _dealTime = dealTime;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private int _uid = uid;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private long _ISBN = ISBN;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private decimal _price = price;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private int _amount = amount;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CoreLibrary.Core.BasicObjects;
using Mapster;
using MessagePack;

namespace 书店管理系统.Core.Structs
{
    public interface IReadOnlyRechargeDealData
    {
        int Id { get; }
        DateTimeOffset DealCreateTime { get; }
        DateTimeOffset DealPassTime { get; }
        int Uid { get; }
        decimal AddMoney { get; }
        bool IsPass { get; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public sealed partial class RechargeDealData(
        int id,
        DateTimeOffset dealCreateTime,
        DateTimeOffset dealPassTime,
        int uid,
        decimal addMoney,
        bool isPass
    ) : DataBaseModel, IReadOnlyRechargeDealData
    {
        [IgnoreMember]
        [AdaptIgnore]
        public bool IsValid => Id != -1 && Uid != -1 && AddMoney != 0;

        public RechargeDealData()
            : this(-1, default, default, -1, 0, false) { }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private int _id = id;

        [ObservableProperty]
        private DateTimeOffset _dealCreateTime = dealCreateTime;

        [ObservableProperty]
        private DateTimeOffset _dealPassTime = dealPassTime;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private int _uid = uid;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private decimal _addMoney = addMoney;

        [ObservableProperty]
        private bool _isPass = isPass;
    }
}

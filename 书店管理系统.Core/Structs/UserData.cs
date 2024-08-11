using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CoreLibrary.Core.BasicObjects;
using Mapster;
using MessagePack;
using 书店管理系统.Core.Others.MessagePackFormatters;

namespace 书店管理系统.Core.Structs
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    [MessagePackObject]
    public sealed partial class UserData(
        int id,
        string name,
        string password,
        Gender gender,
        string phone,
        string address,
        string email,
        DateTime createTime,
        DateTime updateTime,
        decimal account
    ) : DataBaseModel
    {
        [IgnoreMember]
        [AdaptIgnore]
        public bool IsUserDataValid =>
            !string.IsNullOrEmpty(Name)
            && !string.IsNullOrEmpty(Password)
            && !string.IsNullOrEmpty(Phone)
            && !string.IsNullOrEmpty(Address)
            && !string.IsNullOrEmpty(Email);

        public UserData()
            : this(-1, string.Empty, string.Empty, Gender.Other, string.Empty, string.Empty, string.Empty, default, default, 0) { }

        [ObservableProperty]
        [property: Key(0)]
        private int _id = id;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUserDataValid))]
        [property: Key(1)]
        private string _name = name;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUserDataValid))]
        [property: Key(2)]
        private string _password = password;

        [ObservableProperty]
        [property: Key(3)]
        private Gender _gender = gender;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUserDataValid))]
        [property: Key(4)]
        private string _phone = phone;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUserDataValid))]
        [property: Key(5)]
        private string _address = address;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUserDataValid))]
        [property: Key(6)]
        private string _email = email;

        [ObservableProperty]
        [property: Key(7)]
        private DateTime _createTime = createTime;

        [ObservableProperty]
        [property: Key(8)]
        private DateTime _updateTime = updateTime;

        [ObservableProperty]
        [property: Key(9)]
        private decimal _account = account;

        // TODO 存在严重的数据转换问题，暂时无法解决
        //[ObservableProperty]
        //[property: Key(10), MessagePackFormatter(typeof(CultureInfoFormatter))]
        //private CultureInfo _language = language;
    }
}

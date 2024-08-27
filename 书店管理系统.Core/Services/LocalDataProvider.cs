using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Core.BasicObjects;
using MessagePack;
using 书店管理系统.Core.Contracts;
using 书店管理系统.Core.Structs.EventArgs;

namespace 书店管理系统.Core.Services
{
    internal class LocalDataProvider<DataType> : DisposableObject, IDataProvider<DataType>
        where DataType : notnull, INotifyPropertyChanged
    {
        public event Action<DataType, DataChangedEventArgs>? DataChanged;
        private ObservableCollection<DataType>? _datas;

        readonly string filePath = $"{typeof(DataType).Name}.dat";

        public async Task LoadDataAsync(IProgress<ObservableCollection<DataType>> progress, CancellationToken cancellationToken = default)
        {
            var res = await LoadDataAsync(cancellationToken);
            progress.Report(res);
        }

        private async ValueTask<ObservableCollection<DataType>> LoadDataAsync(CancellationToken cancellationToken = default)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using FileStream fs = File.OpenRead(filePath);
                    _datas = await MessagePackSerializer.DeserializeAsync<ObservableCollection<DataType>>(
                        fs,
                        cancellationToken: cancellationToken
                    );
                }
                catch (Exception e)
                {
                    LibrarySystemManager.Logger.Warning(e, "读取数据异常");
                    _datas = [];
                }
            }
            else
            {
                _datas = [];
            }
            _datas.CollectionChanged += OnCollectionChanged;
            foreach (var data in _datas)
            {
                data.PropertyChanged += OnPropertyChanged;
            }
            return _datas;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            DataChanged?.Invoke((DataType)sender!, new(ChangeType.Edit, e.PropertyName));
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is null)
                        break;
                    foreach (DataType item in e.NewItems)
                    {
                        item.PropertyChanged += OnPropertyChanged;
                        DataChanged?.Invoke(item, new(ChangeType.Add, null));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is null)
                        break;
                    foreach (DataType item in e.OldItems)
                    {
                        item.PropertyChanged -= OnPropertyChanged;
                        DataChanged?.Invoke(item, new(ChangeType.Remove, null));
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems is not null)
                    {
                        foreach (DataType item in e.OldItems)
                        {
                            item.PropertyChanged -= OnPropertyChanged;
                            DataChanged?.Invoke(item, new(ChangeType.Remove, null));
                        }
                    }
                    if (e.NewItems is not null)
                    {
                        foreach (DataType item in e.NewItems)
                        {
                            item.PropertyChanged += OnPropertyChanged;
                            DataChanged?.Invoke(item, new(ChangeType.Add, null));
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException("不支持的操作");
            }
        }

        public async Task SaveDataAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_datas is null)
                    throw new InvalidOperationException("请先读取数据再保存，或者不要在释放资源后再保存");
                using FileStream fs = File.OpenWrite(filePath);
                await MessagePackSerializer.SerializeAsync(fs, _datas, cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                LibrarySystemManager.Logger.Error(e, "保存用户数据异常");
            }
        }

        protected override void DisposeManagedResource()
        {
            if (_datas is not null)
            {
                _datas.CollectionChanged -= OnCollectionChanged;
                foreach (var data in _datas)
                {
                    data.PropertyChanged -= OnPropertyChanged;
                }
                _datas.Clear();
                _datas = null;
            }
        }

        protected override void DisposeUnmanagedResource() { }

        protected override void OnDisposed() { }
    }
}

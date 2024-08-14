using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Structs.EventArgs;

namespace 书店管理系统.Core.Contracts
{
    internal interface IDataProvider<DataType>
        where DataType : notnull, INotifyPropertyChanged
    {
        /// <summary>
        /// 当数据发生变化时触发
        /// </summary>
        event Action<DataType, DataChangedEventArgs>? DataChanged;

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="progress">加载完成后返回的数据</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task LoadDataAsync(IProgress<ObservableCollection<DataType>> progress, CancellationToken cancellationToken = default);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveDataAsync(CancellationToken cancellationToken = default);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.Core.Structs.EventArgs
{
    public enum ChangeType
    {
        Add,
        Remove,
        Edit
    }

    public sealed record DataChangedEventArgs(ChangeType ChangeType, string? ChangedPropertyName);
}

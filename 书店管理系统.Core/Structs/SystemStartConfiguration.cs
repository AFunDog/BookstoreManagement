using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 书店管理系统.Core.Contracts;

namespace 书店管理系统.Core.Structs
{
    public record SystemStartConfiguration(ILibrarySystemProcess LibrarySystemProcess, bool UseSQLDataSource = false);
}

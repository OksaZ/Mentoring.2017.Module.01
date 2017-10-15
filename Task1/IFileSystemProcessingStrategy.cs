using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.EventArgs;

namespace Task1
{
    public interface IFileSystemProcessingStrategy
    {
        ActionType ProcessItemFinded<TItemInfo, TArgs>(
            TItemInfo itemInfo,
            Func<FileSystemInfo, bool> filter,
            EventHandler<TArgs> itemFinded,
            EventHandler<TArgs> filteredItemFinded,
            Action<EventHandler<TArgs>, ItemFindedEventArgs<TItemInfo>> eventEmitter)
            where TItemInfo : FileSystemInfo
            where TArgs : ItemFindedEventArgs<TItemInfo>;
    }
}

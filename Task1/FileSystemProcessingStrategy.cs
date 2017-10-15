using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.EventArgs;

namespace Task1
{
    public class FileSystemProcessingStrategy : IFileSystemProcessingStrategy
    {
        public ActionType ProcessItemFinded<TItemInfo, TArgs>(
            TItemInfo itemInfo,
            Func<FileSystemInfo, bool> filter,
            EventHandler<TArgs> itemFinded,
            EventHandler<TArgs> filteredItemFinded,
            Action<EventHandler<TArgs>, ItemFindedEventArgs<TItemInfo>> eventEmitter)
            where TItemInfo : FileSystemInfo
            where TArgs : ItemFindedEventArgs<TItemInfo>
        {
            ItemFindedEventArgs<TItemInfo> args = new ItemFindedEventArgs<TItemInfo>
            {
                FindedItem = itemInfo,
                ActionType = ActionType.ContinueSearch
            };
            eventEmitter(itemFinded, args);

            if (args.ActionType != ActionType.ContinueSearch || filter == null)
            {
                return args.ActionType;
            }

            if (filter(itemInfo))
            {
                args = new ItemFindedEventArgs<TItemInfo>
                {
                    FindedItem = itemInfo,
                    ActionType = ActionType.ContinueSearch
                };
                eventEmitter(filteredItemFinded, args);
                return args.ActionType;
            }

            return ActionType.SkipElement;
        }
    }
}

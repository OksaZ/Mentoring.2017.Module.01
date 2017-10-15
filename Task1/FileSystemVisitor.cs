using System;
using System.Collections.Generic;
using System.IO;
using Task1.EventArgs;

namespace Task1
{
    public class FileSystemVisitor
    {
        private readonly DirectoryInfo _startDirectory;
        private readonly Func<FileSystemInfo, bool> _filter;

        public FileSystemVisitor(string path, Func<FileSystemInfo, bool> filter = null)
            : this(new DirectoryInfo(path), filter) { }

        public FileSystemVisitor(DirectoryInfo startDirectory, Func<FileSystemInfo, bool> filter = null)
        {
            _startDirectory = startDirectory;
            _filter = filter;
        }

        public event EventHandler<StartEventArgs> Start;
        public event EventHandler<FinishEventArgs> Finish;
        public event EventHandler<ItemFindedEventArgs<FileInfo>> FileFinded;
        public event EventHandler<ItemFindedEventArgs<FileInfo>> FilteredFileFinded;
        public event EventHandler<ItemFindedEventArgs<DirectoryInfo>> DirectoryFinded;
        public event EventHandler<ItemFindedEventArgs<DirectoryInfo>> FilteredDirectoryFinded;

        public IEnumerable<FileSystemInfo> GetFileSystemInfoSequence()
        {
            List<FileSystemInfo> fileSystemInfos = new List<FileSystemInfo>();
            OnEvent(Start, new StartEventArgs());
            BypassFileSystem(_startDirectory, fileSystemInfos);
            OnEvent(Finish, new FinishEventArgs());
            return fileSystemInfos;
        }

        private ActionType BypassFileSystem(DirectoryInfo directory, List<FileSystemInfo> resultSequence)
        {
            ActionType action = ActionType.ContinueSearch;
            foreach (var fileSystemInfo in directory.EnumerateFileSystemInfos())
            {
                FileInfo file = fileSystemInfo as FileInfo;
                if (file != null)
                {
                    action = ProcessFile(file, resultSequence);
                }

                DirectoryInfo dir = fileSystemInfo as DirectoryInfo;
                if (dir != null)
                {
                    action = ProcessDirectory(dir, resultSequence);
                    if (action == ActionType.ContinueSearch)
                    {
                        action = BypassFileSystem(dir, resultSequence);
                    }
                }

                if (action == ActionType.StopSearch)
                {
                    return action;
                }
            }

            return ActionType.ContinueSearch;
        }

        private ActionType ProcessFile(FileInfo file, List<FileSystemInfo> resultSequence)
        {
            ActionType action = ProcessItemFinded(file, FileFinded, FilteredFileFinded);
            if (action == ActionType.ContinueSearch)
            {
                resultSequence.Add(file);
            }

            return action;
        }

        private ActionType ProcessDirectory(DirectoryInfo directory, List<FileSystemInfo> resultSequence)
        {
            ActionType action = ProcessItemFinded(directory, DirectoryFinded, FilteredDirectoryFinded);
            if (action == ActionType.ContinueSearch)
            {
                resultSequence.Add(directory);
            }

            return action;
        }

        private ActionType ProcessItemFinded<TItemInfo>(
            TItemInfo itemInfo,
            EventHandler<ItemFindedEventArgs<TItemInfo>> itemFinded,
            EventHandler<ItemFindedEventArgs<TItemInfo>> filteredItemFinded)
            where TItemInfo : FileSystemInfo
        {
            ItemFindedEventArgs<TItemInfo> args = new ItemFindedEventArgs<TItemInfo>
            {
                FindedItem = itemInfo,
                ActionType = ActionType.ContinueSearch
            };
            OnEvent(itemFinded, args);

            if (args.ActionType != ActionType.ContinueSearch || _filter == null)
            {
                return args.ActionType;
            }

            if (_filter(itemInfo))
            {
                args = new ItemFindedEventArgs<TItemInfo>
                {
                    FindedItem = itemInfo,
                    ActionType = ActionType.ContinueSearch
                };
                OnEvent(filteredItemFinded, args);
                return args.ActionType;
            }

            return ActionType.SkipElement;
        }

        private void OnEvent<TArgs>(EventHandler<TArgs> someEvent, TArgs args)
        {
            someEvent?.Invoke(this, args);
        }
    }
}

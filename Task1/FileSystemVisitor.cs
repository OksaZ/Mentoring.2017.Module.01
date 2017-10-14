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
            OnEvent(Start, new StartEventArgs());
            IEnumerator<FileSystemInfo> fileSystemInfos = GetFileSystemInfoSequence(_startDirectory).GetEnumerator();
            foreach (var fileSystemInfo in GetFileSystemInfoSequence(_startDirectory))
            {
                yield return fileSystemInfo;
            }
            OnEvent(Finish, new FinishEventArgs());
        }

        private IEnumerable<FileSystemInfo> GetFileSystemInfoSequence(DirectoryInfo directory)
        {
            foreach (var fileSystemInfo in directory.EnumerateFileSystemInfos())
            {
                FileInfo file = fileSystemInfo as FileInfo;
                if (file != null)
                {
                    ItemFindedEventArgs<FileInfo> args =
                        ProccessItemFinded(file, FileFinded, FilteredFileFinded);
                    if (args == null || args.IsRemovedFromResults)
                    {
                        continue;
                    }
                    if (args.IsSearchStoped)
                    {
                        yield break;
                    }

                    yield return file;
                }

                DirectoryInfo dir = fileSystemInfo as DirectoryInfo;
                if (dir != null)
                {
                    ItemFindedEventArgs<DirectoryInfo> args =
                        ProccessItemFinded(dir, DirectoryFinded, FilteredDirectoryFinded);
                    if (args == null || args.IsRemovedFromResults)
                    {
                        continue;
                    }
                    if (args.IsSearchStoped)
                    {
                        yield break;
                    }

                    yield return dir;
                }

                if (dir != null)
                {
                    foreach (var innerDirectoryFileSystemInfo in GetFileSystemInfoSequence(dir))
                    {
                        yield return innerDirectoryFileSystemInfo;
                    }
                }
            }
        }

        protected ItemFindedEventArgs<TItemInfo> ProccessItemFinded<TItemInfo>(
            TItemInfo fileInfo,
            EventHandler<ItemFindedEventArgs<TItemInfo>> itemFinded,
            EventHandler<ItemFindedEventArgs<TItemInfo>> filteredItemFinded)
            where TItemInfo : FileSystemInfo
        {
            ItemFindedEventArgs<TItemInfo> args = new ItemFindedEventArgs<TItemInfo> { FindedItem = fileInfo };
            OnEvent(itemFinded, args);

            if (args.IsRemovedFromResults || args.IsSearchStoped)
            {
                return args;
            }

            if (_filter != null)
            {
                if (_filter(fileInfo))
                {
                    args = new ItemFindedEventArgs<TItemInfo> { FindedItem = fileInfo };
                    OnEvent(filteredItemFinded, args);
                    return args;
                }
                else
                {
                    return null;
                }
            }

            return args;
        }

        protected void OnEvent<TArgs>(
            EventHandler<TArgs> someEvent,
            TArgs args)
        {
            someEvent?.Invoke(this, args);
        }
    }
}

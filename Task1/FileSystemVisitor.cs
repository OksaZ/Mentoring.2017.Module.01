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
            OnStart();
            foreach (var fileSystemInfo in GetFileSystemInfoSequence(_startDirectory))
            {
                yield return fileSystemInfo;
            }
            OnFinish();
        }

        private IEnumerable<FileSystemInfo> GetFileSystemInfoSequence(DirectoryInfo directory)
        {
            foreach (var fileSystemInfo in directory.EnumerateFileSystemInfos())
            {
                FileInfo file = fileSystemInfo as FileInfo;
                DirectoryInfo dir = fileSystemInfo as DirectoryInfo;
                ItemFindedEventArgs<FileInfo> args =
                    OnItemFinded<ItemFindedEventArgs<FileInfo>, FileInfo>(FileFinded,
                        new ItemFindedEventArgs<FileInfo> {FindedItem = file});
                //args = OnDirectoryFinded(dir) ?? args;

                if (args != null)
                {
                    if (args.IsRemovedFromResults)
                    {
                        continue;
                    }
                    if (args.IsSearchStoped)
                    {
                        yield break;
                    }
                }
                if (_filter == null || _filter(fileSystemInfo))
                {
                    args = OnItemFinded<ItemFindedEventArgs<FileInfo>, FileInfo>(FilteredFileFinded,
                        new ItemFindedEventArgs<FileInfo> {FindedItem = file});
                    //args = OnFilteredDirectoryFinded(dir) ?? args;
                    if (args != null)
                    {
                        if (args.IsRemovedFromResults)
                        {
                            continue;
                        }
                        if (args.IsSearchStoped)
                        {
                            yield break;
                        }
                    }
                    yield return fileSystemInfo;
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

        //protected void ProcessFindedItem()
        //{
        //    ItemFindedEventArgs<FileInfo> args =
        //        OnItemFinded<ItemFindedEventArgs<FileInfo>, FileInfo>(FileFinded,
        //            new ItemFindedEventArgs<FileInfo> { FindedItem = file });
        //    //args = OnDirectoryFinded(dir) ?? args;

        //    if (args != null)
        //    {
        //        if (args.IsRemovedFromResults)
        //        {
        //            continue;
        //        }
        //        if (args.IsSearchStoped)
        //        {
        //            yield break;
        //        }
        //    }
        //    if (_filter == null || _filter(fileSystemInfo))
        //    {
        //        args = OnItemFinded<ItemFindedEventArgs<FileInfo>, FileInfo>(FilteredFileFinded,
        //            new ItemFindedEventArgs<FileInfo> { FindedItem = file });
        //        //args = OnFilteredDirectoryFinded(dir) ?? args;
        //        if (args != null)
        //        {
        //            if (args.IsRemovedFromResults)
        //            {
        //                continue;
        //            }
        //            if (args.IsSearchStoped)
        //            {
        //                yield break;
        //            }
        //        }
        //        yield return fileSystemInfo;
        //    }
        //}


        protected void OnStart()
        {
            Start?.Invoke(this, new StartEventArgs());
        }

        protected void OnFinish()
        {
            Finish?.Invoke(this, new FinishEventArgs());
        }

        protected TArgs OnItemFinded<TArgs, TItem>(EventHandler<TArgs> onFinded, TArgs args)
            where TItem: FileSystemInfo
            where TArgs: ItemFindedEventArgs<TItem>
        {
            if (args.FindedItem == null)
            {
                return null;
            }

            onFinded?.Invoke(this, args);
            return args;
        }
    }
}

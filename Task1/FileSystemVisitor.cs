using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
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
        public event EventHandler<FileFindedEventArgs> FileFinded;
        public event EventHandler<FilteredFileFindedEventArgs> FilteredFileFinded;
        public event EventHandler<DirectoryFindedEventArgs> DirectoryFinded;
        public event EventHandler<FilteredDirectoryFindedEventArgs> FilteredDirectoryFinded;

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
                ElementFindedEventArgs args = OnFileFinded(file);
                args = OnDirectoryFinded(dir) ?? args;

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
                    args = OnFilteredFileFinded(file);
                    args = OnFilteredDirectoryFinded(dir) ?? args;
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

        protected void OnStart()
        {
            Start?.Invoke(this, new StartEventArgs());
        }

        protected void OnFinish()
        {
            Finish?.Invoke(this, new FinishEventArgs());
        }

        protected FileFindedEventArgs OnFileFinded(FileSystemInfo file)
        {
            if (file == null)
            {
                return null;
            }

            var args = new FileFindedEventArgs
            {
                File = file
            };

            FileFinded?.Invoke(this, args);
            return args;
        }

        protected FilteredFileFindedEventArgs OnFilteredFileFinded(FileSystemInfo filteredFile)
        {
            if (filteredFile == null)
            {
                return null;
            }

            var args = new FilteredFileFindedEventArgs
            {
                FilteredFile = filteredFile
            };

            FilteredFileFinded?.Invoke(this, args);
            return args;
        }

        protected DirectoryFindedEventArgs OnDirectoryFinded(DirectoryInfo directory)
        {
            if (directory == null)
            {
                return null;
            }

            var args = new DirectoryFindedEventArgs
            {
                Directory = directory
            };

            DirectoryFinded?.Invoke(this, args);
            return args;
        }

        protected FilteredDirectoryFindedEventArgs OnFilteredDirectoryFinded(DirectoryInfo filteredDirectory)
        {
            if (filteredDirectory == null)
            {
                return null;
            }

            var args = new FilteredDirectoryFindedEventArgs
            {
                FilteredDirectory = filteredDirectory
            };

            FilteredDirectoryFinded?.Invoke(this, args);
            return args;
        }
    }
}

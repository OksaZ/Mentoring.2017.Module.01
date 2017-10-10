using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public class FileSystemVisitor
    {
        private DirectoryInfo _startDirectory;

        public FileSystemVisitor(string path)
        {
            _startDirectory = new DirectoryInfo(path);
        }

        public IEnumerable<FileSystemInfo> GetFileSystemInfoSequence()
        {
            return GetFileSystemInfoSequence(_startDirectory);
        }

        private IEnumerable<FileSystemInfo> GetFileSystemInfoSequence(DirectoryInfo directory)
        {
            foreach (var fileSystemInfo in directory.EnumerateFileSystemInfos())
            {
                yield return fileSystemInfo;
                DirectoryInfo innerDirectory = fileSystemInfo as DirectoryInfo;
                if (innerDirectory != null)
                {
                    foreach (var innerDirectoryFileSystemInfo in GetFileSystemInfoSequence(innerDirectory))
                    {
                        yield return innerDirectoryFileSystemInfo;
                    }
                }
            }
        }
    }
}

using System.IO;

namespace Task1.EventArgs
{
    public class FileFindedEventArgs : ElementFindedEventArgs
    {
        public FileSystemInfo File { get; set; }
    }
}
using System.IO;

namespace Task1.EventArgs
{
    public class FilteredFileFindedEventArgs : ElementFindedEventArgs
    {
        public FileSystemInfo FilteredFile { get; set; }
    }
}
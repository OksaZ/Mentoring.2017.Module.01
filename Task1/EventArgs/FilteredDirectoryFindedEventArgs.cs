using System.IO;

namespace Task1.EventArgs
{
    public class FilteredDirectoryFindedEventArgs : ElementFindedEventArgs
    {
        public DirectoryInfo FilteredDirectory { get; set; }
    }
}
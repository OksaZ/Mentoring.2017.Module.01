using System.IO;

namespace Task1.EventArgs
{
    public class DirectoryFindedEventArgs : ElementFindedEventArgs
    {
        public DirectoryInfo Directory { get; set; }
    }
}
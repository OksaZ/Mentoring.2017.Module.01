using System.IO;

namespace Task1.EventArgs
{
    public class ItemFindedEventArgs<T> : System.EventArgs 
        where T : FileSystemInfo
    {
        public T FindedItem { get; set; }
        public bool IsRemovedFromResults { get; set; }
        public bool IsSearchStoped { get; set; }
    }
}
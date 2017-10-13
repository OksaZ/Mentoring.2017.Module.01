namespace Task1.EventArgs
{
    public class ElementFindedEventArgs : System.EventArgs
    {
        public bool IsRemovedFromResults { get; set; }
        public bool IsSearchStoped { get; set; }
    }
}
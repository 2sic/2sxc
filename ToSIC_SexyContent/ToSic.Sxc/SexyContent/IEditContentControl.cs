namespace ToSic.SexyContent
{
    /// <summary>
    /// Interface used in EditContentGroupItem
    /// Allows editing of ContentGroupItems and Entities
    /// </summary>
    interface IEditContentControl
    {
        void Cancel();
        void Save();
        bool IsPublished { get; set; }
    }
}

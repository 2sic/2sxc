#if NETFRAMEWORK



// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Search
{
    /// <remarks>
    /// This is part of a public API, so don't touch the name or namespace
    /// </remarks>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface ISearchInfo
    {
        string UniqueKey { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Body { get; set; }
        string Url { get; set; }
        DateTime ModifiedTimeUtc { get; set; }
        bool IsActive { get; set; }
        string QueryString { get; set; }
        string CultureCode { get; set; }
        IEntity Entity { get; set; }
    }
}
#endif
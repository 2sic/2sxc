namespace ToSic.Sxc.Search.Sys;

/// <summary>Search data before conversion to the platform's index document.</summary>
/// <remarks>
/// Mutable input for ICustomizeSearch. Oqtane maps it to a new document so adding an
/// occurrence-specific key, URL or permission string does not change the custom item.
/// </remarks>
[PrivateApi]
public sealed class SearchItemData : ISearchItem
{
    public string UniqueKey { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Body { get; set; } = "";
    public string Url { get; set; } = "";
    public DateTime ModifiedTimeUtc { get; set; }
    public bool IsActive { get; set; } = true;
    public string QueryString { get; set; } = "";
    public string CultureCode { get; set; } = "";
    public IEntity Entity { get; set; } = null!;
}

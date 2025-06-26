//#if !NETFRAMEWORK
//#pragma warning disable CS0109
//#endif

namespace ToSic.Sxc.Search;

/// <summary>
/// Defines an item in the search system - which is prepared by Sxc, and can be customized as needed
/// </summary>
[PublicApi]
public interface ISearchItem
//#if NETFRAMEWORK
//    : ToSic.SexyContent.Search.ISearchInfo // backward compatibility
//#endif
{
     string UniqueKey { get; set; }

    /// <summary>
    /// Title in search results
    /// </summary>
     string Title { get; set; }

    /// <summary>
    /// Description in search results
    /// </summary>
     string Description { get; set; }

    /// <summary>
    /// Contents of this item - will be indexed
    /// </summary>
     string Body { get; set; }

    /// <summary>
    /// Url to go to, when looking at the details of this search result
    /// </summary>
     string Url { get; set; }

    /// <summary>
    /// Timestamp in GMT / UTC
    /// </summary>
     DateTime ModifiedTimeUtc { get; set; }

    /// <summary>
    /// Determines if this item should appear in search or be ignored
    /// </summary>
     bool IsActive { get; set; }

    /// <summary>
    /// Query String params to access this item
    /// </summary>
     string QueryString { get; set; }

    /// <summary>
    /// Culture code, for language sensitive searches
    /// </summary>
     string CultureCode { get; set; }

    /// <summary>
    /// The underlying data in the search
    /// </summary>
     IEntity Entity { get; set; }

}
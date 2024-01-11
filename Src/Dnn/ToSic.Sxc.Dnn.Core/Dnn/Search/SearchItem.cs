using DotNetNuke.Services.Search.Entities;
using ToSic.Eav.Data;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Search;

/// <summary>
/// A search item which is passed around before handed over to the indexing system
/// </summary>
[PublicApi]
public class SearchItem : SearchDocument, ISearchItem
{
    public IEntity Entity { get; set; }
}
using Oqtane.Models;
using System;
using ToSic.Eav.Data;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Search;

/// <summary>
/// A search item which is passed around before handed over to the indexing system
/// </summary>
[PublicApi]
public class SearchItem : SearchContent, ISearchItem
{
    public IEntity Entity { get; set; }
    public DateTime ModifiedTimeUtc { get ; set; }
    public bool IsActive { get ; set; }

    public new string UniqueKey { get => $"{base.UniqueKey}:{_uniqueKey}"; set => _uniqueKey = value; }
    private string _uniqueKey = string.Empty;

    public string QueryString { get; set; }
    public string CultureCode { get; set; }
}
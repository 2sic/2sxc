using ToSic.Sxc.Context;

namespace ToSic.Sxc.Search;

public interface ISearchController<T>
{
    /// <summary>
    /// Get search info for each cms module containing 2sxc data
    /// </summary>
    /// <returns></returns>
    IList<T> GetModifiedSearchDocuments(IModule module, DateTime beginDate);

    /// <inheritdoc />
    ILog Log { get; }
}
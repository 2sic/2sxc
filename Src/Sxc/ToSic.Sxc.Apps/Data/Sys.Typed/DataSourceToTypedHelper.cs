using ToSic.Eav.Data.Sys.Entities;
using ToSic.Sxc.Data.Models.Sys;
using ToSic.Sxc.Data.Sys.Factory;
using static ToSic.Eav.DataSource.DataSourceConstants;

namespace ToSic.Sxc.Data.Sys.Typed;
internal class DataSourceToTypedHelper(ICodeDataFactory cdf, IDataSource dataSource, ILog? parentLog) : HelperBase(parentLog, "Sxc.Ds2Typ")
{

    internal IEnumerable<T>? GetAllShared<T>(string? typeName, bool nullIfNotFound, bool useDefaultIfNameNotSetAndNotFound)
        where T : class, ICanWrapData
    {
        var autoUseDefault = typeName == null && useDefaultIfNameNotSetAndNotFound;

        var streamNames = typeName == null
            ? DataModelAnalyzer.GetStreamNameList<T>()
            : autoUseDefault
                ? [typeName, StreamDefaultName]
                : [typeName];

        // Get the list - will be null if not found
        IDataStream? list = null;
        foreach (var name in streamNames)
            list ??= dataSource.GetStream(name, nullIfNotFound: true);

        // New for queries - which may use a type name but still expect to use the default stream
        if (list == null && autoUseDefault)
            list ??= dataSource.GetStream(StreamDefaultName, nullIfNotFound: true);

        // If we didn't find anything yet, then we must now try to re-access the stream
        // but in a way which will throw an exception with the expected stream names
        if (list == null && !nullIfNotFound)
            list = dataSource.GetStream(string.Join(",", streamNames), nullIfNotFound: false);

        return list == null
            ? null
            : cdf.AsCustomList<T>(source: list, npo: default, nullIfNull: nullIfNotFound);
    }

    /// <inheritdoc />
    public T? GetOne<T>(int id, NoParamOrder npo, bool skipTypeCheck)
        where T : class, ICanWrapData
        => cdf.GetOne<T>(() => dataSource.List.GetOne(id), id, skipTypeCheck);

    /// <inheritdoc />
    public T? GetOne<T>(Guid id, NoParamOrder npo, bool skipTypeCheck)
        where T : class, ICanWrapData
        => cdf.GetOne<T>(() => dataSource.List.GetOne(id), id, skipTypeCheck);

}

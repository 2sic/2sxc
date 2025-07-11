using System.Collections;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.DataSource.Sys.Caching;
using ToSic.Sxc.Apps.Sys.Api01;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Models.Sys;
using ToSic.Sxc.Data.Sys.Factory;
using static ToSic.Eav.DataSource.DataSourceConstants;

namespace ToSic.Sxc.Apps.Sys.AppTyped;

internal class AppDataTyped(
    Eav.DataSources.App.Dependencies services,
    LazySvc<SimpleDataEditService> dataController,
    LazySvc<IDataSourceCacheService> dsCacheSvc)
    : AppDataWithCrud(services, dataController, dsCacheSvc), IAppDataTyped
{
    #region Get Content Types - explicit implementation to ensure it's only available in Typed APIs

    IEnumerable<IContentType> IAppDataTyped.GetContentTypes()
        => AppReader.ContentTypes;

    IContentType? IAppDataTyped.GetContentType(string name)
        => AppReader.TryGetContentType(name);

    #endregion

    #region Kit Attachments

    internal AppDataTyped Setup(ICodeDataFactory cdfConnected)
    {
        CdfConnected = cdfConnected;
        return this;
    }

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory CdfConnected
    {
        get => field ?? throw new(nameof(CdfConnected) + " not set");
        set;
    }

    #endregion

    /// <inheritdoc />
    IEnumerable<T>? IAppDataTyped.GetAll<T>(NoParamOrder protector, string? typeName, bool nullIfNotFound)
        => GetAllShared<T>(this, CdfConnected, typeName, nullIfNotFound, false);

    internal static IEnumerable<T>? GetAllShared<T>(DataSourceBase dataSource, ICodeDataFactory cdf, string? typeName, bool nullIfNotFound, bool useDefaultIfNameNotSetAndNotFound)
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
            : cdf.AsCustomList<T>(source: list, protector: default, nullIfNull: nullIfNotFound);
    }

    /// <inheritdoc />
    T? IAppDataTyped.GetOne<T>(int id, NoParamOrder protector, bool skipTypeCheck)
        where T : class
        => CdfConnected.GetOne<T>(() => List.One(id), id, skipTypeCheck);

    /// <inheritdoc />
    T? IAppDataTyped.GetOne<T>(Guid id, NoParamOrder protector, bool skipTypeCheck)
        where T : class
        => CdfConnected.GetOne<T>(() => List.One(id), id, skipTypeCheck);
}
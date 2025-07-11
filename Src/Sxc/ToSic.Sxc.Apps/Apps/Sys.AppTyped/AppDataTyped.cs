using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.DataSource.Sys.Caching;
using ToSic.Sxc.Apps.Sys.Api01;
using ToSic.Sxc.Data.Models.Sys;
using ToSic.Sxc.Data.Sys.Factory;

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
    {
        var streamNames = typeName == null
            ? DataModelAnalyzer.GetStreamNameList<T>()
            : ([typeName], typeName);

        // Get the list - will be null if not found
        IDataStream? list = null;
        foreach (var streamName2 in streamNames.List)
            list ??= GetStream(streamName2, nullIfNotFound: true);

        // If we didn't find anything yet, then we must now try to re-access the stream
        // but in a way which will throw an exception with the expected stream names
        if (list == null && !nullIfNotFound)
            list = GetStream(streamNames.Flat, nullIfNotFound: false);

        return list == null
            ? null
            : CdfConnected.AsCustomList<T>(source: list, protector: protector, nullIfNull: nullIfNotFound);
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
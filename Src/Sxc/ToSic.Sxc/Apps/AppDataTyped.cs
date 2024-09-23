using ToSic.Eav.Apps.Internal.Api01;
using ToSic.Eav.DataSource.Internal.Caching;
using ToSic.Eav.DataSources.Internal;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Apps;

internal class AppDataTyped(
    Eav.DataSources.App.MyServices services,
    LazySvc<SimpleDataEditService> dataController,
    LazySvc<IDataSourceCacheService> dsCacheSvc)
    : AppDataWithCrud(services, dataController, dsCacheSvc), IAppDataTyped
{
    #region Content Types - explicit implementation to ensure it's only available in Typed APIs

    IEnumerable<IContentType> IAppDataTyped.GetContentTypes() => AppReader.ContentTypes;

    IContentType IAppDataTyped.GetContentType(string name) => AppReader.GetContentType(name);

    #endregion

    internal void Setup(ServiceKit16 kit)
    {
        _kit = kit;
    }

    private ServiceKit16 Kit => _kit ?? throw new("ServiceKit16 not set");
    private ServiceKit16 _kit;

    /// <inheritdoc />
    IEnumerable<T> IAppDataTyped.GetAll<T>(NoParamOrder protector, string typeName, bool nullIfNotFound)
    {
        typeName ??= new T().ForContentType;

        // Get the list - will be null if not found
        var list = GetStream(typeName, nullIfNotFound: nullIfNotFound);

        return list.NullOrGetWith(l => Kit._CodeApiSvc.Cdf.AsCustomList<T>(source: l, protector: protector, nullIfNull: nullIfNotFound));
    }

    /// <inheritdoc />
    T IAppDataTyped.GetOne<T>(int id, NoParamOrder protector, bool skipTypeCheck)
        => GetOne<T>(() => List.One(id), id, skipTypeCheck);

    /// <inheritdoc />
    T IAppDataTyped.GetOne<T>(Guid id, NoParamOrder protector, bool skipTypeCheck)
        => GetOne<T>(() => List.One(id), id, skipTypeCheck);


    private TResult GetOne<TResult>(Func<IEntity> getItem, object id, bool skipTypeCheck)
        where TResult : class, ITypedItemWrapper16, ITypedItem, new()
    {
        var item = getItem();
        if (item == null) return null;

        // Skip Type-Name check
        if (skipTypeCheck) return Kit._CodeApiSvc.Cdf.AsCustom<TResult>(item);

        // Do Type-Name check
        var typeName = new TResult().ForContentType;
        if (!item.Type.Is(typeName)) throw new($"Item with ID {id} is not a {typeName}. This is probably a mistake, otherwise use {nameof(skipTypeCheck)}: true");
        return Kit._CodeApiSvc.Cdf.AsCustom<TResult>(item);
    }



}
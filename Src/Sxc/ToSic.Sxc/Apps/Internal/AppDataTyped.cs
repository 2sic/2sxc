using ToSic.Eav.Apps.Internal.Api01;
using ToSic.Eav.DataSource.Internal.Caching;
using ToSic.Eav.DataSources.Internal;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Apps.Internal;

internal class AppDataTyped(
    Eav.DataSources.App.MyServices services,
    LazySvc<SimpleDataEditService> dataController,
    LazySvc<IDataSourceCacheService> dsCacheSvc)
    : AppDataWithCrud(services, dataController, dsCacheSvc), IAppDataTyped
{
    #region Get Content Types - explicit implementation to ensure it's only available in Typed APIs

    IEnumerable<IContentType> IAppDataTyped.GetContentTypes() => AppReader.ContentTypes;

    IContentType IAppDataTyped.GetContentType(string name) => AppReader.GetContentType(name);

    #endregion

    #region Kit Attachments

    internal AppDataTyped Setup(ServiceKit16 kit)
    {
        Kit = kit;
        return this;
    }

    private ServiceKit16 Kit { get => field ?? throw new("ServiceKit16 not set"); set => field = value; }

    #endregion

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
        => Kit._CodeApiSvc.Cdf.GetOne<T>(() => List.One(id), id, skipTypeCheck);

    /// <inheritdoc />
    T IAppDataTyped.GetOne<T>(Guid id, NoParamOrder protector, bool skipTypeCheck)
        => Kit._CodeApiSvc.Cdf.GetOne<T>(() => List.One(id), id, skipTypeCheck);
}
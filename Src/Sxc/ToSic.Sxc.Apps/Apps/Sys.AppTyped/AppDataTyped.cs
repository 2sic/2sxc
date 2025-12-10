using ToSic.Eav.DataSource.Sys.Caching;
using ToSic.Sxc.Apps.Sys.Api01;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Typed;

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

    #region Cdf Attachments and setup ToTypedHelper

    internal AppDataTyped Setup(ICodeDataFactory cdfConnected)
    {
        ToTypedHelper = new(cdfConnected, this, Log);
        return this;
    }

    [field: AllowNull, MaybeNull]
    private DataSourceToTypedHelper ToTypedHelper { get => field ?? throw new($"{nameof(ToTypedHelper)} not set"); set; }

    #endregion

    /// <inheritdoc />
    IEnumerable<T>? IAppDataTyped.GetAll<T>(NoParamOrder npo, string? typeName, bool nullIfNotFound)
        => ToTypedHelper.GetAllShared<T>(typeName, nullIfNotFound, false);

    /// <inheritdoc />
    T? IAppDataTyped.GetOne<T>(int id, NoParamOrder npo, bool skipTypeCheck)
        where T : class
        => ToTypedHelper.GetOne<T>(id, npo, skipTypeCheck);

    /// <inheritdoc />
    T? IAppDataTyped.GetOne<T>(Guid id, NoParamOrder npo, bool skipTypeCheck)
        where T : class
        => ToTypedHelper.GetOne<T>(id, npo, skipTypeCheck);
}
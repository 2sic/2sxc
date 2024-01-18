using ToSic.Eav.Apps.Internal.Api01;
using ToSic.Eav.DataSource.Internal.Caching;
using ToSic.Eav.DataSources.Internal;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Apps;

internal class AppDataTyped(
    Eav.DataSources.App.MyServices services,
    LazySvc<SimpleDataEditService> dataController,
    LazySvc<IDataSourceCacheService> dsCacheSvc)
    : AppDataWithCrud(services, dataController, dsCacheSvc), IAppDataTyped
{
    #region Content Types

    IEnumerable<IContentType> IAppDataTyped.GetContentTypes() => AppState.ContentTypes;

    IContentType IAppDataTyped.GetContentType(string name) => AppState.GetContentType(name);

    #endregion

}
using System.Collections.Generic;
using ToSic.Eav.Apps.Internal.Api01;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource.Internal.Caching;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Apps;

internal class AppDataTyped(
    Eav.DataSources.App.MyServices services,
    LazySvc<SimpleDataEditService> dataController,
    LazySvc<IDataSourceCacheService> dsCacheSvc)
    : ToSic.Eav.Apps.DataSources.AppDataWithCrud(services, dataController, dsCacheSvc), IAppDataTyped
{
    #region Content Types

    IEnumerable<IContentType> IAppDataTyped.GetContentTypes() => AppState.ContentTypes;

    IContentType IAppDataTyped.GetContentType(string name) => AppState.GetContentType(name);

    #endregion

}
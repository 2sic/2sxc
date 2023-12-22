using System.Collections.Generic;
using ToSic.Eav.Api.Api01;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource.Caching;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Apps
{
    internal class AppDataTyped: ToSic.Eav.Apps.DataSources.AppDataWithCrud, IAppDataTyped
    {
        public AppDataTyped(MyServices services, LazySvc<SimpleDataController> dataController, LazySvc<IDataSourceCacheService> dsCacheSvc) : base(services, dataController, dsCacheSvc)
        {
        }

        #region Content Types

        IEnumerable<IContentType> IAppDataTyped.GetContentTypes() => AppState.ContentTypes;

        IContentType IAppDataTyped.GetContentType(string name) => AppState.GetContentType(name);

        #endregion

    }
}

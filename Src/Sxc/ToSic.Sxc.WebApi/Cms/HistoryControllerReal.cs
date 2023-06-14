using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Lib.Logging;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Formats;
using ToSic.Lib.DI;
using ToSic.Lib.Services;

namespace ToSic.Sxc.WebApi.Cms
{
    // IMPORTANT: Uses the Proxy/Real concept - see https://go.2sxc.org/proxy-controllers

    public class HistoryControllerReal : ServiceBase, IHistoryController
    {
        public const string LogSuffix = "Hist";

        // #UnusedFeatureHistoryOfGroup 2022-07-05 2dm removed - probably clean up ca. Q4 2022
        public HistoryControllerReal(LazySvc<AppManager> appManagerLazy) : base("Api.CmsHistoryRl")
        {
            ConnectServices(
                _appManagerLazy = appManagerLazy
            );
        }

        private readonly LazySvc<AppManager> _appManagerLazy;


        public List<ItemHistory> Get(int appId, ItemIdentifier item)
            => _appManagerLazy.Value.Init(appId).Entities.VersionHistory(item.EntityId);


        public bool Restore(int appId, int changeId, ItemIdentifier item)
        {
            _appManagerLazy.Value.Init(appId).Entities.VersionRestore(item.EntityId, changeId);
            return true;
        }
    }
}

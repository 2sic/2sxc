using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Lib.Logging;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.Sxc.WebApi.Cms
{
    // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

    public class HistoryControllerReal : HasLog, IHistoryController
    {
        public const string LogSuffix = "Hist";

        // #UnusedFeatureHistoryOfGroup 2022-07-05 2dm removed - probably clean up ca. Q4 2022
        public HistoryControllerReal(Lazy<AppManager> appManagerLazy) : base("Api.CmsHistoryRl")
        {
            _appManagerLazy = appManagerLazy;
        }

        private readonly Lazy<AppManager> _appManagerLazy;


        public List<ItemHistory> Get(int appId, ItemIdentifier item)
            => _appManagerLazy.Value.Init(appId, Log).Entities.VersionHistory(item.EntityId);


        public bool Restore(int appId, int changeId, ItemIdentifier item)
        {
            _appManagerLazy.Value.Init(appId, Log).Entities.VersionRestore(item.EntityId, changeId);
            return true;
        }
    }
}

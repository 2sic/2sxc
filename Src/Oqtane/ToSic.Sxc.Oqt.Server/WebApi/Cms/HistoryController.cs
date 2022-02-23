using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Cms}")]

    // Beta routes - TODO: @STV - why is this beta?
    [Route(WebApiConstants.WebApiStateRoot + $"/{AreaRoutes.Cms}")]

    [ValidateAntiForgeryToken]
    public class HistoryController : OqtStatefulControllerBase<DummyControllerReal>, IHistoryController
    {
        public HistoryController(IdentifierHelper idHelper, Lazy<AppManager> appManagerLazy): base("History")
        {
            _idHelper = idHelper;
            _appManagerLazy = appManagerLazy;
        }
        private readonly IdentifierHelper _idHelper;
        private readonly Lazy<AppManager> _appManagerLazy;

        /// <summary>
        /// Used to be POST Entities/History
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public List<ItemHistory> Get(int appId, [FromBody] ItemIdentifier item)
            => _appManagerLazy.Value.Init(appId, Log).Entities.VersionHistory(_idHelper.Init(Log).ResolveItemIdOfGroup(appId, item, Log).EntityId);

        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public bool Restore(int appId, int changeId, [FromBody] ItemIdentifier item)
        {
            _appManagerLazy.Value.Init(appId, Log).Entities.VersionRestore(_idHelper.Init(Log).ResolveItemIdOfGroup(appId, item, Log).EntityId, changeId);
            return true;
        }

    }
}
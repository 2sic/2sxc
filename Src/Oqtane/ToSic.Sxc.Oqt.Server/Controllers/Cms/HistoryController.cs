using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [Route(WebApiConstants.WebApiStateRoot + "/cms/[controller]/[action]")]
    [ValidateAntiForgeryToken]
    public class HistoryController : SxcStatefulControllerBase, IHistoryController
    {
        private readonly IdentifierHelper _idHelper;
        protected override string HistoryLogName => "Api.History";

        public HistoryController(StatefulControllerDependencies dependencies, IdentifierHelper idHelper) : base(dependencies)
        {
            _idHelper = idHelper;
        }

        /// <summary>
        /// Used to be POST Entities/History
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = "EditModule")]  // TODO: disabled
        public List<ItemHistory> Get(int appId, [FromBody] ItemIdentifier item)
            => new AppManager(appId, Log).Entities.VersionHistory(_idHelper.Init(Log).ResolveItemIdOfGroup(appId, item, Log).EntityId);

        [HttpPost]
        //[Authorize(Policy = "EditModule")]  // TODO: disabled
        public bool Restore(int appId, int changeId, [FromBody] ItemIdentifier item)
        {
            new AppManager(appId, Log).Entities.VersionRestore(_idHelper.Init(Log).ResolveItemIdOfGroup(appId, item, Log).EntityId, changeId);
            return true;
        }

    }
}
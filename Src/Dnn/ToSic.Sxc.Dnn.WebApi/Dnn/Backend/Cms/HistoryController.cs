using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Dnn.WebApi.Internal;
using ToSic.Sxc.WebApi;
using RealController = ToSic.Sxc.Backend.Cms.HistoryControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

[SupportedModules(DnnSupportedModuleNames)]
[ValidateAntiForgeryToken]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HistoryController() : DnnSxcControllerBase(RealController.LogSuffix), IHistoryController
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public List<ItemHistory> Get(int appId, [FromBody] ItemIdentifier item)
        => Real.Get(appId, item);

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public bool Restore(int appId, int changeId, [FromBody] ItemIdentifier item) 
        => Real.Restore(appId, changeId, item);
}
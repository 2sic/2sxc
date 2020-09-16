using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Dnn.Pages;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.Usage;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class TemplateController
    {
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<ViewDto> Usage(int appId, Guid guid)
            => new UsageBackend().Init(Log)
                .ViewUsage(GetContext(), appId, guid,
                    (views, blocks) =>
                    {
                        // create array with all 2sxc modules in this portal
                        var allMods = new Pages(Log).AllModulesWithContent(PortalSettings.PortalId);
                        Log.Add($"Found {allMods.Count} modules");

                        return views.Select(vwb => new ViewDto().Init(vwb, blocks, allMods));
                    });
    }
}

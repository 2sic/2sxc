using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Pages;
using ToSic.Sxc.WebApi.DataTransferObjects.Usage;
using ToSic.Sxc.WebApi.Security;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class TemplateController
    {
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public dynamic Usage(int appId, Guid guid)
        {
            var wrapLog = Log.Call<dynamic>($"{appId}, {guid}");
            
            // extra security to only allow zone change if host user
            var permCheck = new MultiPermissionsApp(GetContext(), GetApp(appId), Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            var cms = new CmsRuntime(appId, Log, true);
            // treat view as a list - in case future code will want to analyze many views together
            var views = new List<IView> {cms.Views.Get(guid)};

            var blocks = cms.Blocks.AllWithView();

            Log.Add($"Found {blocks.Count} content blocks");

            // create array with all 2sxc modules in this portal
            var allMods = new Pages(Log).AllModulesWithContent(PortalSettings.PortalId);
            Log.Add($"Found {allMods.Count} modules");

            var result = views.Select(vwb => new ViewDto(vwb, blocks, allMods));

            return wrapLog("ok", result);
        }
    }
}

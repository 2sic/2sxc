using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.WebApi.Usage
{
    internal class UsageBackend: WebApiBackendBase<UsageBackend>
    {
        public UsageBackend() : base("Bck.Usage") { }

        public IEnumerable<ViewDto> ViewUsage(IInstanceContext context, int appId, Guid guid, 
            Func<List<IView>, List<BlockConfiguration>, IEnumerable<ViewDto>> finalBuilder)
        {
            var wrapLog = Log.Call<IEnumerable<ViewDto>>($"{appId}, {guid}");

            // extra security to only allow zone change if host user
            var permCheck = new MultiPermissionsApp().Init(context, GetApp(appId, null), Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            var cms = new CmsRuntime(appId, Log, true);
            // treat view as a list - in case future code will want to analyze many views together
            var views = new List<IView> { cms.Views.Get(guid) };

            var blocks = cms.Blocks.AllWithView();

            Log.Add($"Found {blocks.Count} content blocks");

            // create array with all 2sxc modules in this portal
            //var allMods = new Pages(Log).AllModulesWithContent(PortalSettings.PortalId);
            //Log.Add($"Found {allMods.Count} modules");

            var result = finalBuilder(views, blocks); // views.Select(vwb => new ViewDto().Init(vwb, blocks, allMods));

            return wrapLog("ok", result);
        }

    }
}

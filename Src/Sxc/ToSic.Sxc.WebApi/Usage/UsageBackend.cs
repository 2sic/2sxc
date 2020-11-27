using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Run.Context;
using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.WebApi.Usage
{
    internal class UsageBackend: WebApiBackendBase<UsageBackend>
    {
        private readonly CmsRuntime _cmsRuntime;
        public UsageBackend(CmsRuntime cmsRuntime, IServiceProvider serviceProvider) : base(serviceProvider, "Bck.Usage")
        {
            _cmsRuntime = cmsRuntime;
        }

        public IEnumerable<ViewDto> ViewUsage(IContextOfBlock context, int appId, Guid guid, 
            Func<List<IView>, List<BlockConfiguration>, IEnumerable<ViewDto>> finalBuilder)
        {
            var wrapLog = Log.Call<IEnumerable<ViewDto>>($"{appId}, {guid}");

            // extra security to only allow zone change if host user
            var permCheck = ServiceProvider.Build<MultiPermissionsApp>().Init(context, GetApp(appId, null), Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            var cms = _cmsRuntime.Init(State.Identity(null, appId), true, Log);
            // treat view as a list - in case future code will want to analyze many views together
            var views = new List<IView> { cms.Views.Get(guid) };

            var blocks = cms.Blocks.AllWithView();

            Log.Add($"Found {blocks.Count} content blocks");

            var result = finalBuilder(views, blocks);

            return wrapLog("ok", result);
        }

    }
}

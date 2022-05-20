using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Usage
{
    public class UsageBackend: WebApiBackendBase<UsageBackend>
    {
        private readonly CmsRuntime _cmsRuntime;
        private readonly IContextResolver _ctxResolver;

        public UsageBackend(CmsRuntime cmsRuntime, IServiceProvider serviceProvider, IContextResolver ctxResolver) : base(serviceProvider, "Bck.Usage")
        {
            _cmsRuntime = cmsRuntime;
            _ctxResolver = ctxResolver;
        }

        public IEnumerable<ViewDto> ViewUsage(int appId, Guid guid, Func<List<IView>, List<BlockConfiguration>, IEnumerable<ViewDto>> finalBuilder)
        {
            var wrapLog = Log.Fn<IEnumerable<ViewDto>>($"{appId}, {guid}");
            var context = _ctxResolver.BlockOrApp(appId);

            // extra security to only allow zone change if host user
            var permCheck = GetService<MultiPermissionsApp>().Init(context, context.AppState, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            var cms = _cmsRuntime.Init(context.AppState, context.UserMayEdit, Log);
            // treat view as a list - in case future code will want to analyze many views together
            var views = new List<IView> { cms.Views.Get(guid) };

            var blocks = cms.Blocks.AllWithView();

            Log.A($"Found {blocks.Count} content blocks");

            var result = finalBuilder(views, blocks);

            return wrapLog.Return(result, "ok");
        }
    }
}

using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Usage
{
    public class UsageBackend: ServiceBase
    {
        private readonly CmsRuntime _cmsRuntime;
        private readonly Generator<MultiPermissionsApp> _appPermissions;
        private readonly IContextResolver _ctxResolver;

        public UsageBackend(
            CmsRuntime cmsRuntime,
            Generator<MultiPermissionsApp> appPermissions,
            IContextResolver ctxResolver
            ) : base("Bck.Usage")
        {
            ConnectServices(
                _cmsRuntime = cmsRuntime,
                _appPermissions = appPermissions,
                _ctxResolver = ctxResolver
            );
        }

        public IEnumerable<ViewDto> ViewUsage(int appId, Guid guid, Func<List<IView>, List<BlockConfiguration>, IEnumerable<ViewDto>> finalBuilder)
        {
            var wrapLog = Log.Fn<IEnumerable<ViewDto>>($"{appId}, {guid}");
            var context = _ctxResolver.BlockOrApp(appId);

            // extra security to only allow zone change if host user
            var permCheck = _appPermissions.New().Init(context, context.AppState);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            var cms = _cmsRuntime.InitQ(context.AppState, context.UserMayEdit);
            // treat view as a list - in case future code will want to analyze many views together
            var views = new List<IView> { cms.Views.Get(guid) };

            var blocks = cms.Blocks.AllWithView();

            Log.A($"Found {blocks.Count} content blocks");

            var result = finalBuilder(views, blocks);

            return wrapLog.ReturnAsOk(result);
        }
    }
}

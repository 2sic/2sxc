using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.WebApi.Admin
{
    public partial class AdminBackend: WebApiBackendBase<AdminBackend>
    {
        public AdminBackend(IServiceProvider serviceProvider) : base(serviceProvider, "Bck.Admin")
        {
        }

        public dynamic DialogSettings(IInstanceContext context, ContextBuilderBase contextBuilder, int appId)
        {
            IApp app = null;
            // if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
            if (appId != 0 && appId != Eav.Constants.AppIdEmpty)
            {
                var appAndPerms = ServiceProvider.Build<MultiPermissionsApp>().Init(context, GetApp(appId, null), Log);
                if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var error))
                    throw HttpException.PermissionDenied(error);
                app = appAndPerms.App;
            }

            var cb = contextBuilder.InitApp(app?.ZoneId ?? context.Tenant.ZoneId, app);

            return new
            {
                Context = cb.Get(Ctx.All),
            };
        }

    }
}
